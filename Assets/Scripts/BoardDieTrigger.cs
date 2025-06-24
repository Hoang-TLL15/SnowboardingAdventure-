using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class BoardDieTrigger : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem collisionEffectPrefab; // Đổi thành prefab

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
        yield return new WaitForSeconds(0.5f); // Chờ 1 giây trước khi dừng game
        Time.timeScale = 0f; // Dừng game sau 1 giây

        yield return new WaitForSecondsRealtime(1f); // Tiếp tục chờ 1 giây (không bị ảnh hưởng bởi timeScale)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowEndGamePanelPublic();
        }
    }
}