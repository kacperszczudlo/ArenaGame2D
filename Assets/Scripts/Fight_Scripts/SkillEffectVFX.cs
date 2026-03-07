using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillEffectVFX : MonoBehaviour
{
    public Image iconImage;
    public CanvasGroup canvasGroup; // Odpowiada za p³ynne zanikanie ca³ego obiektu
    public float displayTime = 0.6f;
    public float fadeSpeed = 3f;

    public void Setup(Sprite icon)
    {
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(true);
        }
        StartCoroutine(AnimateRoutine());
    }

    IEnumerator AnimateRoutine()
    {
        // Efekt pop-up (wyskoczenia z mniejszego rozmiaru do normalnego)
        Vector3 targetScale = transform.localScale;
        transform.localScale = targetScale * 0.3f;

        float t = 0;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale * 0.3f, targetScale, t / 0.15f);
            yield return null;
        }
        transform.localScale = targetScale;

        // Czekamy chwilê
        yield return new WaitForSeconds(displayTime);

        // P³ynne znikanie przez CanvasGroup
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        Destroy(gameObject); // Usuwamy obiekt po znikniêciu
    }
}