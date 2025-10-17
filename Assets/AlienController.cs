using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    // This will all be set by AlienSwarmController when an alien is spawned
    [HideInInspector] // Hides this from the Inspector because the AlienSwarmController will take care of it
    public GameObject projectilePrefab;

    public float minFireDelay = 3.0f;
    public float maxFireDelay = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireRepeatedly());
    }

    IEnumerator FireRepeatedly()
    {
        // Loop forever as long as the alien is alive
        while (true)
        {
            float fireDelay = Random.Range(minFireDelay, maxFireDelay);
            yield return new WaitForSeconds(fireDelay);

            // Check if the projectile prefab has been assigned before firing again
            if (projectilePrefab != null )
            {
                Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
