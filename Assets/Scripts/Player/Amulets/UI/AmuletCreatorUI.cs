using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmuletCreatorUI : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _amuletImage;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _statDescriptionText;


    [SerializeField] private GameObject _creatorPanel;
    [SerializeField] private Button _selectButton;

    private AmuletInstance _currentAmuletInstance;

    private void Awake()
    {
        _creatorPanel.SetActive(false);
        _selectButton.onClick.AddListener(OnSelectAmuletClicked);
    }
    public void SetUpVisuals(AmuletInstance amuletInstance)
    {
        Time.timeScale = 0f;
        CursorManager.SetCursorVisible(true);
        _creatorPanel.SetActive(true);

        _currentAmuletInstance = amuletInstance;

        if (_currentAmuletInstance == null || _currentAmuletInstance.Data == null)
        {
            return;
        }

        _titleText.text = _currentAmuletInstance.Data.DisplayName;
        _amuletImage.sprite = _currentAmuletInstance.Data.Icon;
        _descriptionText.text = _currentAmuletInstance.GetFormattedDescription();
        _statDescriptionText.text = _currentAmuletInstance.GetFormattedStatsDescription();
    }

    public void OnSelectAmuletClicked()
    {
        if (_currentAmuletInstance != null)
        {
            CursorManager.SetCursorVisible(false);
            Time.timeScale = 1f;
            AmuletCreator.Instance.SelectAmulet(_currentAmuletInstance);
        }
    }
}
