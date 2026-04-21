using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GemTraderNPC : MonoBehaviour, IPointerClickHandler
{
	[Header("UI Dialogu")]
	public GameObject dialogBox;
	public TMP_Text npcNameText;
	public TMP_Text dialogText;
	public Button btnOpenExchange;
	public Button btnLeave;

	[Header("UI Wymiany")]
	public GameObject exchangePanel;
	public TMP_Text textGemsAmount;
	public TMP_Text textStatsAmount;
	public TMP_Text exchangeHintText;
	public Button btnConfirmExchange;
	public Button btnCloseExchange;

	[Header("Wizualia i Ustawienia")]
	public GameObject chatBubble;
	public string npcName = "Mistrz Areny";
	public int gemCost = 1;
	public int statPointsPerExchange = 1;

	private void Start()
	{
		if (btnOpenExchange != null)
		{
			btnOpenExchange.onClick.AddListener(OpenExchangeWindow);
		}

		if (btnLeave != null)
		{
			btnLeave.onClick.AddListener(CloseDialogue);
		}

		if (btnConfirmExchange != null)
		{
			btnConfirmExchange.onClick.AddListener(ExchangeGem);
		}

		if (btnCloseExchange != null)
		{
			btnCloseExchange.onClick.AddListener(CloseExchangeWindow);
		}

		if (dialogBox != null)
		{
			dialogBox.SetActive(false);
		}

		if (exchangePanel != null)
		{
			exchangePanel.SetActive(false);
		}

		if (chatBubble != null)
		{
			chatBubble.SetActive(true);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OpenDialogue();
	}

	private void OpenDialogue()
	{
		if (dialogBox != null)
		{
			dialogBox.SetActive(true);
		}

		if (chatBubble != null)
		{
			chatBubble.SetActive(false);
		}

		if (npcNameText != null)
		{
			npcNameText.text = npcName;
		}

		if (dialogText != null)
		{
			dialogText.text = "Witaj czempionie. Turniejowe klejnoty to rzadki skarb. Mogę wymienić je na punkty statystyk.";
		}

		if (btnOpenExchange != null)
		{
			btnOpenExchange.gameObject.SetActive(true);
			TMP_Text openLabel = btnOpenExchange.GetComponentInChildren<TMP_Text>();
			if (openLabel != null)
			{
				openLabel.text = "1. Pokaż mi ofertę wymiany.";
			}
		}

		if (btnLeave != null)
		{
			TMP_Text leaveLabel = btnLeave.GetComponentInChildren<TMP_Text>();
			if (leaveLabel != null)
			{
				leaveLabel.text = "2. Może innym razem. [Wyjście]";
			}
		}
	}

	private void OpenExchangeWindow()
	{
		if (dialogBox != null)
		{
			dialogBox.SetActive(false);
		}

		if (exchangePanel != null)
		{
			exchangePanel.SetActive(true);
		}

		RefreshExchangeUI();
	}

	private void RefreshExchangeUI()
	{
		if (GameManager.Instance == null || PlayerDataManager.Instance == null)
		{
			if (exchangeHintText != null)
			{
				exchangeHintText.text = "Brak managerów w scenie.";
			}
			return;
		}

		if (textGemsAmount != null)
		{
			textGemsAmount.text = $"Twoje Gemy: {GameManager.Instance.tournamentGems}";
		}

		if (textStatsAmount != null)
		{
			textStatsAmount.text = $"Dostępne punkty: {PlayerDataManager.Instance.availableStatPoints}";
		}

		if (exchangeHintText != null)
		{
			exchangeHintText.text = $"Koszt: {gemCost} gem = {statPointsPerExchange} punkt";
		}
	}

	private void ExchangeGem()
	{
		if (GameManager.Instance == null || PlayerDataManager.Instance == null)
		{
			Debug.LogWarning("[WYMIANA] Brak GameManager lub PlayerDataManager.");
			return;
		}

		if (gemCost <= 0 || statPointsPerExchange <= 0)
		{
			Debug.LogWarning("[WYMIANA] Nieprawidłowa konfiguracja kosztu wymiany.");
			return;
		}

		if (!GameManager.Instance.RemoveTournamentGems(gemCost))
		{
			if (exchangeHintText != null)
			{
				exchangeHintText.text = "Za mało gemów turniejowych!";
			}
			return;
		}

		PlayerDataManager.Instance.AddStatPoints(statPointsPerExchange);
		if (exchangeHintText != null)
		{
			exchangeHintText.text = $"Wymieniono {gemCost} gem na {statPointsPerExchange} punkt!";
		}

		RefreshExchangeUI();
	}

	private void CloseExchangeWindow()
	{
		if (exchangePanel != null)
		{
			exchangePanel.SetActive(false);
		}

		if (chatBubble != null)
		{
			chatBubble.SetActive(true);
		}
	}

	private void CloseDialogue()
	{
		if (dialogBox != null)
		{
			dialogBox.SetActive(false);
		}

		if (chatBubble != null)
		{
			chatBubble.SetActive(true);
		}
	}
}
