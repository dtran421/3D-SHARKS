using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHARKSProtocol : MonoBehaviour
{
    public Transform self;
    public Transform target;
    public float distanceToMove;
    public float delta;
    public float epsilon;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        Vector3 selfPos = transform.position;
        Vector3 targetPos = target.position;

        Vector3 distToTarget = new Vector3(targetPos.x - selfPos.x, targetPos.y - selfPos.y, targetPos.z - selfPos.z);
        float absoluteDistToTarget = CalculateDistance(distToTarget);

        CenterRule(selfPos, distToTarget, absoluteDistToTarget);
    }

    float CalculateDistance(Vector3 distToTarget)
    {
        return Mathf.Sqrt(Mathf.Pow(distToTarget.x, 2) + Mathf.Pow(distToTarget.y, 2) + Mathf.Pow(distToTarget.z, 2));
    }

    void CenterRule(Vector3 selfPos, Vector3 distVector, float dist)
    {
        if (delta - dist > epsilon)
        {
            // add collision detection
            // move backwards
            Debug.Log("move backwards");
            selfPos.x += (distVector.x > 0 ? -1 : 1) * distanceToMove * Mathf.Cos(self.rotation.y) * Time.deltaTime;
            selfPos.y += (distVector.y > 0 ? -1 : 1) * distanceToMove * Mathf.Cos(self.rotation.z) * Time.deltaTime;
            selfPos.z += (distVector.z > 0 ? -1 : 1) * distanceToMove * Mathf.Cos(self.rotation.x) * Time.deltaTime;
        }
        if (delta - dist < -epsilon)
        {
            // add collision detection
            // move forwards
            Debug.Log("move forwards");
            selfPos.x += (distVector.x > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.y) * Time.deltaTime;
            selfPos.y += (distVector.y > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.z) * Time.deltaTime;
            selfPos.z += (distVector.z > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.x) * Time.deltaTime;
        }
        transform.position = selfPos;
    }
}
