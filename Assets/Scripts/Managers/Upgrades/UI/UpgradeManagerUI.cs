using UnityEngine;

public class UpgradeManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private RectTransform upgradeCardContainer;

    public void DisplayUpgrades(UpgradeData[] upgrades)
    {
        ClearUpgradeCards();

        foreach (var upgrade in upgrades)
        {
            GameObject card = Instantiate(upgradeCardPrefab, upgradeCardContainer);
            UpgradeCardUI cardUI = card.GetComponent<UpgradeCardUI>();
            cardUI.SetUpgradeData(upgrade);
        }
    }

    private void ClearUpgradeCards()
    {
        foreach (Transform child in upgradeCardContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
