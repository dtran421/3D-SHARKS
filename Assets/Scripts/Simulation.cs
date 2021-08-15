using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject UAV;

    private long epoch = -1;
    private SHARKSProtocol sharksProtocol;

    // Start is called before the first frame update
    void Start()
    {
        string simName = "testing2";
        Debug.Log("*[begin simulation]: " + simName);
        sharksProtocol = UAV.GetComponent<SHARKSProtocol>();
    }

    // Update is called once per frame
    void Update()
    {
        epoch++;
        Debug.Log("*[epoch]: " + epoch);
        Debug.Log("*[error]: " + CalculateError() + "%");
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
                    cumulativeError += (distToTarget - (sharksProtocol.delta + sharksProtocol.epsilon)) / (sharksProtocol.epsilon * 2);
                } else
                {
                    cumulativeError += (sharksProtocol.delta - sharksProtocol.epsilon - distToTarget) / (sharksProtocol.epsilon * 2);
                }
            }
        }
        int numAgents = UAVs.Length - 1;
        float averageError = cumulativeError / numAgents;

        return averageError * 100;
    }
}
