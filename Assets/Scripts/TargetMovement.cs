using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    public Transform target;

    public float movementDistance;
    public float movementSpeed;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.position;
        if (Input.GetKey("return"))
        {
            targetPos.z += movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("right shift"))
        {
            targetPos.z -= movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("up"))
        {
            targetPos.y += movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("down"))
        {
            targetPos.y -= movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("left"))
        {
            targetPos.x -= movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("right"))
        {
            targetPos.x += movementDistance * movementSpeed * Time.deltaTime;
        }
        transform.position = targetPos;
    }
}
