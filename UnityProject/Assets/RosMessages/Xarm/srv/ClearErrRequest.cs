//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Xarm
{
    [Serializable]
    public class ClearErrRequest : Message
    {
        public const string k_RosMessageName = "xarm_msgs/ClearErr";
        public override string RosMessageName => k_RosMessageName;


        public ClearErrRequest()
        {
        }
        public static ClearErrRequest Deserialize(MessageDeserializer deserializer) => new ClearErrRequest(deserializer);

        private ClearErrRequest(MessageDeserializer deserializer)
        {
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
        }

        public override string ToString()
        {
            return "ClearErrRequest: ";
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
