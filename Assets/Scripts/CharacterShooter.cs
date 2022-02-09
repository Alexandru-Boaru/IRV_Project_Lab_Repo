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
    public float fireRateCooldown;
    public float range;
    public float accuracyRingRadius;
    public float accuracyRingDistance;
    public bool autoFire;

    //public Vector3 viewOffset;

    public LayerMask targetLayers;
    public LayerMask damageLayers;

    public int maxNumberOfRounds;
    public int numberOfRounds;
    public int ammoSize;
    //[SerializeField]
    public int ammoLeft;
    public int ammoTotal;
    [SerializeField]
    protected bool recharging;
    public float rechargingCooldown;

    public GameObject bulletHolePrefab;
    //public int maxBulletHoles = 100;
    public ObjectPooler objectPooler;
    [Min(0)]
    public float timeToDestroyHole;

    public float force;

    
    public float lineEffectTime;
    public GameObject lineEffect;
    public GameObject debugLineEffect;
    public List<GameObject> roundLines = new List<GameObject>();
    public Transform gun;
    public Transform muzzlePoint;
    private IEnumerator leCor;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ammoLeft = ammoSize;
        objectPooler = ObjectPooler.Instance;
        for(int i = 0; i < maxNumberOfRounds; i++)
        {
            GameObject le = Instantiate(lineEffect, muzzlePoint, true);
            le.layer = LayerMask.NameToLayer("FX");
            le.SetActive(false);
            roundLines.Add(le);
        }
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        fireRateCooldown -= fireRateCooldown < 0? 0 : Time.deltaTime;
        if (objectPooler == null)
            objectPooler = ObjectPooler.Instance;
    }

    public virtual void Shoot()
    {
        if (LevelManager.instance.isPaused || PlayerStats.instance.dead)
            return;
        if (ammoLeft <= 0)
        {
            DryGun();
            return;
        }
        if (fireRateCooldown > 0)
        {
            return;
        }
        if (recharging)
            return;
        if (objectPooler != null)
        {
            for (int i = 0; i < numberOfRounds; i++)
            {
                Vector3 v = origin.up * (Random.Range(0f, 1f) * accuracyRingRadius);
                v = Quaternion.AngleAxis(Random.Range(0f, 360f), shootDir) * v;
                Vector3 destination = origin.position + shootDir * accuracyRingDistance + v;
                Vector3 currentShootDir = (destination - origin.position).normalized;
                Debug.DrawLine(origin.position, origin.position + currentShootDir * range, Color.cyan, 0.2f);

                Vector3 bulletHolePosition = origin.position + currentShootDir * range;

                RaycastHit hit;
                if (Physics.Raycast(origin.position, currentShootDir, out hit, range, targetLayers.value))
                {
                    Debug.Log(hit.transform.name);
                    bulletHolePosition = hit.point + hit.normal * 0.001f;
                    /*
                    GameObject bh = Instantiate(bulletHolePrefab, bulletHolePosition, Quaternion.LookRotation(hit.normal));
                    bh.transform.SetParent(hit.transform, true);
                    Destroy(bh, timeToDestroyHole);
                    */
                    objectPooler.SpawnFromPool("bullet", bulletHolePosition, Quaternion.LookRotation(hit.normal), hit.transform);
                    if ((1 << hit.transform.gameObject.layer & damageLayers.value) != 0)
                    {
                        if (hit.transform.TryGetComponent<Rigidbody>(out Rigidbody rb))
                            rb.AddForceAtPosition(currentShootDir * force, hit.point);
                    }
                    EntityStats es = hit.transform.GetComponent<EntityStats>();
                    if (es != null)
                    {
                        es.TakeDamage(damage);
                    }
                }
                if (leCor != null)
                {
                    StopCoroutine(leCor);
                }
                roundLines[i].SetActive(false);
                roundLines[i].GetComponent<LineRenderer>().SetPosition(0, muzzlePoint.position);
                roundLines[i].GetComponent<LineRenderer>().SetPosition(1, bulletHolePosition);
                roundLines[i].SetActive(true);
                leCor = LineEffectCounter();
                StartCoroutine(leCor);
            }
        }

        ammoLeft--;
        fireRateCooldown = fireRate;
    }

    
    IEnumerator LineEffectCounter()
    {
        yield return new WaitForSeconds(lineEffectTime);
        //Debug.Log("Boo");
        for (int i = 0; i < numberOfRounds; i++)
        {
            roundLines[i].SetActive(false);
        }
    }
    

    public void Recharge()
    {
        if (recharging)
            return;
        if (ammoTotal == -1)
        {
            ammoLeft = ammoSize;
        }
        else if (ammoSize <= ammoTotal)
        {
            ammoTotal -= (ammoSize - ammoLeft);
            ammoLeft = ammoSize;
        }
        else if (ammoTotal > 0)
        {
            int ammoDif = ammoSize - ammoLeft;
            if (ammoDif > ammoTotal)
            {
                ammoLeft += ammoTotal;
                ammoTotal = 0;
            }
            else
            {
                ammoLeft = ammoSize;
                ammoTotal -= ammoDif;
            }
        }

        fireRateCooldown = 0;
        StartCoroutine(RechargeCooldown());
    }

    IEnumerator RechargeCooldown()
    {
        recharging = true;
        yield return new WaitForSeconds(rechargingCooldown);
        recharging = false;
    }

    public void AddAmmo(int ammo)
    {
        ammoTotal += ammo;
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

    public virtual void DryGun()
    {

    }
}
