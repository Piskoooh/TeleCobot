#!/usr/bin/env python3
# coding: utf-8

import sys
import copy
import threading
import rospy
import geometry_msgs.msg
import moveit_commander
from telecobot_ros_unity.msg import TelecobotUnityControl
from interbotix_xs_modules.locobot import InterbotixLocobotXS
from moveit_commander.conversions import pose_to_list


class telecobotUnityController:
    def __init__(self,subTopicName, pubTopicName):
        self.sub=rospy.Subscriber(subTopicName, TelecobotUnityControl, self.callback)
        self.current_loop_rate = 25
        self.loop_rates = {"course" : 25, "fine" : 25}
        self.tUC_mutex = threading.Lock()
        self.rate = rospy.Rate(self.current_loop_rate)
        self.use_base = rospy.get_param("~use_base")
        robot_model = rospy.get_param("~robot_model")
        self.base_type = rospy.get_param("~base_type")
        robot_name = rospy.get_namespace().strip("/")
        self.arm_model = "mobile_" + robot_model.split("_")[1]
        self.locobot = InterbotixLocobotXS(
            robot_model=robot_model,
            arm_model=self.arm_model,
            robot_name=robot_name,
            use_move_base_action=True,
            init_node=False
        )
        self.move_group = moveit_commander.MoveGroupCommander("interbotix_arm")


    def update_speed(self, loop_rate):
        self.current_loop_rate = loop_rate
        self.rate = rospy.Rate(self.current_loop_rate)
        rospy.loginfo("Current loop rate is %d Hz." % self.current_loop_rate)

    def callback(self, msg):
        with self.tUC_mutex:
            self.tUC_msg = copy.deepcopy(msg)

        print("cmd_id: ", msg.cmd_id)    
        if(msg.cmd_id==0):
            print("Received pose_data from Unity side...")
            print("Start planning route...")
            self.locobot.base.move_to_pose(x=msg.pose_data[0],y=msg.pose_data[1],yaw=msg.pose_data[2],wait=True)
        elif(msg.cmd_id==1):
            print("Received pose_data from Unity side...")
            print("Start to calculate trajectory...")
            self.locobot.arm.set_ee_cartesian_trajectory(x=msg.pose_data[0],y=0,z=msg.pose_data[2],roll=msg.pose_data[3],pitch=msg.pose_data[4])
        elif(msg.cmd_id==2):
            print("Received goal pose from Unity side...")
            # ターゲットとエンドエフェクタの誤差を計算
            delta = abs(msg.end_effector_pose.position.x - msg.goal_pose.position.x) \
                    + abs(msg.end_effector_pose.position.y - msg.goal_pose.position.y) \
                    + abs(msg.end_effector_pose.position.z - msg.goal_pose.position.z) \
                    + abs(msg.end_effector_pose.orientation.x - msg.goal_pose.orientation.x) \
                    + abs(msg.end_effector_pose.orientation.y - msg.goal_pose.orientation.y) \
                    + abs(msg.end_effector_pose.orientation.z - msg.goal_pose.orientation.z) \
                    + abs(msg.end_effector_pose.orientation.w - msg.goal_pose.orientation.w)
            print("delta: ", delta)
            print("goal_pose: ", msg.goal_pose)
            print("end_effector_pose: ", msg.end_effector_pose)
            if delta >0.02:
                self.move_group.set_pose_target(msg.goal_pose)
                ## Now, we call the planner to compute the plan and execute it.
                plan = self.move_group.go(wait=True)
                # Calling `stop()` ensures that there is no residual movement
                self.move_group.stop()
                # It is always good to clear your targets after planning with poses.
                # Note: there is no equivalent function for clear_joint_value_targets()
                self.move_group.clear_pose_targets()
                if plan:
                    print("success>>")
                else:
                    print("fail>>")
                current_pose = self.move_group.get_current_pose().pose
                return all_close(msg.goal_pose, current_pose, 0.01)
            
        # ここからはCoreの関数を呼び出すコマンドを記載
        elif(msg.cmd_id==900):
            print("Recived torque_enable from Unity side...")
            self.locobot.core.robot_torque_enable("group",self.locobot.arm_group_name,enable=True)
            self.locobot.core.robot_torque_enable("group",self.locobot.turret_group_name,enable=True)
            self.locobot.core.robot_torque_enable("group",self.locobot.gripper_name,enable=True)
        elif(msg.cmd_id==901):
            print("Recived torque_disable from Unity side...")
            self.locobot.core.robot_torque_enable("group",self.locobot.arm_group_name,enable=False)
            self.locobot.core.robot_torque_enable("group",self.locobot.turret_group_name,enable=False)
            self.locobot.core.robot_torque_enable("group",self.locobot.gripper_name,enable=False)
        elif(msg.cmd_id==902):
            print("Recived robot_reboot_motors(smart) from Unity side...")
            print("Start to smart reboot...")
            self.locobot.core.robot_reboot_motors("group",self.locobot.arm_group_name,enable=True,smart_reboot=True)
            self.locobot.core.robot_reboot_motors("group",self.locobot.turret_group_name,enable=True,smart_reboot=True)
            self.locobot.core.robot_reboot_motors("group",self.locobot.gripper_name,enable=True,smart_reboot=True)
        elif(msg.cmd_id==903):
            print("Recived robot_reboot_motors(all) from Unity side...")
            print("Start to reboot all motors...")
            self.locobot.core.robot_reboot_motors("group",self.locobot.arm_group_name,enable=True,smart_reboot=False)
            self.locobot.core.robot_reboot_motors("group",self.locobot.turret_group_name,enable=True,smart_reboot=False)
            self.locobot.core.robot_reboot_motors("group",self.locobot.gripper_name,enable=True,smart_reboot=False)
        else:
            print("Received unknown cmd_id from Unity side...")


    def controller(self):
        with self.tUC_mutex:
            msg = copy.deepcopy(self.tUC_msg)

def all_close(goal, actual, tolerance):
    """
    Convenience method for testing if a list of values are within a tolerance of their counterparts in another list
    @param: goal       A list of floats, a Pose or a PoseStamped
    @param: actual     A list of floats, a Pose or a PoseStamped
    @param: tolerance  A float
    @returns: bool
    """
    all_equal = True
    if type(goal) is list:
        for index in range(len(goal)):
            if abs(actual[index] - goal[index]) > tolerance:
                return False

    elif type(goal) is geometry_msgs.msg.PoseStamped:
        return all_close(goal.pose, actual.pose, tolerance)

    elif type(goal) is geometry_msgs.msg.Pose:
        return all_close(pose_to_list(goal), pose_to_list(actual), tolerance)

    return True

def main():
    moveit_commander.roscpp_initialize(sys.argv)
    rospy.init_node('telecobot_controller')
    unityControl=telecobotUnityController("/unity_control","/telecobot_responce")
    while not rospy.is_shutdown():
        unityControl.rate.sleep()

if __name__ == '__main__':
    main()