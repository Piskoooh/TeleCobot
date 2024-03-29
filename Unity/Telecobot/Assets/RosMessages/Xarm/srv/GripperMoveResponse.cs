//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Xarm
{
    [Serializable]
    public class GripperMoveResponse : Message
    {
        public const string k_RosMessageName = "xarm_msgs/GripperMove";
        public override string RosMessageName => k_RosMessageName;

        public short ret;
        public string message;

        public GripperMoveResponse()
        {
            this.ret = 0;
            this.message = "";
        }

        public GripperMoveResponse(short ret, string message)
        {
            this.ret = ret;
            this.message = message;
        }

        public static GripperMoveResponse Deserialize(MessageDeserializer deserializer) => new GripperMoveResponse(deserializer);

        private GripperMoveResponse(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.ret);
            deserializer.Read(out this.message);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.ret);
            serializer.Write(this.message);
        }

        public override string ToString()
        {
            return "GripperMoveResponse: " +
            "\nret: " + ret.ToString() +
            "\nmessage: " + message.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Response);
        }
    }
}
