using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 5f;
    public float boundary = -6f;

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;

        if (transform.position.y < boundary)
        {
            Destroy(gameObject);
        }
    }
}

