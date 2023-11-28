//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.InterbotixXs
{
    [Serializable]
    public class RobotInfoRequest : Message
    {
        public const string k_RosMessageName = "interbotix_xs_msgs/RobotInfo";
        public override string RosMessageName => k_RosMessageName;

        //  Get robot information
        // 
        //  Note that if a 'gripper' joint is specified, all information will be specified in terms of the 'left_finger' joint
        public string cmd_type;
        //  set to 'group' if requesting info about a joint group or 'single' if requesting info about a single joint
        public string name;
        //  the group name if requesting info about a group or the joint name if requesting info about a single joint

        public RobotInfoRequest()
        {
            this.cmd_type = "";
            this.name = "";
        }

        public RobotInfoRequest(string cmd_type, string name)
        {
            this.cmd_type = cmd_type;
            this.name = name;
        }

        public static RobotInfoRequest Deserialize(MessageDeserializer deserializer) => new RobotInfoRequest(deserializer);

        private RobotInfoRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.cmd_type);
            deserializer.Read(out this.name);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.cmd_type);
            serializer.Write(this.name);
        }

        public override string ToString()
        {
            return "RobotInfoRequest: " +
            "\ncmd_type: " + cmd_type.ToString() +
            "\nname: " + name.ToString();
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
