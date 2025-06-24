using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    [SerializeField] private AudioSource collectSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add 50 points to the score
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Score += 50;
                GameManager.Instance.SendMessage("UpdateScoreUI", SendMessageOptions.DontRequireReceiver);
            }

            // Play sound at star position
            if (collectSound != null)
            {
                collectSound.Play();
            }

            // Destroy the star
            Destroy(gameObject);
        }
    }
}