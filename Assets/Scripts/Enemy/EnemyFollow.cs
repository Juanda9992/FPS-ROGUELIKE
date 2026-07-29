using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;      // Referencia al jugador
    public float speed = 3f;      // Velocidad de movimiento
    public float rotationSpeed = 5f;

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
}