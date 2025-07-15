using System.Numerics;
using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public class Revolver : MonoBehaviour
{
    public bool readyToFire;
    public int maxAmmo = 6;
    public int currentAmmo;
    public float bulletSpeed;
    public float shellSpeed;
    public GameObject bulletSP, bulletPrefab, shellPrefab, shellSP;
    public GameObject[] bullets;
    public GameObject cylinder;
    public ParticleSystem muzzleFlash, flash;
    public AudioSource revAudioSource;
    public AudioClip revolverFire, revolverDryFire, revClick;
    private bool cylinderOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = maxAmmo;
        readyToFire = true;
        cylinderOpen = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

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
    void SpawnBullet()
    {
        GameObject spawnedBullet = Instantiate(bulletPrefab);
        spawnedBullet.transform.position = bulletSP.transform.position;
        spawnedBullet.transform.rotation = UnityEngine.Quaternion.LookRotation(transform.forward);
        spawnedBullet.GetComponent<Rigidbody>().linearVelocity = transform.forward * bulletSpeed;
    }
    void MuzzleFlash()
    {
        flash.Play();
        muzzleFlash.Play();
    }

    void SpawnShell()
    {
        bullets[currentAmmo].SetActive(false);
        GameObject shell = Instantiate(shellPrefab);
        shell.transform.position = shellSP.transform.position;
        shell.GetComponent<Rigidbody>().AddForce(transform.up * shellSpeed, ForceMode.Impulse);
        shell.GetComponent<Rigidbody>().AddForce(transform.right * shellSpeed, ForceMode.Impulse);
        shell.GetComponent<Rigidbody>().AddTorque(transform.right * shellSpeed, ForceMode.Impulse);

    }
    void finishAnimation()
    {
        Debug.Log("resetFire");
        readyToFire = true;
        // bullets[currentAmmo].SetActive(false);
        // bullets[maxAmmo - currentAmmo].SetActive(true);

    }
    void revolverClick()
    {
        revAudioSource.PlayOneShot(revClick);
    }
    void DryFire()
    {
        readyToFire = false;
        revAudioSource.PlayOneShot(revolverDryFire);
    }
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
    
    public void Holster()
    {
        Debug.Log("Holster called");
        Reload();
        cylinder.transform.localPosition = new UnityEngine.Vector3(cylinder.transform.localPosition.x, cylinder.transform.localPosition.y, 0f);


    }
    public void UnHolster()
    {
        Debug.Log("UnHolster called");
        //readyToFire = true;
    }
}
