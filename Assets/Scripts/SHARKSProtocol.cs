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
    public float delta; // radius
    public float epsilon; // radius error (tolerance)

    public float deltaThreshold;
    public float gamma; // radius adaptation increment
    private Vector3 adversaryPos;

    private Vector3 lastPos;

    void Start()
    {
        Vector3 randomPos = GenerateRandomPosition();
        Collider[] hitColliders = Physics.OverlapSphere(randomPos, transform.localScale.z);
        while (hitColliders.Length > 1)
        {
            randomPos = GenerateRandomPosition();
            hitColliders = Physics.OverlapSphere(randomPos, transform.localScale.z);
        }
        self.position = randomPos;
        lastPos = randomPos;
    }

    public Vector3 GenerateRandomPosition()
    {
        float lowerBound = -30f;
        float upperBound = 30f;
        return new Vector3(Random.Range(lowerBound, upperBound), Random.Range(lowerBound, upperBound), Random.Range(lowerBound, upperBound));
    }

    void Update()
    {
        Vector3 selfPos = transform.position;
        Vector3 targetPos = target.position;

        lastPos = selfPos;

        float distToTarget = Vector3.Distance(targetPos, selfPos);
        // Debug.Log(distToTarget);

        bool approachingAdversary = false;
        Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.name == "PlayerUAV")
            {
                // approachingAdversary = true; * uncomment for real code!!
                adversaryPos = collider.gameObject.transform.position;
            }
        }
        if (IsNearestObjectAdversarial(selfPos) && approachingAdversary)
        {
            if (delta <= deltaThreshold)
            {
                DynamicDistanceEjection(selfPos);
            } 
            else if (InsideStabilityRegion(distToTarget, epsilon))
            {
                AdaptDelta(distToTarget);
            }
        } 
        else
        {
            CenterRule(selfPos, distToTarget);
            selfPos = transform.position;
            DispersionRule(selfPos);
        }

        transform.LookAt(target);
    }

    public bool InsideStabilityRegion(float distToTarget, float tolerance)
    {
        return distToTarget <= delta + tolerance && distToTarget >= delta - tolerance;
    }

    bool IsNearestObjectAdversarial(Vector3 selfPos)
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        Transform nearestObject = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject UAV in UAVs)
        {
            if (UAV.name != self.name)
            {
                float dist = Vector3.Distance(selfPos, UAV.transform.position);
                if (dist < minDist)
                {
                    nearestObject = UAV.transform;
                    minDist = dist;
                }
            }
        }

        return nearestObject.name == "PlayerUAV";
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
            Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z / 2);
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
        // Vector3 eulerRotation = transform.rotation.eulerAngles;
        // transform.rotation = Quaternion.Euler(0, eulerRotation.y, eulerRotation.z);
        // adjust tilt based on y position to target
        if (selfPos.y > nearestNeighbor.position.y)
        {
            transform.RotateAround(selfPos, transform.right, -dispersionTilt);
        }
        else
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
            Debug.DrawRay(transform.position, new Vector3(dispersionRuleDistance * transform.forward.x * movementSpeed, dispersionRuleDistance * transform.forward.y * movementSpeed, dispersionRuleDistance * transform.forward.z * movementSpeed), Color.red);
            transform.position = selfPos;
        }
    }

    float CalculateIdealDistance()
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");
        int numAgents = UAVs.Length - 1;
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

        float averageDistBetweenAgents = 0;
        for (int i = 0; i < legitimateUAVs.Length; i++)
        {
            if (i == legitimateUAVs.Length - 1)
            {
                averageDistBetweenAgents += Vector3.Distance(legitimateUAVs[i].transform.position, legitimateUAVs[0].transform.position);
            }
            else
            {
                averageDistBetweenAgents += Vector3.Distance(legitimateUAVs[i].transform.position, legitimateUAVs[i + 1].transform.position);
            }
        }
        averageDistBetweenAgents /= legitimateUAVs.Length;

        float idealDist = CalculateIdealDistance();
        return Mathf.Abs(averageDistBetweenAgents - idealDist) /  idealDist;
    }

    void DynamicDistanceEjection(Vector3 selfPos)
    {
        // Debug.Log("eject!");
        transform.LookAt(adversaryPos);

        Vector3 ejectVector = -transform.forward + -transform.right;
        float ejectDistance = delta / 2 * Mathf.Pow(CalculateError(), 1 / 4);
        selfPos.x += ejectVector.x * ejectDistance * movementSpeed * Time.deltaTime;
        selfPos.y += ejectVector.y * ejectDistance * movementSpeed * Time.deltaTime;
        selfPos.z += ejectVector.z * ejectDistance * movementSpeed * Time.deltaTime;

        // collision detection
        Collider[] hitColliders = Physics.OverlapSphere(selfPos, transform.localScale.z / 2);
        if (hitColliders.Length <= 1)
        {
            transform.position = selfPos;
        }
        Debug.DrawRay(selfPos, new Vector3(ejectVector.x * ejectDistance * movementSpeed, ejectVector.y * ejectDistance * movementSpeed, ejectVector.z * ejectDistance * movementSpeed), Color.blue);
    }

    void AdaptDelta(float distToTarget)
    {
        // Debug.Log("adapt!");
        if (distToTarget > deltaThreshold)
        {
            delta -= gamma;
        }
    }

    public Vector3 GetLastPosition()
    {
        return lastPos;
    }
}