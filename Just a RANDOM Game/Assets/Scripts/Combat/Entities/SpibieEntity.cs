using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpibieEntity : Entity
{
    public Transform target;
    public float[] wanderDistance = new float[2];
    public float[] wanderInterval = new float[2];

    private NavMeshAgent agent;
    private float wanderTimer;
    private Vector3 biasedNormal;
    private float previousDistance;
    private Vector3 previousDestination;

    private bool alerted = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        wanderTimer = Random.Range(wanderInterval[0], wanderInterval[1]);
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(!alerted)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                Wander();
                wanderTimer = Random.Range(wanderInterval[0], wanderInterval[1]);
            }
        }
        else
        {
            //if player is detected in a cone shape in front vision + sphere space around
            //move towards player

            //call a circle alert for allies when being attacked
        }
    }

    private void Wander()
    {
        Vector3 direction = BiasDirectionAwayFromBorder();
        previousDestination = transform.position + direction;
        previousDistance = direction.magnitude;

        agent.SetDestination(previousDestination);
        
        biasedNormal = agent.destination - previousDestination;
        biasedNormal.y = 0;
    }

    private Vector3 BiasDirectionAwayFromBorder()
    {
        if (biasedNormal.magnitude < 0.1f)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            return new Vector3(randomDir.x, 0, randomDir.y) * Random.Range(wanderDistance[0], wanderDistance[1]);
        }

        float mu = Mathf.Atan2(biasedNormal.z, biasedNormal.x);
        float sigma = 60f / (1 + biasedNormal.magnitude / previousDistance) * Mathf.Deg2Rad;
        float angle = BoxMuller(mu, sigma);

        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * Random.Range(wanderDistance[0], wanderDistance[1]);
    }

    private float BoxMuller(float mu, float sigma)
    {
        float u1 = Random.value;
        float u2 = Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mu + sigma * randStdNormal;
    }
}
