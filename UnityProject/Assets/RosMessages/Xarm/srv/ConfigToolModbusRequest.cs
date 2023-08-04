//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Xarm
{
    [Serializable]
    public class ConfigToolModbusRequest : Message
    {
        public const string k_RosMessageName = "xarm_msgs/ConfigToolModbus";
        public override string RosMessageName => k_RosMessageName;

        //  configure the tool modbus communication baud rate, in bps:
        public int baud_rate;
        //  configure the timeout parameter in modbus communication, in milliseconds
        public int timeout_ms;

        public ConfigToolModbusRequest()
        {
            this.baud_rate = 0;
            this.timeout_ms = 0;
        }

        public ConfigToolModbusRequest(int baud_rate, int timeout_ms)
        {
            this.baud_rate = baud_rate;
            this.timeout_ms = timeout_ms;
        }

        public static ConfigToolModbusRequest Deserialize(MessageDeserializer deserializer) => new ConfigToolModbusRequest(deserializer);

        private ConfigToolModbusRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.baud_rate);
            deserializer.Read(out this.timeout_ms);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.baud_rate);
            serializer.Write(this.timeout_ms);
        }

        public override string ToString()
        {
            return "ConfigToolModbusRequest: " +
            "\nbaud_rate: " + baud_rate.ToString() +
            "\ntimeout_ms: " + timeout_ms.ToString();
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