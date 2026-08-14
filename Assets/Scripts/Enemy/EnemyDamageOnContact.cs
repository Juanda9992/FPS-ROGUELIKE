using UnityEngine;

public class EnemyDamageOnContact : MonoBehaviour
{
    [SerializeField] private float damageDistance = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRate = 1f;

    private Transform player;
    private PlayerHealthController playerHealth;
    private IBlindable blindable;

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
        if (player == null || playerHealth == null) 
        {
            return;
        }

        if (blindable != null && blindable.IsBlind)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= damageDistance)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackRate)
            {
                playerHealth.TakeDamage(damage);
                attackTimer = 0f;
            }
        }
    }
}