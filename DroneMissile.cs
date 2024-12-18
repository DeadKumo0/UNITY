using UnityEngine;
public class DroneMissile : MonoBehaviour
{
    public GameObject missilePrefab;
    public float missileSpeed = 20f; 
    public Transform missileSpawnPoint; 

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            LaunchMissile();
        }
    }

    void LaunchMissile()
    {
        if (missilePrefab != null && missileSpawnPoint != null)
        {
            Debug.Log("Launching missile...");
            GameObject missile = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation);
            Rigidbody missileRb = missile.GetComponent<Rigidbody>();
            missileRb.linearVelocity = missileSpawnPoint.forward * missileSpeed;
        }
        else
        {
            Debug.LogError("MissilePrefab or MissileSpawnPoint is missing!");
        }
    }
}
