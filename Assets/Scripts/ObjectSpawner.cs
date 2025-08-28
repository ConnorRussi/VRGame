using UnityEngine;

public class ObjectSpawner : MonoBehaviour, IButtonInteractor
{

    public GameObject spawnObjectPrefab;
    //public float spawnDelay;
    
    public Transform spawnPoint;
    public AudioSource audioSource;
    public AudioClip spawnSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void IButtonInteractor.Activate()
    {
        //Spawns the object
        audioSource.PlayOneShot(spawnSound);
        Instantiate(spawnObjectPrefab, spawnPoint.position, spawnPoint.rotation);

    }
}
