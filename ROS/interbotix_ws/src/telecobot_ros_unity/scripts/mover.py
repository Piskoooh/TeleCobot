#!/usr/bin/env python3
# coding: utf-8

import sys
import rospy
import geometry_msgs.msg
import moveit_commander
from sensor_msgs.msg import JointState
from moveit_msgs.msg import RobotState
from moveit_msgs.msg import RobotTrajectory
from telecobot_ros_unity.msg import telecobotMoveitJoints

class telecobotMover:
    def __init__(self,subTopicName, pubTopicName):
        self.sub=rospy.Subscriber(subTopicName, telecobotMoveitJoints, self.callback)
        self.pub=rospy.Publisher(pubTopicName, RobotTrajectory, queue_size=1)
        self.move_group = moveit_commander.MoveGroupCommander("arm")
        self.joint_state = JointState()
        self.joint_state.name = ['joint_1', 'joint_2', 'joint_3', 'joint_4', 'joint_5']
        self.robot_state = RobotState()
        self.pose_goal=geometry_msgs.msg.Pose()

    def callback(self, msg):
        plan=self.plan_trajectory(self.move_group,msg.goal_pose,msg.joints)
        self.move_group.stop()
        self.move_group.clear_pose_targets()

        if type(plan) is tuple:
            print("success>>")
        else:
            print("fail>>")

        self.pub.publish(plan[1])

    def plan_trajectory(self,move_group,pose_target,start_joints):
        self.joint_state.position = start_joints
        self.robot_state.joint_state = self.joint_state
        self.move_group.set_start_state(self.robot_state)

        self.pose_goal.position=pose_target.position
        self.pose_goal.orientation=pose_target.orientation
        self.move_group.set_joint_value_target(self.pose_goal,True)

        return self.move_group.plan()
    
def main():
    rospy.init_node('telecobot_mover')
    node=telecobotMover("telecobot_joints","moveit_response")
    rospy.spin()

if __name__ == "__main__":
    main()