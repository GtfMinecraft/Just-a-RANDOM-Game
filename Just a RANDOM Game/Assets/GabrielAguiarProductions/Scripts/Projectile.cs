using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    public bool followGround = true;
    public Damage damage;
    public bool slowDown;
    public float slowDownRate = 0.01f;
    public float detectingDistance = 0.1f;
    public float destroyDelay = 5;
    public float objectsToDetachDelay = 2;
    public List<GameObject> objectsToDetach = new List<GameObject>();
    [Space]
    public float erodeInRate = 0.06f;
    public float erodeOutRate = 0.03f;
    public float erodeRefreshRate = 0.01f;
    public float erodeAwayDelay = 1.25f;
    public List<SkinnedMeshRenderer> objectsToErode = new List<SkinnedMeshRenderer>();

    private Rigidbody rb;
    public bool fired { get; private set; } = false;
    private bool stopped;
    private GameObject parentObj;

    private void Start()
    {
        fired = false;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        StartStopSpin(false);
    }

    private void Update()
    {
        if (!fired)
        {
            Vector3 arrowDirection = Camera.main.GetComponent<ThirdPersonCam>().combatLookAt.position - Camera.main.transform.position;
            transform.rotation = Quaternion.LookRotation(arrowDirection);
        }
    }

    public void Fire()
    {
        fired = true;
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(true);

        if (followGround)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if(slowDown)
                StartCoroutine(SlowDown());

        if (objectsToDetach != null)
            StartCoroutine(DetachObjects());

        if (objectsToErode != null)
            StartCoroutine(ErodeObjects());

        StartStopSpin(true);
        ObjectPoolManager.DestroyPooled(gameObject, destroyDelay);
    }

    private void StartStopSpin(bool start)
    {
        transform.GetChild(1).gameObject.SetActive(start);
        transform.GetChild(2).gameObject.SetActive(start);
        transform.GetChild(3).gameObject.SetActive(start);

        VisualEffect arrowVFX = transform.GetChild(0).GetComponent<VisualEffect>();
        arrowVFX.SetFloat("ArrowSpinVelocity", start ? 1.33f : 0);
        arrowVFX.SetFloat("StretchedParticlesRate", start ? 18 : 0);
        arrowVFX.SetFloat("SmokeRate", start ? 32 : 0);
    }

    private void FixedUpdate()
    {
        if (!stopped && followGround)
        {
            RaycastHit hit;
            Vector3 distance = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            if (Physics.Raycast(distance, transform.TransformDirection(-Vector3.up), out hit, detectingDistance))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            Debug.DrawRay(distance, transform.TransformDirection(-Vector3.up * detectingDistance), Color.red);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fired)
        {
            StartStopSpin(false);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.SetParent(other.transform);
            parentObj = other.gameObject;
            if (other.gameObject.GetComponent<Entity>() != null)
            {
                other.gameObject.GetComponent<Entity>().TakeDamage(damage);
            }
        }
    }

    private void OnEnable()
    {
        if(transform.parent != null && transform.parent.gameObject == parentObj)
        {
            ObjectPoolManager.DestroyPooled(gameObject);
        }
    }

    IEnumerator SlowDown ()
    {
        float t = 1;
        while (t > 0)
        {
            rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, t);
            t -= slowDownRate;
            yield return new WaitForSeconds(0.1f);
        }

        stopped = true;
    }

    IEnumerator DetachObjects ()
    {
        yield return new WaitForSeconds(objectsToDetachDelay);

        for (int i=0; i<objectsToDetach.Count; i++)
        {
            objectsToDetach[i].transform.parent = null;
            Destroy(objectsToDetach[i], objectsToDetachDelay);
        }
    }

    IEnumerator ErodeObjects()
    {
        for (int i = 0; i < objectsToErode.Count; i++)
        {
            float t = 1;
            while (t > 0)
            {
                t -= erodeInRate;
                for (int j = 0; j < objectsToErode[i].materials.Length; j++)
                {
                    objectsToErode[i].materials[j].SetFloat("_Erode", t);
                }
                yield return new WaitForSeconds(erodeRefreshRate);
            }
        }

        yield return new WaitForSeconds(erodeAwayDelay);

        for (int i = 0; i < objectsToErode.Count; i++)
        {
            float t = 0;
            while (t < 1)
            {
                t += erodeOutRate;
                for (int j = 0; j < objectsToErode[i].materials.Length; j++)
                {
                    objectsToErode[i].materials[j].SetFloat("_Erode", t);
                }
                yield return new WaitForSeconds(erodeRefreshRate);
            }
        }
    }
}
