using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunSpecs
{
    public string name;
    public int damage;
    public bool autoFire;
    public float fireRate;
    public int numberOfRounds;

    public float accuracyRingRadius;
    public float range;

    public int ammoLeft;
    public int ammoSize;
    public int ammoTotal;

    public float force;
    public string gunSoundName;
    public GameObject gunModel;
    public Transform muzzle;
}
public class PlayerShooter : CharacterShooter
{
    public new Transform camera;
    public float angle;
    public PlayerInput input;
    public Animator anim;
    public GunUIManager gum;

    public int currentGunId;
    public string gunName;
    public List<GunSpecs> guns;
    public List<GunScriptableObject> gunObjects;		

    public GameObject cone;
    public AudioController audioController;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        origin = camera;
        shootDir = camera.forward;
		guns.Clear();
        foreach(GunScriptableObject gunObj in gunObjects)
        {
            guns.Add(gunObj.initializeGunSpecs(gun));
        }
		
        gum.StartGuns();
        if(audioController == null)
            audioController = GetComponentInChildren<AudioController>();
        SetGun(0);
    }

    public void ResetPlayerGun()
    {
        guns.Clear();
        foreach (GunScriptableObject gunObj in gunObjects)
        {
            guns.Add(gunObj.initializeGunSpecs(gun));
        }
        SetGun(0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        shootDir = camera.forward;
        if (recharging)
            return;
        if (autoFire && input.shootAuto)
        {
            if (ammoLeft > 0 && fireRateCooldown <= 0 && !recharging)
            {
                if (audioController) audioController.Play(guns[currentGunId].gunSoundName);
            }
            if (fireRateCooldown < 0)
                anim.SetTrigger("shoot");
            Shoot();
            
            gum.UpdateGunUI();
        }
        else if(!autoFire && input.shootOnce)
        {
            if (ammoLeft > 0 && fireRateCooldown <= 0 && !recharging)
            {
                if (audioController) audioController.Play(guns[currentGunId].gunSoundName);
            }
            if (fireRateCooldown < 0)
                anim.SetTrigger("shoot");
            Shoot();
            gum.UpdateGunUI();
            //if (audioController) audioController.Play(guns[currentGunId].gunSoundName);
            input.shootOnce = false;
        }
        if(!input.shootAuto)
        {
            input.shootOnce = false;
        }
        if (!input.shootAuto)
        {
            anim.SetBool("shootAuto", false);
        }
        if (!input.shootAuto && autoFire)
        {
            if (audioController) audioController.Stop(guns[currentGunId].gunSoundName);
        }
        if (input.mustRecharge)
        {
            Recharge();
            gum.UpdateGunUI();
            anim.SetTrigger("recharge");
            input.mustRecharge = false;
        }
        SwitchWeapon();
        anim.SetInteger("bulletsLeft", ammoLeft);
        SetConeScale();

    }

    public void SetConeScale()
    {
        float coneRad = accuracyRingRadius * range / accuracyRingDistance;
        cone.transform.localScale = new Vector3(coneRad, coneRad, range / 2);
    }

    public void SwitchWeapon()
    {
        GunSpecs gs = guns[currentGunId];
        gs.ammoTotal = ammoTotal;
        gs.ammoLeft = ammoLeft;
        if (input.nextWeapon)
        {
            currentGunId = (currentGunId + 1) % guns.Count;
            SetGun(currentGunId);
        }
        if (input.prevWeapon)
        {
            currentGunId = currentGunId - 1;
            if (currentGunId < 0)
                currentGunId = guns.Count - 1;
            SetGun(currentGunId);
        }
        input.nextWeapon = false;
        input.prevWeapon = false;
    }

    public void SetGun(int i)
    {
        GunSpecs gs = guns[i];
        gunName = gs.name;
        damage = gs.damage;
        autoFire = gs.autoFire;
        fireRate = gs.fireRate;
        numberOfRounds = gs.numberOfRounds;

        accuracyRingRadius = gs.accuracyRingRadius;
        range = gs.range;

        ammoLeft = gs.ammoLeft;
        ammoSize = gs.ammoSize;
        ammoTotal = gs.ammoTotal;
        force = gs.force;
        gum.UpdateGunUI();
		
		foreach ( GunSpecs gunSpec in guns)
        {
            gunSpec.gunModel.SetActive(false);
        }

        gs.gunModel.SetActive(true);
        if (gs.muzzle == null)
            muzzlePoint.position = gun.position;
        else
            muzzlePoint.position = gs.muzzle.position;
    }



    public void AddAmmo(int i, int ammo)
    {
        GunSpecs gs = guns[i];
        gs.ammoTotal += ammo;
        if (i == currentGunId)
        {
            ammoTotal += ammo;
            gum.UpdateGunUI();
        }
    }

    public override void DryGun()
    {
        audioController.Play("Dry_Shoot");
    }

    protected override void OnDrawGizmos()
    {
        origin = camera;
        shootDir = camera.forward;
        base.OnDrawGizmos();
    }
}
