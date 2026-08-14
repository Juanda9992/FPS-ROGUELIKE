using UnityEngine;

public class EnemyFollow : MonoBehaviour, ISlowable, IStuneable, IPusheable, IBlindable
{
    private Transform player;
    public float speed = 3f;
    public float rotationSpeed = 5f;
    [SerializeField] private Rigidbody rb;
    private Vector3 pushVelocity;
    [SerializeField] private float pushDecay = 5f;

    [SerializeField] private bool isBlind = false;
    public bool IsBlind => isBlind;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (pushVelocity.sqrMagnitude > 0.0001f)
        {
            transform.position += pushVelocity * Time.deltaTime;
            pushVelocity = Vector3.Lerp(pushVelocity, Vector3.zero, pushDecay * Time.deltaTime);
        }

        if (player == null || isBlind)
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

    public void ApplyStunEffect(float duration)
    {
        speed = 0f;
        CancelInvoke(nameof(RemoveStunEffect));
        Invoke(nameof(RemoveStunEffect), duration);
    }

    public void RemoveStunEffect()
    {
        speed = 3f;
    }

    public void Push(Vector3 center, float strenght, bool attract = false)
    {
        Vector3 pushDirection = attract ? (center - transform.position) : (transform.position - center);
        if (pushDirection.sqrMagnitude < 0.0001f)
        {
            pushDirection = attract ? -transform.forward : transform.forward;
        }
        else
        {
            pushDirection.Normalize();
        }
        rb.AddForce(pushDirection * strenght, ForceMode.Impulse);
    }

    public void Blind(float duration)
    {
        isBlind = true;
        CancelInvoke(nameof(UnBlind));
        Invoke(nameof(UnBlind), duration);
    }

    public void UnBlind()
    {
        isBlind = false;
    }
}