using UnityEngine;

public class DustTrail : MonoBehaviour
{
    [SerializeField] ParticleSystem dustParticles;
    [SerializeField] SurfaceEffector2D surfaceEffector;
    [SerializeField] AudioClip skiingSfx;

    AudioSource audioSource;

    bool isTouchingGround = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.clip = skiingSfx;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isTouchingGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isTouchingGround = false;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            if (dustParticles.isPlaying)
            {
                dustParticles.Stop();
            }
            return;
        }

        if (surfaceEffector != null && isTouchingGround && !Mathf.Approximately(surfaceEffector.speed, 0f))
        {
            if (!dustParticles.isPlaying)
            {
                dustParticles.Play();
                if (!audioSource.isPlaying && skiingSfx != null)
                {
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (dustParticles.isPlaying)
            {
                dustParticles.Stop();
            }
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}