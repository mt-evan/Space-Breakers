using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip alienHitSound;
    public AudioClip playerBounceSound;
    public AudioClip levelClearSound;
    public AudioClip gameOverSound;
    public AudioClip playerDeathSound;
    public AudioClip powerUpSound;
    public AudioClip wallBounceSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAlienHit()
    {
        if (alienHitSound != null) audioSource.PlayOneShot(alienHitSound);
    }

    public void PlayPlayerBounce()
    {
        if (playerBounceSound != null) audioSource.PlayOneShot(playerBounceSound);
    }

    public void PlayLevelClear()
    {
        if (levelClearSound != null) audioSource.PlayOneShot(levelClearSound);
    }

    public void PlayGameOver()
    {
        if (gameOverSound != null) audioSource.PlayOneShot(gameOverSound);
    }

    public void PlayPlayerDeath()
    {
        if (playerDeathSound != null) audioSource.PlayOneShot(playerDeathSound);
    }

    public void PlayPowerUp()
    {
        if (powerUpSound != null) audioSource.PlayOneShot(powerUpSound);
    }

    public void PlayWallBounce()
    {
        if (wallBounceSound != null )
        {
            audioSource.PlayOneShot(wallBounceSound);
        }
    }
}
