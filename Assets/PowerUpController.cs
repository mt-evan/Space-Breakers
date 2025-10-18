using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public enum PowerUpType { Pierce, Shield, Freeze }
    public PowerUpType type;

    public float fallSpeed = 3f;
    public float boundary = -15f;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < boundary)
        {
            Destroy(gameObject);
        }
    }
}
