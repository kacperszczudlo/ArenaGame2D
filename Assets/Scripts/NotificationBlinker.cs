using UnityEngine;
using UnityEngine.UI;

public class NotificationBlinker : MonoBehaviour
{
    public enum NotificationType { Stats, Abilities }

    [Header("Ustawienia Powiadomienia")]
    public NotificationType type;
    public Image targetImage;

    [Header("Kolory")]
    public Color normalColor = Color.white;
    public Color blinkColor = new Color(1f, 0.8f, 0f, 1f);
    public float blinkSpeed = 3f;

    void Update()
    {
        if (PlayerDataManager.Instance == null) return;

        bool shouldBlink = false;
        int currentPoints = 0;

        if (type == NotificationType.Stats)
        {
            shouldBlink = PlayerDataManager.Instance.hasUnseenStatPoints;
            currentPoints = PlayerDataManager.Instance.availableStatPoints;
        }
        else
        {
            shouldBlink = PlayerDataManager.Instance.hasUnseenSkillPoints;
            currentPoints = PlayerDataManager.Instance.availableSkillPoints;
        }

        if (shouldBlink && currentPoints > 0)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            if (targetImage != null) targetImage.color = Color.Lerp(normalColor, blinkColor, t);
        }
        else
        {
            if (targetImage != null) targetImage.color = normalColor;
        }
    }

    public void OnButtonClicked()
    {
        if (PlayerDataManager.Instance == null) return;

        if (type == NotificationType.Stats)
        {
            PlayerDataManager.Instance.hasUnseenStatPoints = false;
        }
        else
        {
            PlayerDataManager.Instance.hasUnseenSkillPoints = false;
        }

        if (targetImage != null) targetImage.color = normalColor;
    }
}