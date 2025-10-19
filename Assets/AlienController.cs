using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlienController : MonoBehaviour
{
    [HideInInspector] public GameObject projectilePrefab;
    [HideInInspector] public float projectileSpeed;
    [HideInInspector] public float minFireDelay = 3.0f;
    [HideInInspector] public float maxFireDelay = 20.0f;

    [HideInInspector] public List<GameObject> powerUpPrefabs;
    public float powerUpDropChance = 0.1f; // 10% chance for an alien to drop a power-up
    private Coroutine firingCoroutine;

    IEnumerator Start()
    {
        // Wait for a small, unique, and random delay
        // This staggers each alien's firing timer so they don't all start at once
        yield return new WaitForSeconds(Random.Range(0.0f, 2.0f));
        firingCoroutine = StartCoroutine(FireRepeatedly());
    }

    public void StopFiring()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }
    }

    public void ResumeFiring()
    {
        // Don't start a new one if it's already running
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }
        firingCoroutine = StartCoroutine(FireRepeatedly());
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

    private void OnDestroy()
    {
        // Check if the game is closing or else potential error could occur
        if (GameManager.instance != null)
        {
            // Check if we should drop a power-up
            if (Random.value < powerUpDropChance)
            {
                DropPowerUp();
            }
        }
    }

    void DropPowerUp()
    {
        // Checks that the list of power-ups is valid, maybe this check is not needed
        if (powerUpPrefabs != null && powerUpPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, powerUpPrefabs.Count);
            GameObject chosenPowerUp = powerUpPrefabs[randomIndex];
            Instantiate(chosenPowerUp, transform.position, Quaternion.identity);
        }
    }
}
