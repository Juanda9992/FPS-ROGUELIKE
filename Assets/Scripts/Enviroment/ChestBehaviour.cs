using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class ChestBehaviour : MonoBehaviour, IInteractable
{
    [Header("Visuals & Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _openAnimationTrigger = "Open";

    [Header("Events")]
    [SerializeField] private UnityEvent _onChestOpened;

    private PlayerInputActions _input;
    private bool _isPlayerInRange;
    private bool _isOpened;

    public bool IsPlayerInRange => _isPlayerInRange;
    public bool IsOpened => _isOpened;
    public bool CanInteract => !_isOpened;

    public event Action OnChestOpened;

    private void Awake()
    {
        _input = new PlayerInputActions();
        _input.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.Player.Interact.performed -= OnInteractPerformed;
            _input.Dispose();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isOpened)
        {
            return;
        }

        if (other.CompareTag("Player") || other.TryGetComponent<FPSController>(out _))
        {
            _isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.TryGetComponent<FPSController>(out _))
        {
            _isPlayerInRange = false;

        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInRange)
        {
            return;
        }

        if (_isOpened)
        {
            return;
        }

        Interact();
    }

    public void Interact()
    {
        if (_isOpened)
        {
            return;
        }

        OpenChest();
    }

    public void OpenChest()
    {
        _isOpened = true;

        if (_animator != null && !string.IsNullOrEmpty(_openAnimationTrigger))
        {
            _animator.SetTrigger(_openAnimationTrigger);
        }

        AmuletCreator.Instance.CreateRandomAmulet();

        transform.DOScale(0f, 1f).OnComplete(() => Destroy(gameObject));

        _onChestOpened?.Invoke();
        OnChestOpened?.Invoke();

        Debug.Log("[ChestBehaviour] Chest opened and Amulet creation initiated.");
    }
}
