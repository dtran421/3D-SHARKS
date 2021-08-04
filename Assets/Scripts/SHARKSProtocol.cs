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

        float absoluteDistToTarget = Vector3.Distance(targetPos, selfPos);
        // Debug.Log(absoluteDistToTarget);
        
        CenterRule(selfPos, absoluteDistToTarget);
        selfPos = transform.position;
        DispersionRule(selfPos);
    }

    void CenterRule(Vector3 selfPos, float dist)
    {
        transform.LookAt(target);

        bool shouldMove = false;
        // check if UAV should move backwards
        if (delta - dist > epsilon)
        {
            // Debug.Log("move backwards");
            selfPos.x += centerRuleDistance * -transform.forward.x * Time.deltaTime;
            selfPos.y += centerRuleDistance * -transform.forward.y * Time.deltaTime;
            selfPos.z += centerRuleDistance * -transform.forward.z * Time.deltaTime;

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
            selfPos.z += centerRuleDistance * transform.forward.z * Time.deltaTime;

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
            Debug.DrawLine(selfPos, target.position, Color.white);
        }
    }

    Transform FindNearestNeighbor(Vector3 selfPos)
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        Transform nearestNeighbor = self;
        float minDist = Mathf.Infinity;
        foreach (GameObject UAV in UAVs)
        {
            float dist = Vector3.Distance(selfPos, UAV.transform.position);
            if (dist < minDist && UAV.name != self.name)
            {
                nearestNeighbor = UAV.transform;
                minDist = dist;
            }
        }
        return nearestNeighbor;
    }

    void DispersionRule(Vector3 selfPos)
    {
        Transform nearestNeighbor = FindNearestNeighbor(selfPos);
        if (nearestNeighbor.name == self.name)
        {
            nearestNeighbor = target;
        }
        transform.LookAt(nearestNeighbor);

        // yaw
        transform.RotateAround(selfPos, transform.up, 180 + dispersionRotation);
        // pitch
        // transform.RotateAround(selfPos, transform.right, 180 + nearestNeighbor.rotation.x - dispersionRotation);
        // roll
        // transform.RotateAround(selfPos, transform.forward, 180 + dispersionRotation);

        // point agent nose at horizon
        transform.RotateAround(selfPos, transform.right, 0);
        if (selfPos.z > target.position.z)
        {
            transform.RotateAround(selfPos, transform.right, -dispersionTilt);
        } else
        {
            transform.RotateAround(selfPos, transform.right, dispersionTilt);
        }

        selfPos.x += dispersionRuleDistance * transform.forward.x * Time.deltaTime;
        selfPos.y += dispersionRuleDistance * transform.forward.y * Time.deltaTime;
        selfPos.z += dispersionRuleDistance * transform.forward.z * Time.deltaTime;

        Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z);
        if (hitColliders.Length <= 1)
        {
            // Debug.Log("disperse");
            Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.x * transform.TransformDirection(Vector3.right), Color.red);
            Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.y * transform.TransformDirection(Vector3.up), Color.green);
            Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.z * transform.TransformDirection(Vector3.forward), Color.blue);
            transform.position = selfPos;
        }
    }
}
