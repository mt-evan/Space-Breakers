using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ForceFieldEffect : MonoBehaviour
{
    [Tooltip("How fast the force field pulses")]
    public float pulseSpeed = 1.0f;

    [Tooltip("The minimum brightness/alpha of the pulse")]
    [Range(0, 1)]
    public float minAlpha = 0.5f;

    [Tooltip("The maximum brightness/alpha of the pulse")]
    [Range(0, 1)]
    public float maxAlpha = 1.0f;

    private SpriteRenderer spriteRenderer;
    private Material materialInstance;
    private Color baseColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // get an instance of the material so each wall can pulse independently
        materialInstance = spriteRenderer.material;
        baseColor = materialInstance.color;
    }

    void Update()
    {
        // using Mathf.PingPong to create a smooth alternating value between min and max
        float targetAlpha = minAlpha + Mathf.PingPong(Time.time * pulseSpeed, maxAlpha - minAlpha);

        Color newColor = baseColor;
        newColor.a = targetAlpha;
        materialInstance.color = newColor;
    }
}
