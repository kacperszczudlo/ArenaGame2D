using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellConfirmationUI : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private TextMeshProUGUI questionText;
	[SerializeField] private Button yesButton;
	[SerializeField] private Button noButton;

	private Action onConfirmAction;

	private void Awake()
	{
		if (yesButton != null)
		{
			yesButton.onClick.RemoveListener(OnYesClicked);
			yesButton.onClick.AddListener(OnYesClicked);
		}

		if (noButton != null)
		{
			noButton.onClick.RemoveListener(OnNoClicked);
			noButton.onClick.AddListener(OnNoClicked);
		}

		gameObject.SetActive(false);
	}

	public void Show(string itemName, int price, Action onConfirm)
	{
		if (questionText == null || yesButton == null || noButton == null)
		{
			Debug.LogError("[HANDEL] SellConfirmationUI nie ma kompletnych referencji UI.");
			return;
		}

		questionText.text = $"Czy na pewno chcesz sprzedać {itemName} za {price}g?";
		onConfirmAction = onConfirm;
		gameObject.SetActive(true);
	}

	private void OnYesClicked()
	{
		onConfirmAction?.Invoke();
		Close();
	}

	private void OnNoClicked()
	{
		Close();
	}

	private void Close()
	{
		onConfirmAction = null;
		gameObject.SetActive(false);
	}
}
