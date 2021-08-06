using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryMovement : MonoBehaviour
{
    public Transform target;
    public GameObject UAV;

    // Update is called once per frame
    void Update()
    {
        SHARKSProtocol sharksProtocolScript = UAV.GetComponent<SHARKSProtocol>();
        float diameter = sharksProtocolScript.delta * 2;
        transform.localScale = new Vector3(diameter, diameter, diameter);
        
        transform.position = target.position;
    }
}
