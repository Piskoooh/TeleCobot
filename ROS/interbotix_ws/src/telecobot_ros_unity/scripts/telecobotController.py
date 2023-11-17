#!/usr/bin/env python3
# coding: utf-8

import sys
import copy
import threading
import rospy
import geometry_msgs.msg
import moveit_commander
import numpy as np
from telecobot_ros_unity.msg import TelecobotUnityControl
from interbotix_common_modules import angle_manipulation as ang
from interbotix_xs_modules.locobot import InterbotixLocobotXS
from moveit_commander.conversions import pose_to_list


class telecobotUnityController:
    def __init__(self,subTopicName, pubTopicName):
        self.waist_step = 0.06
        self.current_loop_rate = 40
        self.loop_rates = {"course" : 25, "fine" : 25}
        self.tUC_msg = TelecobotUnityControl()
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
        if self.arm_model is not None:
            self.rotate_step = 0.04
            self.translate_step = 0.01
            self.gripper_pressure_step = 0.125
            self.current_gripper_pressure = 0.5
            self.num_joints = self.locobot.arm.group_info.num_joints
            self.waist_index = self.locobot.arm.group_info.joint_names.index("waist")
            self.waist_ll = self.locobot.arm.group_info.joint_lower_limits[self.waist_index]
            self.waist_ul = self.locobot.arm.group_info.joint_upper_limits[self.waist_index]
            self.T_sy = np.identity(4)
            self.T_yb = np.identity(4)
            self.update_T_yb()
        rospy.Subscriber(subTopicName, TelecobotUnityControl, self.callback)
        self.move_group = moveit_commander.MoveGroupCommander("interbotix_arm")


    def update_speed(self, loop_rate):
        self.current_loop_rate = loop_rate
        self.rate = rospy.Rate(self.current_loop_rate)
        rospy.loginfo("Current loop rate is %d Hz." % self.current_loop_rate)

    def update_T_yb(self):
        T_sb = self.locobot.arm.get_ee_pose_command()
        rpy = ang.rotationMatrixToEulerAngles(T_sb[:3, :3])
        self.T_sy[:2,:2] = ang.yawToRotationMatrix(rpy[2])
        self.T_yb = np.dot(ang.transInv(self.T_sy), T_sb)

    def update_gripper_pressure(self, gripper_pressure):
        self.current_gripper_pressure = gripper_pressure
        self.locobot.gripper.set_pressure(self.current_gripper_pressure)
        rospy.loginfo("Gripper pressure is at %.2f%%." % (self.current_gripper_pressure * 100.0))

    def callback(self, msg):
        with self.tUC_mutex:
            self.tUC_msg = copy.deepcopy(msg)

        # Check the speed_cmd
        if (msg.speed_cmd == TelecobotUnityControl.SPEED_INC and self.current_loop_rate < 40):
            self.update_speed(self.current_loop_rate + 1)
        elif (msg.speed_cmd == TelecobotUnityControl.SPEED_DEC and self.current_loop_rate > 10):
            self.update_speed(self.current_loop_rate - 1)

        # Check the speed_toggle_cmd
        if (msg.speed_toggle_cmd == TelecobotUnityControl.SPEED_COURSE):
            self.loop_rates["fine"] = self.current_loop_rate
            rospy.loginfo("Switched to Course Control")
            self.update_speed(self.loop_rates["course"])
        elif (msg.speed_toggle_cmd == TelecobotUnityControl.SPEED_FINE):
            self.loop_rates["course"] = self.current_loop_rate
            rospy.loginfo("Switched to Fine Control")
            self.update_speed(self.loop_rates["fine"])

        # ここからはCoreの関数を呼び出すコマンドを記載
        if(msg.motor_cmd==TelecobotUnityControl.ENABLE_TORQUE):
            print("Recived torque_enable from Unity side...")
            self.locobot.dxl.robot_torque_enable("group",self.locobot.arm.group_name,enable=True)
            self.locobot.dxl.robot_torque_enable("group",self.locobot.camera.group_name,enable=True)
            self.locobot.dxl.robot_torque_enable("single","gripper",enable=True)
        elif(msg.motor_cmd==TelecobotUnityControl.DISABLE_TORQUE):
            print("Recived torque_disable from Unity side...")
            self.locobot.dxl.robot_torque_enable("group",self.locobot.arm.group_name,enable=False)
            self.locobot.dxl.robot_torque_enable("group",self.locobot.camera.group_name,enable=False)
            self.locobot.dxl.robot_torque_enable("single","gripper",enable=False)

        if(msg.reboot_cmd==TelecobotUnityControl.REBOOT_ERROR_MOTOR):
            print("Recived robot_reboot_motors(smart) from Unity side...")
            print("Start to smart reboot...")
            self.locobot.dxl.robot_reboot_motors("group",self.locobot.arm.group_name,enable=True,smart_reboot=True)
            self.locobot.dxl.robot_reboot_motors("group",self.locobot.camera.group_name,enable=True,smart_reboot=True)
            self.locobot.dxl.robot_reboot_motors("single","gripper",enable=True,smart_reboot=True)
        elif(msg.reboot_cmd==TelecobotUnityControl.REBOOT_ALL_MOTOR):
            print("Recived robot_reboot_motors(all) from Unity side...")
            print("Start to reboot all motors...")
            self.locobot.dxl.robot_reboot_motors("group",self.locobot.arm.group_name,enable=True,smart_reboot=False)
            self.locobot.dxl.robot_reboot_motors("group",self.locobot.camera.group_name,enable=True,smart_reboot=False)
            self.locobot.dxl.robot_reboot_motors("single","gripper",enable=True,smart_reboot=False)
        
        # check base_reset_odom_cmd
        if (msg.base_reset_odom_cmd == TelecobotUnityControl.RESET_ODOM and self.use_base):
            if self.base_type == 'kobuki':
                self.locobot.base.reset_odom()
            elif self.base_type == 'create3':
                rospy.logwarn("Can't reset odometry using a Create 3 base.")

        if self.arm_model is None:  return
 
        # Check the gripper_cmd
        if (msg.gripper_cmd == TelecobotUnityControl.GRIPPER_OPEN):
            self.locobot.gripper.open(delay=0)
        elif (msg.gripper_cmd == TelecobotUnityControl.GRIPPER_CLOSE):
            self.locobot.gripper.close(delay=0)

        # Check the gripper_pwm_cmd
        if (msg.gripper_pwm_cmd == TelecobotUnityControl.GRIPPER_PWM_INC and self.current_gripper_pressure < 1):
            self.update_gripper_pressure(self.current_gripper_pressure + self.gripper_pressure_step)
        elif (msg.gripper_pwm_cmd == TelecobotUnityControl.GRIPPER_PWM_DEC and self.current_gripper_pressure > 0):
            self.update_gripper_pressure(self.current_gripper_pressure - self.gripper_pressure_step)

    def controller(self):
        with self.tUC_mutex:
            msg = copy.deepcopy(self.tUC_msg)

        # check if the pan-and-tilt mechanism should be reset
        if (msg.pan_cmd == TelecobotUnityControl.PAN_TILT_HOME and
            msg.tilt_cmd == TelecobotUnityControl.PAN_TILT_HOME):
            self.locobot.camera.pan_tilt_go_home(1.0, 0.5, 1.0, 0.5, False)

        # check if the pan/tilt mechanism should be rotated
        elif (msg.pan_cmd != 0 or msg.tilt_cmd != 0):
            cam_positions = self.locobot.camera.get_joint_commands()

            if (msg.pan_cmd == TelecobotUnityControl.PAN_CCW):
                cam_positions[0] += self.waist_step
            elif (msg.pan_cmd == TelecobotUnityControl.PAN_CW):
                cam_positions[0] -= self.waist_step

            if (msg.tilt_cmd == TelecobotUnityControl.TILT_UP):
                cam_positions[1] += self.waist_step
            elif (msg.tilt_cmd == TelecobotUnityControl.TILT_DOWN):
                cam_positions[1] -= self.waist_step

            self.locobot.camera.pan_tilt_move(cam_positions[0], cam_positions[1], 0.2, 0.1, 0.2, 0.1, False)

        # check base related commands
        if self.use_base:
            if(msg.base_cmd==TelecobotUnityControl.MOVE_BASE):
                print("Received pose_data from Unity side...")
                print("Start planning route...")
                self.locobot.base.move_to_pose(x=msg.pose_data[0],y=msg.pose_data[1],yaw=msg.pose_data[2],wait=True)
            else: 
                self.locobot.base.command_velocity(msg.base_x_cmd, msg.base_theta_cmd)

        if self.arm_model is None: return

        # Check the pose_cmd
        if (msg.pose_cmd != 0):
            if (msg.pose_cmd == TelecobotUnityControl.HOME_POSE):
                self.locobot.arm.go_to_home_pose(1.5, 0.75)
            elif (msg.pose_cmd == TelecobotUnityControl.SLEEP_POSE):
                self.locobot.arm.go_to_sleep_pose(1.5, 0.75)
            self.update_T_yb()

        # Check the waist_cmd
        if (msg.waist_cmd != 0):
            waist_position = self.locobot.arm.get_single_joint_command("waist")
            if (msg.waist_cmd == TelecobotUnityControl.WAIST_CCW):
                success = self.locobot.arm.set_single_joint_position("waist", waist_position + self.waist_step, 0.2, 0.1, False)
                if (success == False and waist_position != self.waist_ul):
                    self.locobot.arm.set_single_joint_position("waist", self.waist_ul, 0.2, 0.1, False)
            elif (msg.waist_cmd == TelecobotUnityControl.WAIST_CW):
                success = self.locobot.arm.set_single_joint_position("waist", waist_position - self.waist_step, 0.2, 0.1, False)
                if (success == False and waist_position != self.waist_ll):
                    self.locobot.arm.set_single_joint_position("waist", self.waist_ll, 0.2, 0.1, False)
            self.update_T_yb()

        # Check the arm_cmd
        if(msg.arm_cmd!=0):
            if(msg.arm_cmd==TelecobotUnityControl.SET_EE_CARTESIAN_TRAJECTORY):
                print("Received pose_data from Unity side...")
                print(msg.pose_data)
                print("Start to calculate trajectory...")
                if(abs(msg.pose_data[1])<0.01):
                    result=self.locobot.arm.set_ee_cartesian_trajectory(x=msg.pose_data[0],y=0,z=msg.pose_data[2],roll=msg.pose_data[3],pitch=msg.pose_data[4])
                else:
                    result=self.locobot.arm.set_ee_cartesian_trajectory(x=msg.pose_data[0],y=msg.pose_data[1],z=msg.pose_data[2],roll=msg.pose_data[3],pitch=msg.pose_data[4])
                if(result):
                    print("success>>")
                else:
                    print("fail>>")
                    print("Start to calculate trajectory with anothor method...")
                    result2=self.locobot.arm.set_relative_ee_position_wrt_to_base_frame(dx=msg.pose_data[0],dy=msg.pose_data[1],dz=msg.pose_data[2])
                    if(result2):
                        print("success>>")
                        print("Finish to calculate trajectory...")
                    else:
                        print("fail>>")
                        print("All methods failed to calculate trajectory...")

            elif(msg.arm_cmd==TelecobotUnityControl.MOVEIT):
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
            self.update_T_yb()

        else:    
            position_changed = msg.ee_x_cmd + msg.ee_z_cmd
            if (self.num_joints >= 6):
                position_changed += msg.ee_y_cmd
            orientation_changed = msg.ee_roll_cmd + msg.ee_pitch_cmd

            if (position_changed + orientation_changed == 0): return

            # Copy the most recent T_yb transform into a temporary variable
            T_yb = np.array(self.T_yb)

            if (position_changed):
                # check ee_x_cmd
                if (msg.ee_x_cmd == TelecobotUnityControl.EE_X_INC):
                    T_yb[0, 3] += self.translate_step
                elif (msg.ee_x_cmd == TelecobotUnityControl.EE_X_DEC):
                    T_yb[0, 3] -= self.translate_step

                # check ee_y_cmd
                if (msg.ee_y_cmd == TelecobotUnityControl.EE_Y_INC and self.num_joints >= 6 and T_yb[0, 3] > 0.3):
                    T_yb[1, 3] += self.translate_step
                elif (msg.ee_y_cmd == TelecobotUnityControl.EE_Y_DEC and self.num_joints >= 6 and T_yb[0, 3] > 0.3):
                    T_yb[1, 3] -= self.translate_step

                # check ee_z_cmd
                if (msg.ee_z_cmd == TelecobotUnityControl.EE_Z_INC):
                    T_yb[2, 3] += self.translate_step
                elif (msg.ee_z_cmd == TelecobotUnityControl.EE_Z_DEC):
                    T_yb[2, 3] -= self.translate_step

            # check end-effector orientation related commands
            if (orientation_changed != 0):
                rpy = ang.rotationMatrixToEulerAngles(T_yb[:3, :3])

                # check ee_roll_cmd
                if (msg.ee_roll_cmd == TelecobotUnityControl.EE_ROLL_CCW):
                    rpy[0] += self.rotate_step
                elif (msg.ee_roll_cmd == TelecobotUnityControl.EE_ROLL_CW):
                    rpy[0] -= self.rotate_step

                # check ee_pitch_cmd
                if (msg.ee_pitch_cmd == TelecobotUnityControl.EE_PITCH_DOWN):
                    rpy[1] += self.rotate_step
                elif (msg.ee_pitch_cmd == TelecobotUnityControl.EE_PITCH_UP):
                    rpy[1] -= self.rotate_step

                T_yb[:3,:3] = ang.eulerAnglesToRotationMatrix(rpy)

            # Get desired transformation matrix of the end-effector w.r.t. the base frame
            T_sd = np.dot(self.T_sy, T_yb)
            _, success = self.locobot.arm.set_ee_pose_matrix(T_sd, self.locobot.arm.get_joint_commands(), True, 0.2, 0.1, False)
            if (success):
                self.T_yb = np.array(T_yb)

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
        unityControl.controller()
        unityControl.rate.sleep()

if __name__ == '__main__':
    main()