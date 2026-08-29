using UnityEngine;

public class ExpOrb : OrbBase
{
    private void Awake()
    {
        orbType = OrbType.Experience;
    }

    protected override void ApplyEffect(GameObject player)
    {
        PlayerExpManager expManager = player.GetComponentInParent<PlayerExpManager>();
        if (expManager != null)
        {
            expManager.AddExperience(ValueAmount);
        }
    }
}
