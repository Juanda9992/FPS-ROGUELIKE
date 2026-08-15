using UnityEngine;

public class EnemyDamageOnContact : MonoBehaviour, ISilenceable
{
    [SerializeField] private float damageDistance = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRate = 1f;

    private Transform player;
    private PlayerHealthController playerHealth;
    private IBlindable blindable;

    [SerializeField] private bool isSilenced = false;
    public bool IsSilenced => isSilenced;

    private float attackTimer;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealthController>();
        }

        blindable = GetComponent<IBlindable>();
    }

    private void Update()
    {
        if (player == null || playerHealth == null || isSilenced)
        {
            return;
        }

        if (blindable != null && blindable.IsBlind)
        {
            return;
        }

        float damageDistSqr = damageDistance * damageDistance;
        if ((transform.position - player.position).sqrMagnitude <= damageDistSqr)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackRate)
            {
                playerHealth.TakeDamage(damage);
                attackTimer = 0f;
            }
        }
    }

    public void Silence(float duration)
    {
        isSilenced = true;
        CancelInvoke(nameof(UnSilence));
        Invoke(nameof(UnSilence), duration);
    }

    public void UnSilence()
    {
        isSilenced = false;
    }
}