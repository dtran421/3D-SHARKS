using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Transform player;
    public float movementDistance;
    public float movementSpeed;
    public float rotationAngle;
    public float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.position;
        // move along z-axis
        if (Input.GetKey("w"))
        {
            Debug.Log("move forward!");
            playerPosition.z += movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            Debug.Log("move backward!");
            playerPosition.z -= movementDistance * movementSpeed * Time.deltaTime;
        }
        // move along x-axis
        if (Input.GetKey("a"))
        {
            Debug.Log("move left!");
            playerPosition.x -= movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            Debug.Log("move right!");
            playerPosition.x += movementDistance * movementSpeed * Time.deltaTime;
        }
        // move along y-axis
        if (Input.GetKey("space")) {
            Debug.Log("move up!");
            playerPosition.y += movementDistance * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("left shift") || Input.GetKey("right shift")) {
            Debug.Log("move down!"); 
            playerPosition.y -= movementDistance * movementSpeed * Time.deltaTime;
        }
        // rotate around y-axis (yaw)
        if (Input.GetKey("q"))
        {
            Debug.Log("rotate left!");
            transform.RotateAround(playerPosition, transform.up, -rotationAngle * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey("e"))
        {
            Debug.Log("rotate right!");
            transform.RotateAround(playerPosition, transform.up, rotationAngle * rotationSpeed * Time.deltaTime);
        }
        // rotate around z-axis (roll)
        if (Input.GetKey("x"))
        {
            Debug.Log("rotate (roll) left!");
            transform.RotateAround(playerPosition, transform.forward, rotationAngle * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey("c"))
        {
            Debug.Log("rotate (roll) right!");
            transform.RotateAround(playerPosition, transform.forward, -rotationAngle * rotationSpeed * Time.deltaTime);
        }
        // rotate around x-axis (pitch)
        if (Input.GetKey("r"))
        {
            Debug.Log("rotate (tilt) up!");
            transform.RotateAround(playerPosition, transform.right, -rotationAngle * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey("f"))
        {
            Debug.Log("rotate (tilt) down!");
            transform.RotateAround(playerPosition, transform.right, rotationAngle * rotationSpeed * Time.deltaTime);
        }
        player.position = playerPosition;
    }
}
