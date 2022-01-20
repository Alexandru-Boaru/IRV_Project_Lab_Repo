using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewGun", menuName = "ScriptableObjects/Gun")]

public class GunScriptableObject : ScriptableObject
{
    [SerializeField] string name = "Pistol";
    [SerializeField] int damage = 5;
    [SerializeField] bool autoFire = false;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] int numberOfRounds = 1;
    [SerializeField] float accuracyRingRadius = 0.2f;
    [SerializeField] float range = 100;
    [SerializeField] int ammoLeft = 10;
    [SerializeField] int ammoSize = 10;
    [SerializeField] int ammoTotal = -1;
    [SerializeField] float force = 200;

    [SerializeField] GameObject model;

    public GunSpecs initializeGunSpecs(Transform parentLocation)
    {
        GunSpecs gunSpecs = new GunSpecs();

        gunSpecs.name = name;
        gunSpecs.damage = damage;
        gunSpecs.autoFire = autoFire;
        gunSpecs.fireRate = fireRate;
        gunSpecs.numberOfRounds = numberOfRounds;
        gunSpecs.accuracyRingRadius = accuracyRingRadius;
        gunSpecs.range = range;
        gunSpecs.ammoLeft = ammoLeft;
        gunSpecs.ammoSize = ammoSize;
        gunSpecs.ammoTotal = ammoTotal;
        gunSpecs.force = force;

        GameObject gunModel = Instantiate<GameObject>(model);

        gunModel.transform.parent = parentLocation;
        gunModel.transform.localPosition = model.transform.position;
        gunModel.transform.localRotation = model.transform.rotation;

        gunSpecs.gunModel = gunModel;
        gunModel.SetActive(false);


        return gunSpecs;
    }

    public void createModel(Vector3 location) { 
    
    }

}
