using System;
using Unity.Robotics;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Telecobot.Control
{
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

        public float dampingBase = 10;
        public float forceLimitBase = 10;


        void Start()
        {
            this.gameObject.AddComponent<FKRobot>();
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10;
            foreach (ArticulationBody joint in articulationChain)
            {
                ArticulationDrive drive = joint.xDrive;
                if (joint.name == "/left_wheel" || joint.name == "/right_wheel")
                {
                    joint.linearDamping = 0.05f;
                    drive.damping = dampingBase;
                    joint.linearDamping = 50;
                    joint.angularDamping = 10;
                    //drive.forceLimit = forceLimitBase;
                }
                else if (joint.name == "/pan_link" || joint.name == "/tilt_link" || joint.name == "/right_finger_link" || joint.name == "/left_finger_link") 
                {
                    drive.stiffness = stiffness;
                    drive.damping = damping;

                    joint.jointFriction = defDyanmicVal;
                    joint.angularDamping = defDyanmicVal;
                    drive.forceLimit = forceLimit;
                }
                else if (joint.name == "front_caster")
                {
                    return;
                }
                else
                {
                    drive.stiffness = stiffness;
                    drive.damping = damping;
                    joint.jointFriction = defDyanmicVal;
                    joint.angularDamping = defDyanmicVal;
                }

                joint.xDrive = drive;
            }
        }
    }
}
