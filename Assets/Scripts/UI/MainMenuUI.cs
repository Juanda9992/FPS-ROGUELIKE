using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private LoadoutPanelUI _loadoutPanelUI;

    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _loadoutButton;

    private void Awake()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
        _loadoutButton.onClick.AddListener(OnLoadoutButtonClicked);
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
        _loadoutButton.onClick.RemoveListener(OnLoadoutButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        _mainMenuPanel.SetActive(false);

        _loadoutPanelUI.ClosePanel();
        GameEventsManager.Instance.StartGame();
    }

    public void OnLoadoutButtonClicked()
    {
        _loadoutPanelUI.OpenPanel();
    }
}
