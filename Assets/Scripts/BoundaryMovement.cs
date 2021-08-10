using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryMovement : MonoBehaviour
{
    public Transform target;
    public GameObject UAV;

    private void Start()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;

        float diameter = UAV.GetComponent<SHARKSProtocol>().delta * 2;
        transform.localScale = new Vector3(diameter, diameter, diameter);   
    }
}
