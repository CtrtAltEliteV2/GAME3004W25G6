using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sound Clips")]
    public AudioClip walkingSound;
    public AudioClip hitSound;
    public AudioClip destroySound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip healthPickupSound;

    [Header("Music Tracks")]
    public AudioClip sceneMusic;
    public bool loopMusic = false;
    public float musicVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayMusic(sceneMusic, loopMusic);  // Play the scene music when the scene starts
    }

    void Update()
    {
        // Update the volume during runtime if the volume value changes
        audioSource.volume = musicVolume;
    }

    public void PlayWalkingSound()
    {
        audioSource.PlayOneShot(walkingSound);
    }

    public void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }
    
    public void PlayDestroySound()
    {
        audioSource.PlayOneShot(destroySound);
    }

    public void PlayDamageSound()
    {
        audioSource.PlayOneShot(damageSound);
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }

    public void PlayHealthPickupSound()
    {
        audioSource.PlayOneShot(healthPickupSound);
    }

    public void PlayMusic(AudioClip musicClip, bool loop)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = musicClip;  // Set the music clip
        audioSource.loop = loop;  // Set whether it loops or not
        audioSource.volume = musicVolume;  // Set the volume from the inspector
        audioSource.Play();
    }
    
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
