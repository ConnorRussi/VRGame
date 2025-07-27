using System.Numerics;
using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering;

public class Revolver : MonoBehaviour
{
    //Player variables
    [Header("Player Variables")]
    public int maxAmmo = 6;
    public int currentAmmo;
    public AudioClip revolverDryFire, revClick;
    private bool cylinderOpen;
    public GameObject[] bullets;

    [Header("Owner Variables")]
    public bool PlayerIsHolding;
    public enum GunOwner { Player, NPC }
    public GunOwner owner = GunOwner.Player; // Default to player, set to NPC for NPCs
    

    [Header("Bullet Variables")]
    public bool readyToFire;
    public float bulletSpeed;
    public float shellSpeed;
    public GameObject bulletSP, bulletPrefab, shellPrefab, shellSP;

    [Header("Animation/Audio Variables")]
    public GameObject cylinder;
    public ParticleSystem muzzleFlash, flash;
    public AudioSource revAudioSource;
    public AudioClip revolverFire;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = maxAmmo;
        readyToFire = true;
        cylinderOpen = false;
    }
    /// <summary>
    /// Handles the firing of the revolver when activated
    /// </summary>
    void Fire()
    {
        Debug.Log("fire called");
        readyToFire = false;
        currentAmmo--;

        //spawn bullet
        SpawnBullet();
        //muzzle flash
        MuzzleFlash();
        //playaudio
        revAudioSource.PlayOneShot(revolverFire);


    }

    /// <summary>
    /// Spawns a bullet at the bullet spawn point and applies velocity to it
    /// </summary>
    void SpawnBullet()
    {
        GameObject spawnedBullet = Instantiate(bulletPrefab);
        spawnedBullet.transform.position = bulletSP.transform.position;
        spawnedBullet.transform.rotation = UnityEngine.Quaternion.LookRotation(transform.forward);
        spawnedBullet.GetComponent<Rigidbody>().linearVelocity = transform.forward * bulletSpeed;
    }
    /// <summary>
    /// Starts the muzzle flash particle system and plays the muzzle flash sound
    /// </summary>
    void MuzzleFlash()
    {
        flash.Play();
        muzzleFlash.Play();
    }
    /// <summary>
    /// Spanws the empty shells when the gun is fired
    /// </summary>
    void SpawnShell()
    {
        if(currentAmmo <= 0)bullets[currentAmmo].SetActive(false);
        
        GameObject shell = Instantiate(shellPrefab);
        shell.transform.position = shellSP.transform.position;
        shell.GetComponent<Rigidbody>().AddForce(transform.up * shellSpeed, ForceMode.Impulse);
        shell.GetComponent<Rigidbody>().AddForce(transform.right * shellSpeed, ForceMode.Impulse);
        shell.GetComponent<Rigidbody>().AddTorque(transform.right * shellSpeed, ForceMode.Impulse);

    }
    /// <summary>
    /// signals that the animation has finished playing
    /// </summary>
    void finishAnimation()
    {
        Debug.Log("resetFire");
        readyToFire = true;
        // bullets[currentAmmo].SetActive(false);
        // bullets[maxAmmo - currentAmmo].SetActive(true);

    }
    /// <summary>
    /// Players the dry fire sound
    /// </summary>
    void revolverClick()
    {
        revAudioSource.PlayOneShot(revClick);
    }
    /// <summary>
    /// Handles the dry fire action when the gun is fired without ammo
    /// </summary>
    void DryFire()
    {
        readyToFire = false;
        revAudioSource.PlayOneShot(revolverDryFire);
    }
    /// <summary>
    /// Reloads the revolver by resetting the ammo count and activating all bullet game objects
    /// </summary>
    public void Reload()
    {
        Debug.Log("Reload called");
        currentAmmo = maxAmmo;
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].SetActive(true);
        }
        readyToFire = true;
    }
    /// <summary>
    /// Opens or closes the revolver's cylinder after button pressed on controller
    /// If the cylinder is already open, it closes it and sets readyToFire to true
    /// </summary>
    /// <param name="leftHanded"></param>
    public void OpenCylinder(bool leftHanded)
    {
        Debug.Log("managing cylinder");
        if (cylinderOpen)
        {
            //logic to close the cylinder since already open
            cylinder.transform.localPosition = new UnityEngine.Vector3(cylinder.transform.localPosition.x, cylinder.transform.localPosition.y, 0f);
            readyToFire = true;
            cylinderOpen = false;
            return;
        }
        readyToFire = false;
        cylinderOpen = true;
        Debug.Log("Open Cylinder called");
        if (leftHanded) cylinder.transform.localPosition = new UnityEngine.Vector3(cylinder.transform.localPosition.x, cylinder.transform.localPosition.y, 0.03f);
        else cylinder.transform.localPosition = new UnityEngine.Vector3(cylinder.transform.localPosition.x, cylinder.transform.localPosition.y, -0.03f);


    }

    /// <summary>
    /// Holsters the revolver by resetting the cylinder position
    /// Set top reload upon being holstered
    /// </summary>
    public void Holster()
    {
        Debug.Log("Holster called");
        Reload();
        cylinder.transform.localPosition = new UnityEngine.Vector3(cylinder.transform.localPosition.x, cylinder.transform.localPosition.y, 0f);


    }
    /// <summary>
    /// Unholsters the revolver
    /// </summary>
    public void UnHolster()
    {
        Debug.Log("UnHolster called");
        //was turned off not sure why i re enabled it as of 7/20/25 if problem turn it back off
        readyToFire = true;
    }
    public void SetOwner(GunOwner newOwner)
    {
        owner = newOwner;
        if (owner == GunOwner.Player)
        {
            // Reset ammo for player, enable reload, etc.
            currentAmmo = maxAmmo;
            readyToFire = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            // NPC: infinite ammo, ready to fire
            maxAmmo = int.MaxValue; // NPCs can have infinite ammo
            currentAmmo = maxAmmo; // Set to max ammo for NPCs
            readyToFire = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true; // Make the revolver kinematic for NPCs
        }
    }
}
