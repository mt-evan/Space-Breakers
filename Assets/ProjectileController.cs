using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 5f;
    public float boundary = -15f; // y level for when the projectile self-destructs

    // Update is called once per frame
    void Update()
    {
        // Move the projectile straight down
        transform.position += Vector3.down * speed * Time.deltaTime;

        // Destroy the projectile if it reaches the boundary
        if (transform.position.y < boundary)
        {
            Destroy(gameObject);
        }
    }
}
