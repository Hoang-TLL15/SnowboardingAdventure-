using UnityEngine;
using System.Collections;

public class Die : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem collisionEffectPrefab;

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

        if (collisionEffectPrefab != null)
        {
            // Tạo hiệu ứng tại vị trí trigger
            ParticleSystem effect = Instantiate(
                collisionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        Debug.Log("Die");
        StartCoroutine(ShowPanelWithDelay());
    }

    private IEnumerator ShowPanelWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Chờ 0.5 giây trước khi dừng game
        Time.timeScale = 0f; // Dừng game sau 0.5 giây

        yield return new WaitForSecondsRealtime(2f); // Delay 2 giây (không bị ảnh hưởng bởi Time.timeScale)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowEndGamePanelPublic();
        }
    }
}