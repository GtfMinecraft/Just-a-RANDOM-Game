using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineBehavior : MonoBehaviour
{
    public float growSpeed;
    public float growDistance;
    public Transform player;
    public float strangleDistance;
    public GameObject leafPrefab;

    private LineRenderer vine;
    private float timer;
    private Vector3 currentPos;
    private List<GameObject> leaves = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        vine = GetComponent<LineRenderer>();
        currentPos = transform.position;
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
        
        if(timer >= growSpeed)
        {
            vine.positionCount++;
            timer = 0;
            Vector3 dirToPlayer = (player.position - currentPos).normalized;
            currentPos += dirToPlayer * growDistance;
            vine.SetPosition(vine.positionCount - 1, currentPos - transform.position);

            if (vine.positionCount % 10 == 0)
            {
                Vector3 sphereRandom = Random.onUnitSphere;
                leaves.Add(ObjectPoolManager.CreatePooled(leafPrefab, currentPos + sphereRandom * 0.06f, Quaternion.Euler(sphereRandom)));
                leaves[^1].transform.SetParent(transform);
            }
        }
    }

    private void Strangle()
    {

    }
}
