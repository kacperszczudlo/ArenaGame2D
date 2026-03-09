using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI durationText;

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
    }
}