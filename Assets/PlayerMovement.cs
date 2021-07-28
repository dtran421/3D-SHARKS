using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Transform player;

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.position;
          if (Input.GetKey("w"))
        {
            Debug.Log("move forward!");
            playerPosition.y += 1;
            player.position = playerPosition;
            Debug.Log(transform.position);
        }
        if (Input.GetKey("s"))
        {
            Debug.Log("move backward!");
            playerPosition.y -= 1;
            player.position = playerPosition;
            Debug.Log(transform.position);
        }
    }
}
