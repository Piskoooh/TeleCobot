using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalArrow : MonoBehaviour
{

    SceneMaster sceneMaster;
    [SerializeField]
    Transform rightFinger, leftFinger, eeFinger, eeGripper;
    [HideInInspector]
    public Vector3 defL_rN,curL_rN, defEef_egN,curEef_egN, defEeUpN,curEeUpN;


    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateEeArrow()
    {
        curL_rN = (rightFinger.position - leftFinger.position).normalized;
        curEef_egN = (eeFinger.position - eeGripper.position).normalized;
        curEeUpN = Vector3.Cross(curEef_egN, curL_rN).normalized;
    }

    private void OnEnable()
    {
        sceneMaster = GameObject.FindGameObjectWithTag("SceneMaster").GetComponent<SceneMaster>();
        sceneMaster.inputMng.localArrow = this;

        defL_rN = (rightFinger.position - leftFinger.position).normalized;
        defEef_egN = (eeFinger.position - eeGripper.position).normalized;
        defEeUpN = Vector3.Cross(defEef_egN, defL_rN).normalized;
        curL_rN = (rightFinger.position - leftFinger.position).normalized;
        curEef_egN = (eeFinger.position - eeGripper.position).normalized;
        curEeUpN = Vector3.Cross(curEef_egN, curL_rN).normalized;
    }
}

