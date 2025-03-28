using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineBehavior : MonoBehaviour
{
    public float growDuration;
    public float growDistance;
    public Transform player;
    public float strangleDistance;
    public GameObject leafPrefab;
    public int[] leafInterval = new int[2];
    public float vineSineFrequency = 1f;
    public float vineSineAmplitude = 1f;

    private LineRenderer vine;
    private float timer;
    private Vector3 currentPos;
    private Vector3 basePos;
    private List<GameObject> leaves = new List<GameObject>();
    private int summonLeaf = 0;

    // Start is called before the first frame update
    void Start()
    {
        vine = GetComponent<LineRenderer>();
        basePos = currentPos = transform.position;
        summonLeaf = Random.Range(leafInterval[0], leafInterval[1]);
    }

    // Update is called once per frame
    void Update()
    {
        if ((currentPos - player.position).magnitude > strangleDistance)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Strangle();
        }
        
        if(timer >= growDuration)
        {
            vine.positionCount++;
            timer = 0;
            Vector3 dirToPlayer = (player.position - basePos).normalized * growDistance;
            Vector3 perpendicularDir = Vector3.Cross(dirToPlayer, Vector3.up).normalized;
            Vector3 sineWaveOffset = perpendicularDir * Mathf.Sin(Time.time * vineSineFrequency) * vineSineAmplitude;
            
            basePos += dirToPlayer;
            currentPos = basePos + sineWaveOffset;
            vine.SetPosition(vine.positionCount - 1, currentPos - transform.position);

            if (vine.positionCount == summonLeaf)
            {
                summonLeaf += Random.Range(leafInterval[0], leafInterval[1]);
                Vector3 sphereRandom = Random.onUnitSphere;
                leaves.Add(ObjectPoolManager.CreatePooled(leafPrefab, currentPos + sphereRandom * 0.06f, Quaternion.LookRotation(player.position - currentPos)));
                leaves[^1].transform.SetParent(transform);
            }
        }
    }

    private void Strangle()
    {

    }
}
