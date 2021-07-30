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
        float absoluteDistToTarget = Vector3.Distance(targetPos, selfPos);

        CenterRule(selfPos, distToTarget, absoluteDistToTarget);
        DispersionRule(selfPos);
    }

    void CenterRule(Vector3 selfPos, Vector3 distVector, float dist)
    {
        bool shouldMove = false;
        // check if UAV should move backwards
        if (delta - dist > epsilon)
        {
            Debug.Log("move backwards");
            selfPos.x += (distVector.x > 0 ? -1 : 1) * distanceToMove * Mathf.Cos(self.rotation.y) * Time.deltaTime;
            selfPos.y += (distVector.y > 0 ? -1 : 1) * distanceToMove * Mathf.Cos(self.rotation.z) * Time.deltaTime;
            selfPos.z += (distVector.z > 0 ? -1 : 1) * distanceToMove * Mathf.Cos(self.rotation.x) * Time.deltaTime;
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
            Debug.Log("move forwards");
            selfPos.x += (distVector.x > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.y) * Time.deltaTime;
            selfPos.y += (distVector.y > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.z) * Time.deltaTime;
            selfPos.z += (distVector.z > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.x) * Time.deltaTime;
            // collision detection
            Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z);
            Debug.Log(transform.position);
            Debug.Log(selfPos);
            if (hitColliders.Length <= 1)
            {
                shouldMove = true;
            }
        }
        if (shouldMove)
        {
            transform.position = selfPos;
        }
    }

    Transform FindNearestNeighbor(Vector3 selfPos)
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        Transform nearestNeighbor = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject UAV in UAVs)
        {
            float dist = Vector3.Distance(selfPos, UAV.transform.position);
            if (dist < minDist)
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
        transform.LookAt(nearestNeighbor);

        transform.RotateAround(selfPos, transform.right, 180 + nearestNeighbor.rotation.z * Time.deltaTime);
        transform.RotateAround(selfPos, transform.forward, 180 + nearestNeighbor.rotation.x * Time.deltaTime);
        transform.RotateAround(selfPos, transform.up, 180 + nearestNeighbor.rotation.y * Time.deltaTime);
        
        Vector3 distVector = new Vector3(nearestNeighbor.position.x - selfPos.x, nearestNeighbor.position.y - selfPos.y, nearestNeighbor.position.z - selfPos.z);
        selfPos.x += (distVector.x > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.y) * Time.deltaTime;
        selfPos.y += (distVector.y > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.z) * Time.deltaTime;
        selfPos.z += (distVector.z > 0 ? 1 : -1) * distanceToMove * Mathf.Cos(self.rotation.x) * Time.deltaTime;

        Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z);
        if (hitColliders.Length <= 1)
        {
            transform.position = selfPos;
            Debug.Log("disperse");
        }
    }
}
