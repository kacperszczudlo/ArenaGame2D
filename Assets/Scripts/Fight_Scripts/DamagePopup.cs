using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textMesh;
    public SpriteRenderer iconRenderer;

    [Header("Kolory Napisów")]
    public Color normalDamageColor = Color.red;
    public Color critDamageColor = Color.yellow;
    public Color healColor = Color.green;
    public Color manaColor = Color.blue;
    public Color buffColor = new Color(1f, 0.5f, 0f);

    [Header("Parametry animacji")]
    public float moveSpeed = 2f;
    public float disappearTimer = 0.8f;
    public float fadeSpeed = 3f;

    private Color currentTextColor;
    private Color currentIconColor;

    public enum PopupType { NormalDamage, CriticalDamage, Heal, RestoreMana, Buff, TextOnly, Miss }

    // Przyjmuje szansê w procentach (np. "50%")
    public void Setup(string textContent, PopupType type, Sprite icon = null, string chanceText = "")
    {
        // Jeœli podano procent, doklejamy go po prawej stronie!
        if (!string.IsNullOrEmpty(chanceText))
        {
            textContent += $" <size=40%><color=#A0A0A0>{chanceText}</color></size>";
        }

        textMesh.text = textContent;

        switch (type)
        {
            case PopupType.NormalDamage: currentTextColor = normalDamageColor; break;
            case PopupType.CriticalDamage: currentTextColor = critDamageColor; textMesh.fontSize += 3; break;
            case PopupType.Heal: currentTextColor = healColor; break;
            case PopupType.RestoreMana: currentTextColor = manaColor; break;
            case PopupType.Buff: currentTextColor = buffColor; break;
            case PopupType.TextOnly: currentTextColor = Color.white; break;
            case PopupType.Miss: currentTextColor = Color.gray; break; // Szary dla pud³a
        }

        textMesh.color = currentTextColor;

        // Obs³uga ikonki (ukrywamy przy pudle)
        if (iconRenderer != null)
        {
            if (icon != null && type != PopupType.Miss)
            {
                iconRenderer.sprite = icon;
                iconRenderer.gameObject.SetActive(true);
                currentIconColor = iconRenderer.color;
            }
            else
            {
                iconRenderer.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            currentTextColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = currentTextColor;

            if (iconRenderer != null && iconRenderer.gameObject.activeSelf)
            {
                currentIconColor.a -= fadeSpeed * Time.deltaTime;
                iconRenderer.color = currentIconColor;
            }

            if (currentTextColor.a <= 0) Destroy(gameObject);
        }
    }
}