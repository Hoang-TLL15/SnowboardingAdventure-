using UnityEngine;
using UnityEngine.Audio;

public class BoardDieTrigger : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rock"))
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