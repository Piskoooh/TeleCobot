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
        // Stores original colors of the part being highlighted
        private Color[] prevColor;
        private int previousIndex;

        [InspectorReadOnly(hideInEditMode: true)]
        public string selectedJoint;
        [HideInInspector]
        public int selectedIndex;

        public ControlType control = ControlType.PositionControl;
        public float stiffness=10000f;
        public float damping=100f;
        public float forceLimit=1000f;
        public float speed = 5f; // Units: degree/s
        public float torque = 100f; // Units: Nm or N
        public float acceleration = 5f;// Units: m/s^2 / degree/s^2

        [Tooltip("Color to highlight the currently selected join")]
        public Color highLightColor = new Color(1.0f, 0, 0, 1.0f);


        private ArticulationBody wA1;
        private ArticulationBody wA2;

        public float maxLinearSpeed = 2; //  m/s
        public float maxRotationalSpeed = 1;//
        public float wheelRadius = 0.033f; //meters
        public float trackWidth = 0.288f; // meters Distance between tyres
        public float forceLimitBase = 10;
        public float dampingBase = 10;

        public float ROSTimeout = 0.5f;
        private float lastCmdReceived = 0f;

        void Start()
        {
            previousIndex = selectedIndex = 1;
            this.gameObject.AddComponent<FKRobot>();
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10;
            foreach (ArticulationBody joint in articulationChain)
            {
                //次回用メモ：wheelは条件分岐でWheelControlを作成する。
                //WheelControlスクリプトを作成する。内容はAGVControlの必要な部分を切り取る。
                //ROSConnectionは別で実行？JointControlはSubJointStateでUpdeteされてる？

                joint.gameObject.AddComponent<JointControl>();
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
                ArticulationDrive currentDrive = joint.xDrive;
                currentDrive.forceLimit = forceLimit;
                joint.xDrive = currentDrive;

                //wA1 = wheel1.GetComponent<ArticulationBody>();
                //wA2 = wheel2.GetComponent<ArticulationBody>();
                //SetParameters(wA1);
                //SetParameters(wA2);
                //ros = ROSConnection.GetOrCreateInstance();
                //ros.Subscribe<TwistMsg>("cmd_vel", ReceiveROSCmd);
            }
        }


        //void ReceiveROSCmd(TwistMsg cmdVel)
        //{
        //    rosLinear = (float)cmdVel.linear.x;
        //    rosAngular = (float)cmdVel.angular.z;
        //    lastCmdReceived = Time.time;
        //}

        //private void SetParameters(ArticulationBody joint)
        //{
        //    ArticulationDrive drive = joint.xDrive;
        //    drive.forceLimit = forceLimitBase;
        //    drive.damping = dampingBase;
        //    joint.xDrive = drive;
        //}

        //private void SetSpeed(ArticulationBody joint, float wheelSpeed = float.NaN)
        //{
        //    ArticulationDrive drive = joint.xDrive;
        //    if (float.IsNaN(wheelSpeed))
        //    {
        //        drive.targetVelocity = ((2 * maxLinearSpeed) / wheelRadius) * Mathf.Rad2Deg * (int)direction;
        //    }
        //    else
        //    {
        //        drive.targetVelocity = wheelSpeed;
        //    }
        //    joint.xDrive = drive;
        //}

        //private void ROSUpdate()
        //{
        //    if (Time.time - lastCmdReceived > ROSTimeout)
        //    {
        //        rosLinear = 0f;
        //        rosAngular = 0f;
        //    }
        //    RobotInput(rosLinear, -rosAngular);
        //}

        //private void RobotInput(float speed, float rotSpeed) // m/s and rad/s
        //{
        //    if (speed > maxLinearSpeed)
        //    {
        //        speed = maxLinearSpeed;
        //    }
        //    if (rotSpeed > maxRotationalSpeed)
        //    {
        //        rotSpeed = maxRotationalSpeed;
        //    }
        //    float wheel1Rotation = (speed / wheelRadius);
        //    float wheel2Rotation = wheel1Rotation;
        //    float wheelSpeedDiff = ((rotSpeed * trackWidth) / wheelRadius);
        //    if (rotSpeed != 0)
        //    {
        //        wheel1Rotation = (wheel1Rotation + (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
        //        wheel2Rotation = (wheel2Rotation - (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
        //    }
        //    else
        //    {
        //        wheel1Rotation *= Mathf.Rad2Deg;
        //        wheel2Rotation *= Mathf.Rad2Deg;
        //    }
        //    SetSpeed(wA1, wheel1Rotation);
        //    SetSpeed(wA2, wheel2Rotation);
        //}
    
    public void UpdateControlType(JointControl joint)
        {
            joint.controltype = control;
            if (control == ControlType.PositionControl)
            {
                ArticulationDrive drive = joint.joint.xDrive;
                drive.stiffness = stiffness;
                drive.damping = damping;
                joint.joint.xDrive = drive;
            }
        }

        public void OnGUI()
        {
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 200, 10, 400, 20), "Press left/right arrow keys to select a robot joint.", centeredStyle);
            GUI.Label(new Rect(Screen.width / 2 - 200, 30, 400, 20), "Press up/down arrow keys to move " + selectedJoint + ".", centeredStyle);
        }
    }
}
