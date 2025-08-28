using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletForce = 50f;
    public float destroyAfter = 5f;
    public LayerMask hitLayers;  // << LayerMask added

    private Vector3 previousPosition;
    private Rigidbody rb;
    public bool isHit = false;
    public bool debug = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        previousPosition = transform.position;
        Destroy(gameObject, destroyAfter);
    }

    void FixedUpdate()
    {
        // Raycast fallback using LayerMask
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        if (Physics.Raycast(previousPosition, direction.normalized, out RaycastHit hit, distance, hitLayers))
        {
            OnHit(hit.collider, hit.point);
        }

        previousPosition = currentPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only handle collisions with allowed layers
        if (((1 << collision.gameObject.layer) & hitLayers) != 0)
        {
            OnHit(collision.collider, collision.contacts[0].point);
        }
    }

    private void OnHit(Collider collider, Vector3 hitPoint)
    {
        if(isHit) return; // Prevent multiple hits
        isHit = true;
        if(debug)Debug.LogWarning("Bullet hit: " + collider.name);
        var cInteractable = collider.GetComponent<CInteractable>();
        if (cInteractable != null)
        {   if (cInteractable.breakAble)
            {
                cInteractable.Break();
            }
           
        }
        
        switch (LayerMask.LayerToName(collider.gameObject.layer))
        {
            //use for damage on objects or play a hitsound
            case "NPC":
            {
                NPC npc = collider.GetComponent<NPC>();
                // If the NPC is hit, apply damage
                npc.bulletHit(npc.bulletDamage);
                if(debug)Debug.Log("NPC hit by bullet, applying damage.");
                break;
            }
            case "Body":
            {
                Player player = collider.GetComponent<ColliderRelay>().relay<Player>();
                player.TakeDamage(player.bulletDamage);
                if(debug)Debug.Log("PlayerHead hit by bullet, applying damage.");
                break;
            }
            case "Building":
            {
                Register register = collider.GetComponent<ColliderRelay>().relay<Register>();
                if (register != null)
                    {
                        register.OpenRegister();
                       
                    }
                break;
            }
            default:
                if(debug)Debug.Log("Bullet hit an object with layer: " + collider.gameObject.layer);
                break;
        }
           
        
        Rigidbody targetRb = collider.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            targetRb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }
}
