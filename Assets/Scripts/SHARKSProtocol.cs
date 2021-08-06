using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class SHARKSProtocol : MonoBehaviour
{
    public Transform self;
    public Transform target;

    public float movementSpeed;
    public float centerRuleDistance;
    public float dispersionRuleDistance;

    public float dispersionRotation;
    public float dispersionTilt;
    public float delta;
    public float epsilon;

    private bool inStabilityRegion = false;
    private bool detectedAdversary = false;

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

        selfPos = transform.position;
        if (inStabilityRegion && IsNearestObjectAdversarial(selfPos) && detectedAdversary)
        {
            DynamicDistanceEjection(selfPos);
        }
    }

    bool InsideStabilityRegion(float distToTarget)
    {
        return distToTarget <= delta + epsilon && distToTarget >= delta - epsilon;
    }

    bool IsNearestObjectAdversarial(Vector3 selfPos)
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        Transform nearestObject = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject UAV in UAVs)
        {
            if (UAV.name == "PlayerUAV")
            {
                if (InsideStabilityRegion(Vector3.Distance(UAV.transform.position, target.position)))
                {
                    detectedAdversary = true;
                }
                else
                {
                    detectedAdversary = false;
                }
            }
            else if (UAV.name != self.name)
            {
                float dist = Vector3.Distance(selfPos, UAV.transform.position);
                if (dist < minDist)
                {
                    nearestObject = UAV.transform;
                    minDist = dist;
                }
            }
        }

        return nearestObject;
    }

    void CenterRule(Vector3 selfPos, float dist)
    {
        transform.LookAt(target);

        bool shouldMove = false;
        // check if UAV should move backwards
        if (delta - dist > epsilon)
        {
            // Debug.Log("move backwards");
            selfPos.x += centerRuleDistance * -transform.forward.x * movementSpeed * Time.deltaTime;
            selfPos.y += centerRuleDistance * -transform.forward.y * movementSpeed * Time.deltaTime;
            selfPos.z += centerRuleDistance * -transform.forward.z * movementSpeed * Time.deltaTime;

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
            selfPos.x += centerRuleDistance * transform.forward.x * movementSpeed * Time.deltaTime;
            selfPos.y += centerRuleDistance * transform.forward.y * movementSpeed * Time.deltaTime;
            selfPos.z += centerRuleDistance * transform.forward.z * movementSpeed * Time.deltaTime;

            // collision detection
            Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z / 2);
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
        
        if (InsideStabilityRegion(dist))
        {
            inStabilityRegion = true;
        }
    }

    Transform FindNearestNeighbor(Vector3 selfPos)
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        Transform nearestNeighbor = self;
        float minDist = Mathf.Infinity;
        foreach (GameObject UAV in UAVs)
        {
            if (UAV.name != "PlayerUAV" && UAV.name != self.name)
            {
                float dist = Vector3.Distance(selfPos, UAV.transform.position);
                if (dist < minDist)
                {
                    nearestNeighbor = UAV.transform;
                    minDist = dist;
                }
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

        // point agent nose at horizon
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, eulerRotation.y, eulerRotation.z);
        // adjust tilt based on y position to target
        if (selfPos.y > target.position.y)
        {
            transform.RotateAround(selfPos, transform.right, -dispersionTilt);
        } else
        {
            transform.RotateAround(selfPos, transform.right, dispersionTilt);
        }

        selfPos.x += dispersionRuleDistance * transform.forward.x * movementSpeed * Time.deltaTime;
        selfPos.y += dispersionRuleDistance * transform.forward.y * movementSpeed * Time.deltaTime;
        selfPos.z += dispersionRuleDistance * transform.forward.z * movementSpeed * Time.deltaTime;

        Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z / 2);
        if (hitColliders.Length <= 1)
        {
            // Debug.Log("disperse");
            /* Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.x * transform.TransformDirection(Vector3.right), Color.red);
            Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.y * transform.TransformDirection(Vector3.up), Color.green);
            Debug.DrawRay(transform.position, dispersionRuleDistance * transform.forward.z * transform.TransformDirection(Vector3.forward), Color.blue);
            */
            Debug.DrawRay(transform.position, new Vector3(dispersionRuleDistance * transform.forward.x * movementSpeed, dispersionRuleDistance * transform.forward.y * movementSpeed, dispersionRuleDistance * transform.forward.z * movementSpeed), Color.red);
            transform.position = selfPos;
        }
    }
     
    float CalculateIdealDistance()
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");
        float numAgents = UAVs.Length - 1;
        return Mathf.Sin(180 / numAgents * Mathf.Deg2Rad) * 2 * delta;
    }

    float CalculateError()
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");
        GameObject[] legitimateUAVs = new GameObject[UAVs.Length - 1];
        int idx = 0;
        foreach (GameObject UAV in UAVs)
        {
            if (UAV.name != "PlayerUAV")
            {
                legitimateUAVs[idx] = UAV;
                idx++;
            }
        }

        float averageDistanceBetweenAgents = 0;
        for (int i = 0; i < legitimateUAVs.Length; i++)
        {
            if (i == legitimateUAVs.Length - 1)
            {
                averageDistanceBetweenAgents += Vector3.Distance(legitimateUAVs[i].transform.position, legitimateUAVs[0].transform.position);
            }
            else
            {
                averageDistanceBetweenAgents += Vector3.Distance(legitimateUAVs[i].transform.position, legitimateUAVs[i + 1].transform.position);
            }
        }
        averageDistanceBetweenAgents /= legitimateUAVs.Length;

        return Mathf.Abs(CalculateIdealDistance() - averageDistanceBetweenAgents);
    }

    void DynamicDistanceEjection(Vector3 selfPos)
    {
        // Debug.Log("eject!");
        transform.LookAt(target);

        float ejectDistance = delta / 2 * Mathf.Pow(CalculateError(), 1 / 4);
        selfPos.x += ejectDistance * transform.forward.x * movementSpeed * Time.deltaTime;
        selfPos.y += ejectDistance * transform.forward.y * movementSpeed * Time.deltaTime;
        selfPos.z += ejectDistance * transform.forward.z * movementSpeed * Time.deltaTime;

        // collision detection
        Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z / 2);
        if (hitColliders.Length <= 1)
        { 
            transform.position = selfPos;
            Debug.DrawLine(selfPos, target.position, Color.white);
        }
    }
}
