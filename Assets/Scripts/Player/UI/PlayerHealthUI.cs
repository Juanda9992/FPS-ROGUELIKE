using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int current, int max)
    {
        healthSlider.value = (float)current / max;
    }
}