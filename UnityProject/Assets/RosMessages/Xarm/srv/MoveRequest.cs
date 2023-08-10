//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Xarm
{
    [Serializable]
    public class MoveRequest : Message
    {
        public const string k_RosMessageName = "xarm_msgs/Move";
        public override string RosMessageName => k_RosMessageName;

        //  request: command specification for motion executions.
        //  Units:
        // 	joint space/angles: radian, radian/s and radian/s^2.
        // 	Cartesian space: mm, mm/s, and mm/s^2.
        // 	time: sec
        //  pose： target coordinate. 
        // 	For Joint Space target，pose dimention is the number of joints. element as each target joint position.
        // 	For Cartesian target: pose dimention is 6 for (x, y, z, roll, pitch, yaw)
        public float[] pose;
        //  mvvelo: specified maximum velocity during execution. linear or angular velocity 
        public float mvvelo;
        //  mvacc: specified maximum acceleration during execution. linear or angular acceleration.
        public float mvacc;
        //  mvtime: currently do not have any special meaning, please just give it 0. 
        //  PLEASE NOTE: after firmware version 1.5, For servo_cartesian motion, mvtime will be used as indicator of coordinate system. (0 for BASE coordinate, non-zero for TOOL coordinate)  
        public float mvtime;
        //  mvradii: this is special for move_ineb service, meaning the blending radius between 2 straight path trajectories, 0 for no blend.
        public float mvradii;

        public MoveRequest()
        {
            this.pose = new float[0];
            this.mvvelo = 0.0f;
            this.mvacc = 0.0f;
            this.mvtime = 0.0f;
            this.mvradii = 0.0f;
        }

        public MoveRequest(float[] pose, float mvvelo, float mvacc, float mvtime, float mvradii)
        {
            this.pose = pose;
            this.mvvelo = mvvelo;
            this.mvacc = mvacc;
            this.mvtime = mvtime;
            this.mvradii = mvradii;
        }

        public static MoveRequest Deserialize(MessageDeserializer deserializer) => new MoveRequest(deserializer);

        private MoveRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.pose, sizeof(float), deserializer.ReadLength());
            deserializer.Read(out this.mvvelo);
            deserializer.Read(out this.mvacc);
            deserializer.Read(out this.mvtime);
            deserializer.Read(out this.mvradii);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.WriteLength(this.pose);
            serializer.Write(this.pose);
            serializer.Write(this.mvvelo);
            serializer.Write(this.mvacc);
            serializer.Write(this.mvtime);
            serializer.Write(this.mvradii);
        }

        public override string ToString()
        {
            return "MoveRequest: " +
            "\npose: " + System.String.Join(", ", pose.ToList()) +
            "\nmvvelo: " + mvvelo.ToString() +
            "\nmvacc: " + mvacc.ToString() +
            "\nmvtime: " + mvtime.ToString() +
            "\nmvradii: " + mvradii.ToString();
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
