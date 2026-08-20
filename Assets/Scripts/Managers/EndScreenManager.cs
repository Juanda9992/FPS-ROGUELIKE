using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public static EndScreenManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject _endScreenPanel;

    [Header("Stats Texts")]
    [SerializeField] private TextMeshProUGUI _timeSurvivedText;
    [SerializeField] private TextMeshProUGUI _enemiesKilledText;
    [SerializeField] private TextMeshProUGUI _damageTakenText;
    [SerializeField] private TextMeshProUGUI _damageDealtText;

    [Header("Buttons")]
    [SerializeField] private Button _restartButton;

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

        _endScreenPanel.SetActive(false);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void Start()
    {
        GameEventsManager.Instance.OnPlayerDied += ShowEndScreen;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.OnPlayerDied -= ShowEndScreen;
    }

    private void OnDestroy()
    {
        _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
    }

    public void ShowEndScreen()
    {
        Time.timeScale = 0f;
        CursorManager.SetCursorVisible(true);

        int totalSeconds = TimeManager.Instance.TotalSeconds;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        _timeSurvivedText.text = $"Time survived: {string.Format("{0:00}:{1:00}", minutes, seconds)}";
        _enemiesKilledText.text = $"Enemies killed: {GameStatsManager.Instance.EnemiesKilled}";
        _damageTakenText.text = $"Damage taken: {GameStatsManager.Instance.DamageTaken}";
        _damageDealtText.text = $"Damage dealt: {GameStatsManager.Instance.DamageDealtToEnemies}";

        _endScreenPanel.SetActive(true);
    }

    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
