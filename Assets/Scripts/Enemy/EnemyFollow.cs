using UnityEngine;

public class EnemyFollow : MonoBehaviour, ISlowable
{
    private Transform player;
    public float speed = 3f;
    public float rotationSpeed = 5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    public void ApplySlowEffect(float duration, float strength)
    {
        speed *= strength;
        CancelInvoke(nameof(RemoveSlowEffect));
        Invoke(nameof(RemoveSlowEffect), duration);
    }

    public void RemoveSlowEffect()
    {
        speed = 3f;
    }
}