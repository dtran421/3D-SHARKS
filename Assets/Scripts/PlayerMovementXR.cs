using UnityEngine;

public class PlayerMovementXR : MonoBehaviour
{

    public Transform player;
    public float movementDistance;
    public float movementSpeed;
    public float rotationAngle;
    public float rotationSpeed;

    private bool controllingPlayer = true;

    // Update is called once per frame
    void Update()
    {
        if (!controllingPlayer)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
            {
                controllingPlayer = true;
            }
        }
        if (controllingPlayer)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
            {
                controllingPlayer = false;
            }

            Vector3 playerPosition = player.position;
            // move along internal z-axis
            float forwardAcceleration = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            // Debug.Log("move forward!");
            playerPosition += transform.forward * forwardAcceleration * movementDistance * movementSpeed * Time.deltaTime;

            float backwardAcceleration = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
            // Debug.Log("move backward!");
            playerPosition -= transform.forward * backwardAcceleration * movementDistance * movementSpeed * Time.deltaTime;

            // rotate around y-axis (yaw)
            Vector2 yawAndPitch = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            // Debug.Log("rotate left/right!");
            transform.RotateAround(playerPosition, transform.up, yawAndPitch.x * rotationAngle * rotationSpeed * Time.deltaTime);

            // rotate around x-axis (pitch)
            // Debug.Log("rotate (tilt) up/down!");
            transform.RotateAround(playerPosition, transform.right, -yawAndPitch.y * rotationAngle * rotationSpeed * Time.deltaTime);

            // rotate around z-axis (roll)
            Vector2 roll = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            // Debug.Log("rotate (roll) left/right!");
            transform.RotateAround(playerPosition, transform.forward, -roll.x * rotationAngle * rotationSpeed * Time.deltaTime);

            player.position = playerPosition;
        }
    }
}
