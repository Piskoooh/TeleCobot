//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Xarm
{
    [Serializable]
    public class GetErrResponse : Message
    {
        public const string k_RosMessageName = "xarm_msgs/GetErr";
        public override string RosMessageName => k_RosMessageName;

        public short err;
        public string message;

        public GetErrResponse()
        {
            this.err = 0;
            this.message = "";
        }

        public GetErrResponse(short err, string message)
        {
            this.err = err;
            this.message = message;
        }

        public static GetErrResponse Deserialize(MessageDeserializer deserializer) => new GetErrResponse(deserializer);

        private GetErrResponse(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.err);
            deserializer.Read(out this.message);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.err);
            serializer.Write(this.message);
        }

        public override string ToString()
        {
            return "GetErrResponse: " +
            "\nerr: " + err.ToString() +
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
