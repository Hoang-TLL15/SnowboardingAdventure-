using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            DieAction();
        }
    }

    // Call this method from other scripts to trigger the die action
    public void DieAction()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        Debug.Log("Die");
        Time.timeScale = 0f;
    }
}