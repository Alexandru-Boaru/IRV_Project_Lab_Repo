using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShooter : MonoBehaviour
{
    public Transform origin;
    public Vector3 shootDir;
    public int damage;
    public float fireRate;
    [SerializeField]
    private float fireRateCooldown;
    public float range;
    public float accuracyRingRadius;
    public float accuracyRingDistance;
    public bool autoFire;

    //public Vector3 viewOffset;

    public LayerMask ignoredLayers;
    public LayerMask targetLayers;

    public int numberOfRounds;
    public int ammoSize;
    [SerializeField]
    private int ammoLeft;

    public GameObject bulletHolePrefab;
    public int maxBulletHoles = 100;
    [Min(0)]
    public float timeToDestroyHole;

    public float force;

    
    public float lineEffectTime;
    public GameObject lineEffect;
    public GameObject debugLineEffect;
    public List<GameObject> roundLines = new List<GameObject>();
    public Transform gun;
    private IEnumerator leCor;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ammoLeft = ammoSize;
        
        for(int i = 0; i < numberOfRounds; i++)
        {
            GameObject le = Instantiate(lineEffect, gun, true);
            le.SetActive(false);
            roundLines.Add(le);
        }
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        fireRateCooldown -= Time.deltaTime;
    }

    public void Shoot()
    {
        if (ammoLeft <= 0)
            return;
        if (fireRateCooldown > 0)
        {
            return;
        }
        for (int i = 0; i < numberOfRounds; i++)
        {
            Vector3 v = origin.up * (Random.Range(0f, 1f) * accuracyRingRadius);
            v = Quaternion.AngleAxis(Random.Range(0f, 360f), shootDir) * v;
            Vector3 destination = origin.position + shootDir * accuracyRingDistance + v;
            Vector3 currentShootDir = (destination - origin.position).normalized;
            Debug.DrawLine(origin.position, origin.position + currentShootDir * range, Color.cyan, 0.2f);

            Vector3 bulletHolePosition = origin.position + currentShootDir * range;
            
            RaycastHit hit;
            if(Physics.Raycast(origin.position, currentShootDir, out hit, range, ignoredLayers.value))
            {
                Debug.Log(hit.transform.name);
                bulletHolePosition = hit.point + hit.normal * 0.001f;
                GameObject bh = Instantiate(bulletHolePrefab, bulletHolePosition, Quaternion.LookRotation(hit.normal));
                bh.transform.SetParent(hit.transform, true);
                Destroy(bh, timeToDestroyHole);
                if((1<<hit.transform.gameObject.layer & targetLayers.value) != 0)
                {
                    hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(currentShootDir * force, hit.point);
                }
                EntityStats es = hit.transform.GetComponent<EntityStats>();
                if (es != null)
                {
                    es.TakeDamage(damage);
                }
            }
            if (leCor != null)
                StopCoroutine(leCor);
            roundLines[i].SetActive(false);
            roundLines[i].GetComponent<LineRenderer>().SetPosition(0, gun.position);
            roundLines[i].GetComponent<LineRenderer>().SetPosition(1, bulletHolePosition);
            roundLines[i].SetActive(true);
            leCor = LineEffectCounter();
            StartCoroutine(leCor);
        }

        ammoLeft -= numberOfRounds;
        fireRateCooldown = fireRate;
    }

    
    IEnumerator LineEffectCounter()
    {
        yield return new WaitForSeconds(lineEffectTime);
        Debug.Log("Boo");
        for (int i = 0; i < numberOfRounds; i++)
        {
            roundLines[i].SetActive(false);
        }
    }
    

    public void Recharge()
    {
        ammoLeft = ammoSize;
        fireRateCooldown = 0;
    }

    protected virtual void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin.position, (origin.position + shootDir * range));
        Gizmos.DrawLine(origin.position, (origin.position + origin.up * range));
        
        Vector3 pf;
        Vector3 destination;
        Vector3 dir;
        for (int i = 0; i < 360; i += 5)
        {
            pf = origin.up * accuracyRingRadius;
            pf = Quaternion.AngleAxis(i, shootDir) * pf;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin.position, origin.position + pf);
            Gizmos.color = Color.red;
            destination = origin.position + shootDir * accuracyRingDistance + pf;

            dir = (destination - origin.position).normalized;
            Gizmos.DrawLine(origin.position, destination);
            Gizmos.DrawSphere(destination, 0.1f);
            Gizmos.DrawLine(origin.position, origin.position + dir * range);
        }

        Gizmos.color = Color.cyan;
        pf = origin.up * (Random.Range(0f, 1f) * accuracyRingRadius);
        pf = Quaternion.AngleAxis(Random.Range(0f, 360f), shootDir) * pf;
        destination = origin.position + shootDir * accuracyRingDistance + pf;
        dir = (destination - origin.position).normalized;
        
    }
}
