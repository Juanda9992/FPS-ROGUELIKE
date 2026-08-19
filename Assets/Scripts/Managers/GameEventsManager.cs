using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public event Action OnGameStarted;

    [Header("Debug / State")]
    [SerializeField] private bool _isGameStarted;

    public bool IsGameStarted
    {
        get => _isGameStarted;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!_isGameStarted)
        {
            CursorManager.SetCursorVisible(true);
        }
    }

    public void StartGame()
    {
        if (_isGameStarted)
        {
            return;
        }

        _isGameStarted = true;
        OnGameStarted?.Invoke();
    }
}
