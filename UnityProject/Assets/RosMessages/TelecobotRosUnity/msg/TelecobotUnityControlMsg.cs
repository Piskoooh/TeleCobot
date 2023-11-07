//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.TelecobotRosUnity
{
    [Serializable]
    public class TelecobotUnityControlMsg : Message
    {
        public const string k_RosMessageName = "telecobot_ros_unity/TelecobotUnityControl";
        public override string RosMessageName => k_RosMessageName;

        //  recieves commands from the unity and sends them to the robot
        public const sbyte MOVE_BASE = 29;
        public const sbyte SET_EE_CARTESIAN_TRAJECTORY = 30;
        public const sbyte MOVEIT = 31;
        public const sbyte ENABLE_TORQUE = 32;
        public const sbyte DISABLE_TORQUE = 33;
        public const sbyte REBOOT_ERROR_MOTOR = 34;
        public const sbyte REBOOT_ALL_MOTOR = 35;
        // ########################################################################################################
        public sbyte reboot_cmd;
        public sbyte motor_cmd;
        public sbyte arm_cmd;
        public sbyte base_cmd;
        // ########################################################################################################
        //  This is a list of poses.
        //  when move base recives (x,y,yaw)
        //  when set ee pose components recives (x,y,z,roll,pitch)
        public double[] pose_data;
        public Geometry.PoseMsg end_effector_pose;
        public Geometry.PoseMsg goal_pose;
        // ########################################################################################################
        // #############################################################################
        // ##     Below this line is copied from interbotix_xs_msgs/LocobotJoy.msg   ###
        // #############################################################################
        //  This message is used specifically in the interbotix_xslocobot_joy package
        // 
        //  Maps raw 'joy' commands to more specific ones to control an Interbotix LoCoBot
        //  enum values that define the joystick controls for the robot
        // ########################################################################################################
        //  Reset base odometry
        public const sbyte RESET_ODOM = 1;
        // ########################################################################################################
        //  Control the pan-and-tilt mechanism
        public const sbyte PAN_CCW = 2;
        public const sbyte PAN_CW = 3;
        public const sbyte TILT_UP = 4;
        public const sbyte TILT_DOWN = 5;
        public const sbyte PAN_TILT_HOME = 6;
        // ########################################################################################################
        //  Control the motion of the virtual 'ee_gripper_link' or end effector using the modern_robotics_ik engine
        //  Position Control
        public const sbyte EE_X_INC = 7;
        public const sbyte EE_X_DEC = 8;
        public const sbyte EE_Y_INC = 9;
        public const sbyte EE_Y_DEC = 10;
        public const sbyte EE_Z_INC = 11;
        public const sbyte EE_Z_DEC = 12;
        //  Orientation Control
        public const sbyte EE_ROLL_CCW = 13;
        public const sbyte EE_ROLL_CW = 14;
        public const sbyte EE_PITCH_UP = 15;
        public const sbyte EE_PITCH_DOWN = 16;
        // ########################################################################################################
        //  Control the motion of independent joints on the Arm or send predefined robot poses
        //  Waist Joint Control
        public const sbyte WAIST_CCW = 17;
        public const sbyte WAIST_CW = 18;
        //  Gripper Control
        public const sbyte GRIPPER_OPEN = 19;
        public const sbyte GRIPPER_CLOSE = 20;
        //  Pose Control
        public const sbyte HOME_POSE = 21;
        public const sbyte SLEEP_POSE = 22;
        // ########################################################################################################
        //  Customize configurations for the Interbotix Arm
        //  Inc/Dec Joint speed
        public const sbyte SPEED_INC = 23;
        public const sbyte SPEED_DEC = 24;
        //  Quickly toggle between a fast and slow speed setting
        public const sbyte SPEED_COURSE = 25;
        public const sbyte SPEED_FINE = 26;
        //  Inc/Dec Gripper pressure
        public const sbyte GRIPPER_PWM_INC = 27;
        public const sbyte GRIPPER_PWM_DEC = 28;
        // ########################################################################################################
        //  Control the motion of the Kobuki base
        public double base_x_cmd;
        public double base_theta_cmd;
        public sbyte base_reset_odom_cmd;
        //  Control the motion of the camera pan-and-tilt mechanism
        public sbyte pan_cmd;
        public sbyte tilt_cmd;
        //  Control the motion of the Interbotix Arm
        public sbyte ee_x_cmd;
        public sbyte ee_y_cmd;
        public sbyte ee_z_cmd;
        public sbyte ee_roll_cmd;
        public sbyte ee_pitch_cmd;
        //  Independent Joint/Pose Control
        public sbyte waist_cmd;
        public sbyte gripper_cmd;
        public sbyte pose_cmd;
        //  Arm Configs
        public sbyte speed_cmd;
        public sbyte speed_toggle_cmd;
        public sbyte gripper_pwm_cmd;

        public TelecobotUnityControlMsg()
        {
            this.reboot_cmd = 0;
            this.motor_cmd = 0;
            this.arm_cmd = 0;
            this.base_cmd = 0;
            this.pose_data = new double[0];
            this.end_effector_pose = new Geometry.PoseMsg();
            this.goal_pose = new Geometry.PoseMsg();
            this.base_x_cmd = 0.0;
            this.base_theta_cmd = 0.0;
            this.base_reset_odom_cmd = 0;
            this.pan_cmd = 0;
            this.tilt_cmd = 0;
            this.ee_x_cmd = 0;
            this.ee_y_cmd = 0;
            this.ee_z_cmd = 0;
            this.ee_roll_cmd = 0;
            this.ee_pitch_cmd = 0;
            this.waist_cmd = 0;
            this.gripper_cmd = 0;
            this.pose_cmd = 0;
            this.speed_cmd = 0;
            this.speed_toggle_cmd = 0;
            this.gripper_pwm_cmd = 0;
        }

        public TelecobotUnityControlMsg(sbyte reboot_cmd, sbyte motor_cmd, sbyte arm_cmd, sbyte base_cmd, double[] pose_data, Geometry.PoseMsg end_effector_pose, Geometry.PoseMsg goal_pose, double base_x_cmd, double base_theta_cmd, sbyte base_reset_odom_cmd, sbyte pan_cmd, sbyte tilt_cmd, sbyte ee_x_cmd, sbyte ee_y_cmd, sbyte ee_z_cmd, sbyte ee_roll_cmd, sbyte ee_pitch_cmd, sbyte waist_cmd, sbyte gripper_cmd, sbyte pose_cmd, sbyte speed_cmd, sbyte speed_toggle_cmd, sbyte gripper_pwm_cmd)
        {
            this.reboot_cmd = reboot_cmd;
            this.motor_cmd = motor_cmd;
            this.arm_cmd = arm_cmd;
            this.base_cmd = base_cmd;
            this.pose_data = pose_data;
            this.end_effector_pose = end_effector_pose;
            this.goal_pose = goal_pose;
            this.base_x_cmd = base_x_cmd;
            this.base_theta_cmd = base_theta_cmd;
            this.base_reset_odom_cmd = base_reset_odom_cmd;
            this.pan_cmd = pan_cmd;
            this.tilt_cmd = tilt_cmd;
            this.ee_x_cmd = ee_x_cmd;
            this.ee_y_cmd = ee_y_cmd;
            this.ee_z_cmd = ee_z_cmd;
            this.ee_roll_cmd = ee_roll_cmd;
            this.ee_pitch_cmd = ee_pitch_cmd;
            this.waist_cmd = waist_cmd;
            this.gripper_cmd = gripper_cmd;
            this.pose_cmd = pose_cmd;
            this.speed_cmd = speed_cmd;
            this.speed_toggle_cmd = speed_toggle_cmd;
            this.gripper_pwm_cmd = gripper_pwm_cmd;
        }

        public static TelecobotUnityControlMsg Deserialize(MessageDeserializer deserializer) => new TelecobotUnityControlMsg(deserializer);

        private TelecobotUnityControlMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.reboot_cmd);
            deserializer.Read(out this.motor_cmd);
            deserializer.Read(out this.arm_cmd);
            deserializer.Read(out this.base_cmd);
            deserializer.Read(out this.pose_data, sizeof(double), deserializer.ReadLength());
            this.end_effector_pose = Geometry.PoseMsg.Deserialize(deserializer);
            this.goal_pose = Geometry.PoseMsg.Deserialize(deserializer);
            deserializer.Read(out this.base_x_cmd);
            deserializer.Read(out this.base_theta_cmd);
            deserializer.Read(out this.base_reset_odom_cmd);
            deserializer.Read(out this.pan_cmd);
            deserializer.Read(out this.tilt_cmd);
            deserializer.Read(out this.ee_x_cmd);
            deserializer.Read(out this.ee_y_cmd);
            deserializer.Read(out this.ee_z_cmd);
            deserializer.Read(out this.ee_roll_cmd);
            deserializer.Read(out this.ee_pitch_cmd);
            deserializer.Read(out this.waist_cmd);
            deserializer.Read(out this.gripper_cmd);
            deserializer.Read(out this.pose_cmd);
            deserializer.Read(out this.speed_cmd);
            deserializer.Read(out this.speed_toggle_cmd);
            deserializer.Read(out this.gripper_pwm_cmd);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.reboot_cmd);
            serializer.Write(this.motor_cmd);
            serializer.Write(this.arm_cmd);
            serializer.Write(this.base_cmd);
            serializer.WriteLength(this.pose_data);
            serializer.Write(this.pose_data);
            serializer.Write(this.end_effector_pose);
            serializer.Write(this.goal_pose);
            serializer.Write(this.base_x_cmd);
            serializer.Write(this.base_theta_cmd);
            serializer.Write(this.base_reset_odom_cmd);
            serializer.Write(this.pan_cmd);
            serializer.Write(this.tilt_cmd);
            serializer.Write(this.ee_x_cmd);
            serializer.Write(this.ee_y_cmd);
            serializer.Write(this.ee_z_cmd);
            serializer.Write(this.ee_roll_cmd);
            serializer.Write(this.ee_pitch_cmd);
            serializer.Write(this.waist_cmd);
            serializer.Write(this.gripper_cmd);
            serializer.Write(this.pose_cmd);
            serializer.Write(this.speed_cmd);
            serializer.Write(this.speed_toggle_cmd);
            serializer.Write(this.gripper_pwm_cmd);
        }

        public override string ToString()
        {
            return "TelecobotUnityControlMsg: " +
            "\nreboot_cmd: " + reboot_cmd.ToString() +
            "\nmotor_cmd: " + motor_cmd.ToString() +
            "\narm_cmd: " + arm_cmd.ToString() +
            "\nbase_cmd: " + base_cmd.ToString() +
            "\npose_data: " + System.String.Join(", ", pose_data.ToList()) +
            "\nend_effector_pose: " + end_effector_pose.ToString() +
            "\ngoal_pose: " + goal_pose.ToString() +
            "\nbase_x_cmd: " + base_x_cmd.ToString() +
            "\nbase_theta_cmd: " + base_theta_cmd.ToString() +
            "\nbase_reset_odom_cmd: " + base_reset_odom_cmd.ToString() +
            "\npan_cmd: " + pan_cmd.ToString() +
            "\ntilt_cmd: " + tilt_cmd.ToString() +
            "\nee_x_cmd: " + ee_x_cmd.ToString() +
            "\nee_y_cmd: " + ee_y_cmd.ToString() +
            "\nee_z_cmd: " + ee_z_cmd.ToString() +
            "\nee_roll_cmd: " + ee_roll_cmd.ToString() +
            "\nee_pitch_cmd: " + ee_pitch_cmd.ToString() +
            "\nwaist_cmd: " + waist_cmd.ToString() +
            "\ngripper_cmd: " + gripper_cmd.ToString() +
            "\npose_cmd: " + pose_cmd.ToString() +
            "\nspeed_cmd: " + speed_cmd.ToString() +
            "\nspeed_toggle_cmd: " + speed_toggle_cmd.ToString() +
            "\ngripper_pwm_cmd: " + gripper_pwm_cmd.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
