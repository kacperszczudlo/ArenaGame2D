using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceBar : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI maxText;

    private float max;

    public void SetMaxResource(float maxValue)
    {
        max = maxValue;
        if (maxText != null) maxText.text = Mathf.RoundToInt(max).ToString();
        UpdateValue(max); // Startujemy z pe³nym paskiem
    }

    public void UpdateValue(float currentValue)
    {
        if (max <= 0) return;

        // Aktualizacja grafiki paska
        if (fillImage != null)
            fillImage.fillAmount = currentValue / max;

        // Aktualizacja tekstu obecnej wartoœci
        if (currentText != null)
            currentText.text = Mathf.RoundToInt(currentValue).ToString();
    }
}