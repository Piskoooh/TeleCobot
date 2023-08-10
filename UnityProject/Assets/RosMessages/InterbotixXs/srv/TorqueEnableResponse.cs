//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.InterbotixXs
{
    [Serializable]
    public class TorqueEnableResponse : Message
    {
        public const string k_RosMessageName = "interbotix_xs_msgs/TorqueEnable";
        public override string RosMessageName => k_RosMessageName;


        public TorqueEnableResponse()
        {
        }
        public static TorqueEnableResponse Deserialize(MessageDeserializer deserializer) => new TorqueEnableResponse(deserializer);

        private TorqueEnableResponse(MessageDeserializer deserializer)
        {
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
        }

        public override string ToString()
        {
            return "TorqueEnableResponse: ";
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
