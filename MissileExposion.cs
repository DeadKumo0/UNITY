using UnityEngine;


public class MissileExplosion : MonoBehaviour
{
    public GameObject explosionEffect; 
    public float explosionRadius = 5f; 
    public float explosionForce = 700f; 
    public float explosionDuration = 9f; 

    void OnCollisionEnter(Collision collision)
    {
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);

        Destroy(gameObject);

        Destroy(explosion, explosionDuration);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }
}