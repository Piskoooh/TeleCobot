using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalArrow : MonoBehaviour
{

    UIManager uIMng;
    [SerializeField]
    Transform rightFinger, leftFinger, eeFinger, eeGripper;
    [HideInInspector]
    public Vector3 l_rN, ef_egN;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateEeArrow()
    {
        l_rN = (rightFinger.position - leftFinger.position).normalized;
        ef_egN = (eeFinger.position - eeGripper.position).normalized;
    }

    private void OnEnable()
    {
        uIMng = GameObject.Find("UIManager").GetComponent<UIManager>();
        uIMng.localArrow = this;
    }
}
