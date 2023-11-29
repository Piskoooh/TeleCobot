//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Xarm
{
    [Serializable]
    public class SetAxisRequest : Message
    {
        public const string k_RosMessageName = "xarm_msgs/SetAxis";
        public override string RosMessageName => k_RosMessageName;

        //  request: for enabling / disabling motion control of one(or all) joint.
        //  id: 1-7 for target joint number, or 8 for all joints
        //  data: 0 for disabling servo control, 1 for enabling servo control.
        public short id;
        public short data;

        public SetAxisRequest()
        {
            this.id = 0;
            this.data = 0;
        }

        public SetAxisRequest(short id, short data)
        {
            this.id = id;
            this.data = data;
        }

        public static SetAxisRequest Deserialize(MessageDeserializer deserializer) => new SetAxisRequest(deserializer);

        private SetAxisRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.id);
            deserializer.Read(out this.data);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.id);
            serializer.Write(this.data);
        }

        public override string ToString()
        {
            return "SetAxisRequest: " +
            "\nid: " + id.ToString() +
            "\ndata: " + data.ToString();
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