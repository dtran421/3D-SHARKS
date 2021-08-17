using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public Transform player;
    public GameObject UAV;
    public int epochWait;

    private SHARKSProtocol sharksProtocol;

    private float movementDistance;
    private float movementSpeed;

    private Transform randomSeekingUAV;
    private Vector3 lastSeekingUAVPos;
    private int epochTimer = 0;
    private bool reachedSeekingUAV = false;

    void Start()
    {
        sharksProtocol = UAV.GetComponent<SHARKSProtocol>();

        Vector3 randomPos = sharksProtocol.GenerateRandomPosition();
        Collider[] hitColliders = Physics.OverlapSphere(randomPos, transform.localScale.z);
        while (hitColliders.Length > 1)
        {
            randomPos = sharksProtocol.GenerateRandomPosition();
            hitColliders = Physics.OverlapSphere(randomPos, transform.localScale.z);
        }
        player.position = randomPos;

        randomSeekingUAV = FindRandomUAV();
        movementDistance = Mathf.Max(sharksProtocol.centerRuleDistance, sharksProtocol.dispersionRuleDistance);
        movementSpeed = sharksProtocol.movementSpeed;
    }

    void Update()
    {   
        Vector3 selfPos = player.position;

        if (reachedSeekingUAV)
        {
            if (Vector3.Distance(selfPos, lastSeekingUAVPos) > player.localScale.z)
            {
                transform.LookAt(lastSeekingUAVPos);
                selfPos = Seek(selfPos);
            }

            if (epochTimer < epochWait)
            {
                epochTimer++;
            } 
            else
            {
                epochTimer = 0;
                reachedSeekingUAV = false;
                randomSeekingUAV = FindRandomUAV();
            }
        } 
        else
        {
            if (Vector3.Distance(selfPos, randomSeekingUAV.position) < player.localScale.z * 2)
            {
                reachedSeekingUAV = true;
                lastSeekingUAVPos = randomSeekingUAV.position;
            }

            transform.LookAt(randomSeekingUAV);
            selfPos = Seek(selfPos);
        }

        transform.position = selfPos;
    }

    Vector3 Seek(Vector3 selfPos)
    {
        selfPos.x += transform.forward.x * movementDistance * movementSpeed * Time.deltaTime;
        selfPos.y += transform.forward.y * movementDistance * movementSpeed * Time.deltaTime;
        selfPos.z += transform.forward.z * movementDistance * movementSpeed * Time.deltaTime;

        return selfPos;
    }

    Transform FindRandomUAV()
    {
        GameObject[] UAVs = GameObject.FindGameObjectsWithTag("UAV");

        int randomNum = Random.Range(0, UAVs.Length - 1);
        while (UAVs[randomNum].name == "PlayerUAV")
        {
            randomNum = Random.Range(0, UAVs.Length - 1);
        }

        return UAVs[randomNum].transform;
    }
}
