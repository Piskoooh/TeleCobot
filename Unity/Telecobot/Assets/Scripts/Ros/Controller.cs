using System;
using Unity.Robotics;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Telecobot.Control
{
    //JointStateをSubした際に必要なArticulationBodyの係数を設定する。
    //ロボットの各ジョイント(関節)の剛性などを決定
    //関の修論では使用は廃止
    public class Controller : MonoBehaviour
    {
        private ArticulationBody[] articulationChain;

        public ControlType control = ControlType.PositionControl;
        public float stiffness = 10000f;
        public float damping = 100f;
        public float forceLimit = 1000f;
        public float speed = 5f; // Units: degree/s
        public float torque = 100f; // Units: Nm or N
        public float acceleration = 5f;// Units: m/s^2 / degree/s^2


        void Start()
        {
            this.gameObject.AddComponent<FKRobot>();
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10;
            foreach (ArticulationBody joint in articulationChain)
            {
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
                ArticulationDrive drive = joint.xDrive;
                drive.forceLimit = forceLimit;
                drive.stiffness = stiffness;
                drive.damping = damping;
                joint.xDrive = drive;
            }
        }
    }
}
