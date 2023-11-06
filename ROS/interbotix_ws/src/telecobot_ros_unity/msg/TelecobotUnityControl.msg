# 0:move base, 1:set_ee_pose_components to pose, 2:moveit, 
# 900:enable torqe, 901:disable torque, 902:reboot error motor, 903:reboot all motors 
int8 cmd_id

# This is a list of poses.
# cmd_type==0 recives (x,y,yaw)
# cmd_type==1 recives (x,y,z,roll,pitch)
float64[] pose_data

geometry_msgs/Pose end_effector_pose
geometry_msgs/Pose goal_pose