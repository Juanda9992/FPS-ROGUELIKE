using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action<int> OnSecondElapsed;
    public event Action<int> OnMinuteElapsed;

    private float _timer;
    private int _totalSeconds;

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

    private void Update()
    {
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
