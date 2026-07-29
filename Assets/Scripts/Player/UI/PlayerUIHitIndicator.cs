using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerUIHitIndicator : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private Image hitIndicatorImage;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float deathFadeDuration = 1f;

    private void Awake()
    {
        hitIndicatorImage.color = new Color(hitIndicatorImage.color.r, hitIndicatorImage.color.g, hitIndicatorImage.color.b, 0f);
        playerHealth.OnHealthChanged += ShowHitIndicator;
    }

    public void ShowHitIndicator(int health, int maxHealth)
    {
        StopAllCoroutines();
        if (health > 0 || health != maxHealth)
        {
            StartCoroutine(FadeInAndOut());
        }
        else
        {
            hitIndicatorImage.DOFade(1f, deathFadeDuration);
        }
    }

    private System.Collections.IEnumerator FadeInAndOut()
    {
        yield return hitIndicatorImage.DOFade(0.7f, fadeDuration).WaitForCompletion();
        hitIndicatorImage.DOFade(0f, fadeDuration);
    }
}
