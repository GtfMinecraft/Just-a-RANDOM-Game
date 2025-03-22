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
                Transform f = Instantiate(target);
                f.position = transform.position;
                Wander();
                wanderTimer = Random.Range(wanderInterval[0], wanderInterval[1]);
            }
        }
        else
        {
            //call a sphere alert for allies when being attacked
            //move towards player
        }
    }

    private void Wander()
    {
        Vector3 targetPos = transform.position + BiasDirectionAwayFromBorder();

        agent.SetDestination(targetPos);

        StartCoroutine(CheckForBorder());
    }

    private IEnumerator CheckForBorder()
    {
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        biasedNormal = transform.position - agent.destination;
    }

    private Vector3 BiasDirectionAwayFromBorder()
    {
        Vector2 direction = Random.insideUnitCircle.normalized * Random.Range(wanderDistance[0], wanderDistance[1]);
        
        //biased towards biasedNormal direction according to the magnitude

        return new Vector3(direction.x, 0, direction.y);
    }
}
