using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject _mainMenuPanel;

    [Header("Buttons")]
    [SerializeField] private Button _playButton;

    private void Awake()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        _mainMenuPanel.SetActive(false);
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.StartGame();
        }
    }
}
