using UnityEngine;
using System.Collections;

public class AlienController : MonoBehaviour
{
    [HideInInspector]
    public GameObject projectilePrefab;
    [HideInInspector]
    public float projectileSpeed;

    // These are now set by the AlienSwarmController
    [HideInInspector] public float minFireDelay = 3.0f;
    [HideInInspector] public float maxFireDelay = 20.0f;

    IEnumerator Start()
    {
        // Wait for a small, unique, and random delay
        // This staggers each alien's firing timer so they don't all start at once
        yield return new WaitForSeconds(Random.Range(0.0f, 2.0f));
        StartCoroutine(FireRepeatedly());
    }

    IEnumerator FireRepeatedly()
    {
        // This will loop forever for the lifetime of this alien.
        while (true)
        {
            // Wait for the normal firing delay for all subsequent shots.
            float delay = Random.Range(minFireDelay, maxFireDelay);
            yield return new WaitForSeconds(delay);

            FireShot();
        }
    }

    void FireShot()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<ProjectileController>().speed = this.projectileSpeed;
        }
    }
}

