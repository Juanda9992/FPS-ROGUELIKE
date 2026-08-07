using UnityEngine;

public class HealthOrb : OrbBase
{
    private void Awake()
    {
        orbType = OrbType.Health;
    }

    protected override bool CanBePickedUp(GameObject player)
    {
        if (!consumeOnlyIfNeeded) return true;

        PlayerHealthController healthController = player.GetComponent<PlayerHealthController>();
        if (healthController != null)
        {
            return healthController.Health < healthController.MaxHealth;
        }

        return true;
    }

    protected override void ApplyEffect(GameObject player)
    {
        PlayerHealthController healthController = player.GetComponent<PlayerHealthController>();
        if (healthController != null)
        {
            healthController.OnHealthRestored(valueAmount);
        }
    }
}
