//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.InterbotixXs
{
    [Serializable]
    public class OperatingModesRequest : Message
    {
        public const string k_RosMessageName = "interbotix_xs_msgs/OperatingModes";
        public override string RosMessageName => k_RosMessageName;

        //  Set Operating Modes
        // 
        //  To get familiar with the various operating modes, go to...
        //  http://emanual.robotis.com/docs/en/software/dynamixel/dynamixel_workbench/
        //  ...click on a motor model, and scroll down to the 'Operating Mode' section.
        // 
        //  There are 6 valid operating modes. They are...
        //    "position" - allows up to 1 complete joint revolution (perfect for arm joints); units are in radians
        //    "ext_position" - allows up to 512 joint revolutions; units are in radians
        //    "velocity" - allows infinite number of rotations (perfect for wheeled robots); units are in rad/s
        //    "current" - allows infinite number of rotations (perfect for grippers); units are in milliamps
        //    "current_based_position" - allows up to 512 joint revolutions; units are in radians
        //    "pwm" - allows infinite number of rotations (perfect for grippers); units are in PWM
        // 
        //  Note that the interbotix_xs_sdk offers one other 'pseudo' operating mode that can be useful in controlling Interbotix Grippers - called "linear_position".
        //  Behind the scenes, it uses the "position" operating mode mentioned above. The main difference is that with this mode, a desired linear distance [m]
        //  between the two gripper fingers can be commanded. In the "position" mode though, only the angular position of the motor can be commanded.
        // 
        //  There are 2 valid profile types - either 'time' or 'velocity'. Depending on which is chosen, the following parameters behave differently.
        // 
        //  1) profile_velocity: acts as a pass-through to the Profile_Velocity register and operates in one of two ways. If
        //     'profile_type' is set to 'velocity', this parameter describes the max velocity limit for the specified joint(s);
        //     for example, if doing 'position' control, setting this to '131' would be equivalent to a limit of 3.14 rad/s; if
        //     'profile_type' is set to 'time', this parameter sets the time span (in milliseconds) that it should take for the
        //     specified joint(s) to move; to have an 'infinite' max limit, set to '0'.
        // 
        //  2) profile_acceleration: acts as a pass-through to the Profile_Acceleration register and operates in one of two ways. If
        //     'profile_type' is set to 'velocity', this parameter describes the max acceleration limit for the specified joint(s);
        //     for example, if doing 'position' or 'velocity' control, setting this to '15' would be equivalent to a limit of 5.6 rad/s^2;
        //     if 'profile_type' is set to 'time', this parameter sets the time span (in milliseconds) that it should take for the
        //     specified joint(s) to accelerate; to have an 'infinite' max limit, set to '0'.
        public string cmd_type;
        //  set to 'group' if commanding a joint group or 'single' if commanding a single joint
        public string name;
        //  name of the group if commanding a joint group or joint if commanding a single joint
        public string mode;
        //  desired operating mode as described above
        public string profile_type;
        //  desired 'profile' type - either 'time' or 'velocity' as described above
        public int profile_velocity;
        //  desired velocity profile as explained above - only used in 'position' or the 'ext_position' control modes
        public int profile_acceleration;
        //  desired acceleration profile as explained above - used in all modes except for 'current' and 'pwm' control

        public OperatingModesRequest()
        {
            this.cmd_type = "";
            this.name = "";
            this.mode = "";
            this.profile_type = "";
            this.profile_velocity = 0;
            this.profile_acceleration = 0;
        }

        public OperatingModesRequest(string cmd_type, string name, string mode, string profile_type, int profile_velocity, int profile_acceleration)
        {
            this.cmd_type = cmd_type;
            this.name = name;
            this.mode = mode;
            this.profile_type = profile_type;
            this.profile_velocity = profile_velocity;
            this.profile_acceleration = profile_acceleration;
        }

        public static OperatingModesRequest Deserialize(MessageDeserializer deserializer) => new OperatingModesRequest(deserializer);

        private OperatingModesRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.cmd_type);
            deserializer.Read(out this.name);
            deserializer.Read(out this.mode);
            deserializer.Read(out this.profile_type);
            deserializer.Read(out this.profile_velocity);
            deserializer.Read(out this.profile_acceleration);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.cmd_type);
            serializer.Write(this.name);
            serializer.Write(this.mode);
            serializer.Write(this.profile_type);
            serializer.Write(this.profile_velocity);
            serializer.Write(this.profile_acceleration);
        }

        public override string ToString()
        {
            return "OperatingModesRequest: " +
            "\ncmd_type: " + cmd_type.ToString() +
            "\nname: " + name.ToString() +
            "\nmode: " + mode.ToString() +
            "\nprofile_type: " + profile_type.ToString() +
            "\nprofile_velocity: " + profile_velocity.ToString() +
            "\nprofile_acceleration: " + profile_acceleration.ToString();
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
