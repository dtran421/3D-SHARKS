using UnityEngine;

public class TargetMovementXR : MonoBehaviour
{
    public Transform target;

    public float movementDistance;
    public float movementSpeed;

    private bool controllingTarget = false;

    // Update is called once per frame
    void Update()
    {
        if (!controllingTarget)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
            {
                controllingTarget = true;
            }
        }
        if (controllingTarget)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
            {
                controllingTarget = false;
            }

            Vector3 targetPos = target.position;
            Vector2 XAndY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            targetPos.x += XAndY.x * movementDistance * movementSpeed * Time.deltaTime;
            targetPos.y += XAndY.y * movementDistance * movementSpeed * Time.deltaTime;

            Vector2 Z = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            targetPos.z += Z.y * movementDistance * movementSpeed * Time.deltaTime;

            transform.position = targetPos;
        }
    }
}
