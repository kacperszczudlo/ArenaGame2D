using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // <--- NOWOŒÆ: Biblioteka do obs³ugi myszki!

// Dodaliœmy IPointerEnterHandler i IPointerExitHandler, by skrypt reagowa³ na kursor
public class StatusIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TextMeshProUGUI durationText;

    [Header("Dymek z nazw¹ (Tooltip)")]
    public GameObject tooltipBox;
    public TextMeshProUGUI tooltipText;

    public void Setup(StatusEffect status)
    {
        iconImage.sprite = status.icon;

        // Dla tarczy pokazujemy hity, dla DOT pokazujemy rundy
        if (status.type == StatusType.Shield)
        {
            durationText.text = "x" + status.remainingHits;
            durationText.color = Color.white;
        }
        else
        {
            durationText.text = status.duration.ToString();
            durationText.color = status.isDamage ? Color.red : Color.green;
        }

        // Przygotowujemy tekst dymku, ale domyœlnie go ukrywamy
        if (tooltipText != null) tooltipText.text = status.effectName;
        if (tooltipBox != null) tooltipBox.SetActive(false);
    }


    // Odpala siê, gdy kursor wjedzie na ikonkê
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipBox != null) tooltipBox.SetActive(true);
    }

    // Odpala siê, gdy kursor zjedzie z ikonki
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipBox != null) tooltipBox.SetActive(false);
    }
}