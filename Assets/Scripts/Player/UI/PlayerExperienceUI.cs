using UnityEngine;
using DG.Tweening;
public class PlayerExperienceUI : MonoBehaviour
{
    [SerializeField] private PlayerExpManager playerExpManager;
    [SerializeField] private UnityEngine.UI.Image expFillImage;

    private void OnEnable()
    {
        playerExpManager.OnExpChanged += UpdateExpUI;
    }

    private void OnDisable()
    {
        playerExpManager.OnExpChanged -= UpdateExpUI;
    }

    private void UpdateExpUI(int currentExp)
    {
        float fillAmount = (float)currentExp / playerExpManager.expToNextLevel;
        expFillImage.DOFillAmount(fillAmount, 0.5f).SetEase(Ease.InOutQuad);
    }
}
