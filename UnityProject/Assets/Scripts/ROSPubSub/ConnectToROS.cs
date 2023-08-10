using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

/// <summary>
/// connect to ros endpoint 
/// </summary>
public class ConnectToROS : MonoBehaviour
{

    private ROSConnection ros;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
