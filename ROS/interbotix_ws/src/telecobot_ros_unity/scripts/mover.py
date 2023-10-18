#!/usr/bin/env python3
# coding: utf-8

import sys
import rospy
import geometry_msgs.msg
import moveit_commander
from sensor_msgs.msg import JointState
from moveit_msgs.msg import RobotState
from moveit_msgs.msg import RobotTrajectory
from telecobot_ros_unity.msg import TelecobotMoveitJoints
from moveit_commander.conversions import pose_to_list

class telecobotMover:
    def __init__(self,subTopicName, pubTopicName):
        self.sub=rospy.Subscriber(subTopicName, TelecobotMoveitJoints, self.callback)
        self.pub=rospy.Publisher(pubTopicName, RobotTrajectory, queue_size=1)
        self.move_group = moveit_commander.MoveGroupCommander("interbotix_arm")
        self.joint_state = JointState()
        self.joint_state.name = ['waist', 'shoulder', 'elbow', 'wrist_angle', 'wrist_rotate']
        self.robot_state = RobotState()
        self.pose_goal=geometry_msgs.msg.Pose()

        ## Instantiate a `RobotCommander`_ object. This object is the outer-level interface to
        ## the robot:
        self.robot = moveit_commander.RobotCommander()
        ## Getting Basic Information
        ## ^^^^^^^^^^^^^^^^^^^^^^^^^
        # We can get the name of the reference frame for this robot:
        self.planning_frame = self.move_group.get_planning_frame()
        print("============ Reference frame: %s" % self.planning_frame)

        # We can also print the name of the end-effector link for this group:
        self.eef_link = self.move_group.get_end_effector_link()
        print("============ End effector: %s" % self.eef_link)

        # We can get a list of all the groups in the robot:
        self.group_names = self.robot.get_group_names()
        print("============ Robot Groups: " + str(self.group_names))

        # Sometimes for debugging it is useful to print the entire state of the
        # robot:
        print("============ Printing robot state")
        print(self.robot.get_current_state())
        print("")

    def callback(self, msg):
        # plan=self.plan_trajectory(self.move_group,msg.goal_pose,msg.joints)
        # self.move_group.execute(plan[1],wait=True)
        # self.move_group.stop()
        # self.move_group.clear_pose_targets()

        self.move_group.set_pose_target(msg.goal_pose)

        ## Now, we call the planner to compute the plan and execute it.
        plan = self.move_group.go(wait=True)
        # Calling `stop()` ensures that there is no residual movement
        self.move_group.stop()
        # It is always good to clear your targets after planning with poses.
        # Note: there is no equivalent function for clear_joint_value_targets()
        self.move_group.clear_pose_targets()


        if type(plan) is tuple:
            print("success>>")
        else:
            print("fail>>")

        # self.pub.publish(plan[1])

        current_pose = self.move_group.get_current_pose().pose
        return all_close(msg.goal_pose, current_pose, 0.01)
    
    def plan_trajectory(self,move_group,pose_target,start_joints):
        self.joint_state.position = start_joints
        self.robot_state.joint_state = self.joint_state
        self.move_group.set_start_state(self.robot_state)

        self.pose_goal.position=pose_target.position
        self.pose_goal.orientation=pose_target.orientation
        self.move_group.set_joint_value_target(self.pose_goal,True)

        return self.move_group.plan()
    
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
    rospy.init_node('telecobot_mover')
    node=telecobotMover("/telecobot_joints","/moveit_response")
    rospy.spin()

if __name__ == "__main__":
    main()