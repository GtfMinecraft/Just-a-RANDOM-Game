using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HostileEntity : Entity
{
    public Transform player;
    public float[] wanderDistance = new float[2];
    public float[] wanderInterval = new float[2];

    [Header("Behavior")]
    public float rallyRadius = 6;
    public float alertConeDistance;
    public float alertConeRadius;
    public float alertRadius;
    public float unalertDistance;
    public float unalertTime;

    protected NavMeshAgent agent;
    protected float wanderTimer;
    protected Vector3 biasedNormal;
    protected float previousDistance;
    protected Vector3 previousDestination;

    protected bool alerted = false;
    //protected Transform player;
    protected float outerAlertRadius;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        wanderTimer = Random.Range(wanderInterval[0], wanderInterval[1]);
        agent = GetComponent<NavMeshAgent>();
        //player = PlayerController.instance.transform;

        outerAlertRadius = Mathf.Sqrt(alertConeRadius * alertConeRadius + alertConeDistance * alertConeDistance);
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
                //wander also add a central place for each of them to separate after unalerted

                wanderTimer = Random.Range(wanderInterval[0], wanderInterval[1]);
            }

            if (Vector3.Distance(transform.position, player.position) <= outerAlertRadius)
            {
                //detect if player is in a cone shape in front vision + sphere space around
            }
        }
        else
        {
            //move towards player

            if(Vector3.Distance(transform.position, player.position) > unalertDistance)
            {
                //Invoke(Unalert, unalertTime)
            }
            else
            {
                //CancelInvoke Unalert
            }
        }
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        Collider[] hits = Physics.OverlapSphere(transform.position, rallyRadius);

        foreach (Collider hit in hits)
        {
            HostileEntity entity = hit.GetComponent<HostileEntity>();
            if(entity != null)
            {
                entity.alerted = true;
            }
        }

        //call a circle alert for allies when being attacked
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
