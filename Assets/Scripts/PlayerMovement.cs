using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Transform player;
    public float movementSpeed;

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.position;
        if (Input.GetKey("w"))
        {
            Debug.Log("move forward!");
            playerPosition.z += (float) 0.5 * movementSpeed * Time.deltaTime;
            player.position = playerPosition;
        }
        if (Input.GetKey("s"))
        {
            Debug.Log("move backward!");
            playerPosition.z -= (float)0.5 * movementSpeed * Time.deltaTime;
            player.position = playerPosition;
        }
        if (Input.GetKey("a"))
        {
            Debug.Log("move left!");
            playerPosition.x -= (float)0.5 * movementSpeed * Time.deltaTime;
            player.position = playerPosition;
        }
        if (Input.GetKey("d"))
        {
            Debug.Log("move right!");
            playerPosition.x += (float)0.5 * movementSpeed * Time.deltaTime;
            player.position = playerPosition;
        }
    }
}
