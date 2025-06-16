using UnityEngine;
using UnityEngine.UI;

public class BoostCooldownUI : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Image cooldownIcon;

    void Update()
    {
        if (playerController == null || cooldownIcon == null) return;

        float cooldown = playerController.GetBoostCooldown();
        float maxCooldown = playerController.GetBoostCooldownMax();

        // Fill amount: 0 = empty, 1 = full (ready)
        cooldownIcon.fillAmount = 1f - (cooldown / maxCooldown);
    }
}