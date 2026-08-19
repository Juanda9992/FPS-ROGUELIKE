using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<int> OnSecondElapsed;
    public event Action<int> OnMinuteElapsed;

    private float _timer;
    private int _totalSeconds;
    private bool _isGameStarted;

    public int TotalSeconds
    {
        get => _totalSeconds;
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
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
            if (GameEventsManager.Instance.IsGameStarted)
            {
                HandleGameStarted();
            }
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
        }
    }

    private void HandleGameStarted()
    {
        _timer = 0f;
        _totalSeconds = 0;
        _isGameStarted = true;
        OnSecondElapsed?.Invoke(_totalSeconds);
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= 1f)
        {
            _timer -= 1f;
            _totalSeconds++;

            OnSecondElapsed?.Invoke(_totalSeconds);

            if (_totalSeconds % 60 == 0)
            {
                int minutes = _totalSeconds / 60;
                OnMinuteElapsed?.Invoke(minutes);
            }
        }
    }
}
