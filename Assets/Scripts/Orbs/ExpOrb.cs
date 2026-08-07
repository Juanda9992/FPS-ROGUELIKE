using UnityEngine;

public class ExpOrb : OrbBase
{
    private void Awake()
    {
        orbType = OrbType.Experience;
    }

    protected override void ApplyEffect(GameObject player)
    {
        PlayerExpManager expManager = player.GetComponent<PlayerExpManager>();
        if (expManager != null)
        {
            expManager.AddExperience(valueAmount);
        }
    }
}
