using UnityEngine;

public class EnemyDamageOnContact : MonoBehaviour
{
    [SerializeField] private float damageDistance = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRate = 1f;

    private Transform player;
    private PlayerHealthController playerHealth;

    private float attackTimer;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealthController>();
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null) 
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= damageDistance)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackRate)
            {
                playerHealth.OnTakeDamage(damage);
                attackTimer = 0f;
            }
        }
    }
}