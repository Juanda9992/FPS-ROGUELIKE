using TMPro;
using UnityEngine;

public class TimeManagerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeManager _timeManager;
    [SerializeField] private TextMeshProUGUI _timerText;

    private void OnEnable()
    {
        _timeManager.OnSecondElapsed += UpdateTimerUI;
        UpdateTimerUI(_timeManager.TotalSeconds);
    }

    private void OnDisable()
    {
        if (_timeManager != null)
        {
            _timeManager.OnSecondElapsed -= UpdateTimerUI;
        }
    }

    private void UpdateTimerUI(int totalSeconds)
    {
        if (_timerText != null)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
