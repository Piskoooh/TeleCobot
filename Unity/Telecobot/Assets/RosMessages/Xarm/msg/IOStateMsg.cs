//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Xarm
{
    [Serializable]
    public class IOStateMsg : Message
    {
        public const string k_RosMessageName = "xarm_msgs/IOState";
        public override string RosMessageName => k_RosMessageName;

        //  for indicating 2 digital and 2 analog Input port state
        public int digital_1;
        public int digital_2;
        public float analog_1;
        public float analog_2;

        public IOStateMsg()
        {
            this.digital_1 = 0;
            this.digital_2 = 0;
            this.analog_1 = 0.0f;
            this.analog_2 = 0.0f;
        }

        public IOStateMsg(int digital_1, int digital_2, float analog_1, float analog_2)
        {
            this.digital_1 = digital_1;
            this.digital_2 = digital_2;
            this.analog_1 = analog_1;
            this.analog_2 = analog_2;
        }

        public static IOStateMsg Deserialize(MessageDeserializer deserializer) => new IOStateMsg(deserializer);

        private IOStateMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.digital_1);
            deserializer.Read(out this.digital_2);
            deserializer.Read(out this.analog_1);
            deserializer.Read(out this.analog_2);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.digital_1);
            serializer.Write(this.digital_2);
            serializer.Write(this.analog_1);
            serializer.Write(this.analog_2);
        }

        public override string ToString()
        {
            return "IOStateMsg: " +
            "\ndigital_1: " + digital_1.ToString() +
            "\ndigital_2: " + digital_2.ToString() +
            "\nanalog_1: " + analog_1.ToString() +
            "\nanalog_2: " + analog_2.ToString();
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