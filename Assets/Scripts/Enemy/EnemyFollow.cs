using UnityEngine;

public class EnemyFollow : MonoBehaviour, ISlowable, IStuneable, IPusheable, IBlindable
{
    [Header("Movement Settings")]
    [SerializeField] private float _baseSpeed = 3f;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("Physics & Push Settings")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _pushDecay = 5f;

    [Header("Status")]
    [SerializeField] private bool _isBlind = false;

    private Vector3 _pushVelocity;

    public float BaseSpeed
    {
        get => _baseSpeed;
    }

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public float RotationSpeed
    {
        get => _rotationSpeed;
        set => _rotationSpeed = value;
    }

    public bool IsBlind
    {
        get => _isBlind;
    }

    public void InitializeSpeed(float speed)
    {
        _baseSpeed = speed;
        _speed = speed;
    }

    public void TickMovement(float deltaTime, Vector3 playerPosition)
    {
        if (_pushVelocity.sqrMagnitude > 0.0001f)
        {
            transform.position += _pushVelocity * deltaTime;
            _pushVelocity = Vector3.Lerp(_pushVelocity, Vector3.zero, _pushDecay * deltaTime);
        }

        if (_isBlind)
        {
            return;
        }

        Vector3 direction = (playerPosition - transform.position).normalized;

        transform.position += direction * _speed * deltaTime;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationSpeed * deltaTime);
        }
    }

    public void ApplySlowEffect(float duration, float strength)
    {
        _speed *= strength;
        CancelInvoke(nameof(RemoveSlowEffect));
        Invoke(nameof(RemoveSlowEffect), duration);
    }

    public void RemoveSlowEffect()
    {
        _speed = _baseSpeed;
    }

    public void ApplyStunEffect(float duration)
    {
        _speed = 0f;
        CancelInvoke(nameof(RemoveStunEffect));
        Invoke(nameof(RemoveStunEffect), duration);
    }

    public void RemoveStunEffect()
    {
        _speed = _baseSpeed;
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

        if (_rb != null)
        {
            _rb.AddForce(pushDirection * strenght, ForceMode.Impulse);
        }
    }

    public void Blind(float duration)
    {
        _isBlind = true;
        CancelInvoke(nameof(UnBlind));
        Invoke(nameof(UnBlind), duration);
    }

    public void UnBlind()
    {
        _isBlind = false;
    }
}