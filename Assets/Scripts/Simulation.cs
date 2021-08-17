using UnityEngine;

public class Simulation : MonoBehaviour
{
    public string simName;
    public GameObject UAV;
    public float epochLimit;

    private long epoch = -1;
    private SHARKSProtocol sharksProtocol;

    void Start()
    {
        Debug.Log("*[begin simulation]: " + simName);
        sharksProtocol = UAV.GetComponent<SHARKSProtocol>();
    }

    void Update()
    {
        epoch++;
        Debug.Log("*[epoch]: " + epoch);
        Debug.Log("*[speed]: " + CalculateSpeed());
        Debug.Log("*[error]: " + CalculateError());

        if (epoch >= epochLimit)
        {
            Debug.Break();
        }
    }

    float CalculateSpeed()
    {
        float cumulativeSpeed = 0;
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        foreach (GameObject UAV in UAVs)
        {
            if (UAV.name != "PlayerUAV")
            {
                float distTraveled = Vector3.Distance(UAV.transform.position, UAV.GetComponent<SHARKSProtocol>().GetLastPosition());
                cumulativeSpeed += distTraveled / Time.deltaTime;
            }
        }
        int numAgents = UAVs.Length - 1;
        return cumulativeSpeed / numAgents;
    }

    float CalculateError()
    {
        float cumulativeError = 0;
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        foreach (GameObject UAV in UAVs)
        {
            float distToTarget = Vector3.Distance(UAV.transform.position, sharksProtocol.target.position);
            if (UAV.name != "PlayerUAV" && !sharksProtocol.InsideStabilityRegion(distToTarget, sharksProtocol.epsilon))
            {
                if (distToTarget > sharksProtocol.delta + sharksProtocol.epsilon)
                {
                    cumulativeError += (distToTarget - (sharksProtocol.delta + sharksProtocol.epsilon)) / sharksProtocol.epsilon;
                } 
                else
                {
                    cumulativeError += (sharksProtocol.delta - sharksProtocol.epsilon - distToTarget) / sharksProtocol.epsilon;
                }
            }
        }
        int numAgents = UAVs.Length - 1;
        return cumulativeError / numAgents;
    }
}
