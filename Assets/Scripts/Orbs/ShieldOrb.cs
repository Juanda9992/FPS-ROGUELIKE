using UnityEngine;

public class ShieldOrb : OrbBase
{
    private void Awake()
    {
        orbType = OrbType.Shield;
    }

    protected override bool CanBePickedUp(GameObject player)
    {
        if (!consumeOnlyIfNeeded) return true;

        PlayerHealthController healthController = player.GetComponent<PlayerHealthController>();
        if (healthController != null)
        {
            return healthController.CurrentShield < healthController.MaxShield;
        }

        return true;
    }

    protected override void ApplyEffect(GameObject player)
    {
        PlayerHealthController healthController = player.GetComponent<PlayerHealthController>();
        if (healthController != null)
        {
            healthController.RestoreShield(ValueAmount);
        }
    }
}
