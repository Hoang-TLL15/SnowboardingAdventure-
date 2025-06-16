using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour
{
    // Assign this in the Inspector with your particle effect prefab
    public ParticleSystem finishParticleEffect;
    [SerializeField] AudioSource audioSource;

    void Start()
    {

    }

    void Update()
    {

    }

    // Khi có va chạm trigger với object có tag "Player", in ra "Finish" và dừng game sau 2 giây
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Finish");

            // Trigger the particle effect at the finish line's position
            if (finishParticleEffect != null)
            {
                finishParticleEffect.Play();
            }

            // Play finish sound
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Calculate score and finish
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Finish();
            }

            StartCoroutine(StopGameAfterDelay());
        }
    }

    private IEnumerator StopGameAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f; // Dừng game
    }
}