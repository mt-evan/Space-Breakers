using UnityEngine;
using System.Collections;

public class AlienController : MonoBehaviour
{
    [HideInInspector]
    public GameObject projectilePrefab;
    [HideInInspector]
    public float projectileSpeed;

    public float minFireDelay = 3.0f;
    public float maxFireDelay = 10.0f;

    public float firstShotMinDelay = 0.5f;
    public float firstShotMaxDelay = 2.0f;

    void Start()
    {
        StartCoroutine(FireRepeatedly());
    }

    IEnumerator FireRepeatedly()
    {
        float initialDelay = Random.Range(firstShotMinDelay, firstShotMaxDelay);
        yield return new WaitForSeconds(initialDelay);
        FireShot();

        while (true)
        {
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

