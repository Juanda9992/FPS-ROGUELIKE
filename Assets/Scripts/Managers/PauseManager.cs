using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public event Action<bool> OnPauseChanged;
    
    private List<IPausable> _pausables;
    private bool _isPaused;
    private PlayerInputActions _input;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _pausables = new List<IPausable>();
            
            _input = new PlayerInputActions();
            _input.Player.Pause.performed += OnPauseInputPerformed;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (_input != null)
        {
            _input.Enable();
        }
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.Disable();
        }
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.Player.Pause.performed -= OnPauseInputPerformed;
            _input.Dispose();
        }
    }

    private void OnPauseInputPerformed(InputAction.CallbackContext context)
    {
        if (GameEventsManager.Instance != null && !GameEventsManager.Instance.IsGameStarted)
        {
            return;
        }

        TogglePause();
    }

    public void Register(IPausable pausable)
    {
        if (pausable != null && !_pausables.Contains(pausable))
        {
            _pausables.Add(pausable);
        }
    }

    public void Unregister(IPausable pausable)
    {
        if (pausable != null && _pausables.Contains(pausable))
        {
            _pausables.Remove(pausable);
        }
    }

    public void TogglePause()
    {
        SetPause(!_isPaused);
    }

    public void SetPause(bool pauseState)
    {
        _isPaused = pauseState;
        Time.timeScale = _isPaused ? 0f : 1f;

        CursorManager.SetCursorVisible(_isPaused);

        for (int i = 0; i < _pausables.Count; i++)
        {
            if (_pausables[i] != null)
            {
                if (_isPaused)
                {
                    _pausables[i].OnPause();
                }
                else
                {
                    _pausables[i].OnResume();
                }
            }
        }

        OnPauseChanged?.Invoke(_isPaused);
    }
}
