using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class OrbBase : MonoBehaviour
{
    [Header("Orb Configuration")]
    [SerializeField] protected OrbType orbType;
    [SerializeField] protected int valueAmount = 20;
    [SerializeField] protected string playerTag = "Player";
    [SerializeField] protected bool consumeOnlyIfNeeded = false;

    [Header("Visual Feedback & Animation")]
    [SerializeField] private bool enableBobbing = true;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.2f;

    [SerializeField] private bool enableRotation = true;
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Magnet / Attraction")]
    [SerializeField] private bool enableMagnet = true;
    [SerializeField] private float magnetRadius = 5f;
    [SerializeField] private float magnetSpeed = 8f;

    [Header("Audio & Visual Effects")]
    [SerializeField] private GameObject pickupEffectPrefab;
    [SerializeField] private Color orbColor;

    [SerializeField] private Rigidbody rb;
    public OrbType Type => orbType;
    public int ValueAmount => valueAmount;

    private Vector3 startPosition;
    private Transform playerTransform;

    protected virtual void Start()
    {
        startPosition = transform.position;

        GetComponent<Renderer>().material.color = orbColor;
    }

    protected virtual void Update()
    {
        //HandleAnimation();
        HandleMagnet();
    }

    private void HandleAnimation()
    {
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (enableRotation)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private void HandleMagnet()
    {
        if (!enableMagnet) return;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                playerTransform = player.transform;
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= magnetRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position - new Vector3(0, 0.5f, 0), magnetSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject playerObj = GetPlayerObject(other);
        if (playerObj != null)
        {
            TryCollect(playerObj);
        }
    }

    protected virtual GameObject GetPlayerObject(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            return other.gameObject;
        }

        // Fallback: check parent in case collider is on a child object
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(playerTag))
        {
            return other.attachedRigidbody.gameObject;
        }

        return null;
    }

    public bool TryCollect(GameObject player)
    {
        if (!CanBePickedUp(player))
        {
            return false;
        }

        ApplyEffect(player);
        OnPickedUp();
        return true;
    }

    /// <summary>
    /// Evaluates if the orb can be collected by the player.
    /// Can be overridden by subclasses (e.g. check if HP or Shield is not full).
    /// </summary>
    protected virtual bool CanBePickedUp(GameObject player)
    {
        return true;
    }

    /// <summary>
    /// Concrete effect applied to the player on collection.
    /// </summary>
    protected abstract void ApplyEffect(GameObject player);

    protected virtual void OnPickedUp()
    {
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (enableMagnet)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, magnetRadius);
        }
    }

    public Rigidbody GetRb()
    {
        return rb;
    }
}
