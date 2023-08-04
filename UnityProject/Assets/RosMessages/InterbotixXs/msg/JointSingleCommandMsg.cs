//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.InterbotixXs
{
    [Serializable]
    public class JointSingleCommandMsg : Message
    {
        public const string k_RosMessageName = "interbotix_xs_msgs/JointSingleCommand";
        public override string RosMessageName => k_RosMessageName;

        //  Command a desired joint. Note that the command is processed differently based on the joint's operating mode.
        //  For example, if a joint's operating mode is set to 'position', the command is interpreted as a position in radians
        public string name;
        //  Name of joint
        public float cmd;
        //  Joint command

        public JointSingleCommandMsg()
        {
            this.name = "";
            this.cmd = 0.0f;
        }

        public JointSingleCommandMsg(string name, float cmd)
        {
            this.name = name;
            this.cmd = cmd;
        }

        public static JointSingleCommandMsg Deserialize(MessageDeserializer deserializer) => new JointSingleCommandMsg(deserializer);

        private JointSingleCommandMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.name);
            deserializer.Read(out this.cmd);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.name);
            serializer.Write(this.cmd);
        }

        public override string ToString()
        {
            return "JointSingleCommandMsg: " +
            "\nname: " + name.ToString() +
            "\ncmd: " + cmd.ToString();
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