using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class SHARKSProtocol : MonoBehaviour
{
    public Transform self;
    public Transform target;

    public float centerRuleDistance;
    public float dispersionRuleDistance;
    public float dispersionRotation;
    public float dispersionTilt;
    public float delta;
    public float epsilon;

    // Update is called once per frame
    void Update()
    {
        Vector3 selfPos = transform.position;
        Vector3 targetPos = target.position;

        Vector3 distToTarget = new Vector3(targetPos.x - selfPos.x, targetPos.y - selfPos.y, targetPos.z - selfPos.z);
        float absoluteDistToTarget = Vector3.Distance(targetPos, selfPos);
        // Debug.Log(absoluteDistToTarget);
        
        CenterRule(selfPos, distToTarget, absoluteDistToTarget);
        selfPos = transform.position;
        DispersionRule(selfPos);
    }

    void CenterRule(Vector3 selfPos, Vector3 distVector, float dist)
    {
        /* change for 3d */
        transform.LookAt(target, transform.up);

        bool shouldMove = false;
        // check if UAV should move backwards
        if (delta - dist > epsilon)
        {
            // Debug.Log("move backwards");
            selfPos.x += centerRuleDistance * -transform.forward.x * Time.deltaTime;
            selfPos.y += centerRuleDistance * -transform.forward.y * Time.deltaTime;
            // selfPos.z += (distVector.z > 0 ? -1 : 1) * centerRuleDistance * Mathf.Cos(self.rotation.x) * Time.deltaTime;

            // collision detection
            Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z);
            if (hitColliders.Length <= 1)
            {
                shouldMove = true;
            }
        }
        // check if UAV should move forwards
        if (delta - dist < -epsilon)
        {
            // Debug.Log("move forwards");
            selfPos.x += centerRuleDistance * transform.forward.x * Time.deltaTime;
            selfPos.y += centerRuleDistance * transform.forward.y * Time.deltaTime;
            // selfPos.z += (distVector.z > 0 ? 1 : -1) * centerRuleDistance * Mathf.Cos(self.rotation.x) * Time.deltaTime;
            // Debug.Log("test 1 " + self.localEulerAngles.x);
            // Debug.Log("center " + (distVector.x > 0 ? 1 : -1) * centerRuleDistance * Mathf.Abs(Mathf.Cos(self.localEulerAngles.x * Mathf.Deg2Rad)) * Time.deltaTime);
            // Debug.Log("center " + (distVector.y > 0 ? 1 : -1) * centerRuleDistance * Mathf.Abs(Mathf.Sin(self.localEulerAngles.x * Mathf.Deg2Rad)) * Time.deltaTime);

            // collision detection
            Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z);
            if (hitColliders.Length <= 1)
            {
                shouldMove = true;
            }
        }
        if (shouldMove)
        {
            transform.position = selfPos;
            Debug.DrawLine(selfPos, target.position, Color.blue);
        }
    }

    Transform FindNearestNeighbor(Vector3 selfPos)
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        Transform nearestNeighbor = self;
        float minDist = Mathf.Infinity;
        GameObject test = null;
        foreach (GameObject UAV in UAVs)
        {
            float dist = Vector3.Distance(selfPos, UAV.transform.position);
            if (dist < minDist && UAV.name != self.name)
            {
                test = UAV;
                nearestNeighbor = UAV.transform;
                minDist = dist;
            }
        }
        // Debug.Log(self.name);
        // Debug.Log(test.name);
        return nearestNeighbor;
    }

    void DispersionRule(Vector3 selfPos)
    {
        Transform nearestNeighbor = FindNearestNeighbor(selfPos);
        if (nearestNeighbor.name == self.name)
        {
            nearestNeighbor = target;
        }
        transform.LookAt(nearestNeighbor, transform.up);

        // yaw
        // transform.Rotate(new Vector3(0, 180 + dispersionRotation, 0));
        transform.RotateAround(selfPos, transform.up, (180 + dispersionRotation));
        // pitch
        // transform.RotateAround(selfPos, transform.right, 180 + nearestNeighbor.rotation.x - dispersionRotation);
        // roll
        // transform.RotateAround(selfPos, transform.forward, (180 + nearestNeighbor.rotation.z - dispersionRotation));

        // point agent nose at horizon
        /* transform.RotateAround(selfPos, transform.right, 0);
        if (selfPos.z > target.position.z)
        {
            transform.RotateAround(selfPos, transform.right, -dispersionTilt);
        } else
        {
            transform.RotateAround(selfPos, transform.right, dispersionTilt);
        } */

        selfPos.x += dispersionRuleDistance * transform.forward.x * Time.deltaTime;
        selfPos.y += dispersionRuleDistance * transform.forward.y * Time.deltaTime;
        // selfPos.z += (distVector.z > 0 ? -1 : 1) * dispersionRuleDistance * Mathf.Cos(self.rotation.x) * Time.deltaTime;
        // Debug.Log("disperse x " + (distVector.y > 0 ? 1 : -1) * dispersionRuleDistance * Mathf.Abs(Mathf.Cos(theta * Mathf.Deg2Rad)) * Time.deltaTime);
        // Debug.Log("disperse y " + (distVector.x > 0 ? 1 : -1) * dispersionRuleDistance * Mathf.Abs(Mathf.Sin(theta * Mathf.Deg2Rad)) * Time.deltaTime);

        Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z);
        if (hitColliders.Length <= 1)
        {
            Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.x * transform.TransformDirection(Vector3.right), Color.green);
            Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.y * transform.TransformDirection(Vector3.forward), Color.yellow);
            transform.position = selfPos;
            // Debug.Log("disperse");
        }
    }
}
