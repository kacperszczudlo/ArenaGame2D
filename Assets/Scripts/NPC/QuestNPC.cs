using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestNPC : MonoBehaviour, IPointerClickHandler
{
	[Header("UI Dialogu")]
	public GameObject dialogBox;
	public TMP_Text npcNameText;
	public TMP_Text dialogText;
	public Button btnAccept;
	public Button btnLeave;

	[Header("Wizualia")]
	public GameObject chatBubble;
	public string npcName = "Zwiadowca Riko";

	private void Start()
	{
		if (btnLeave != null)
		{
			btnLeave.onClick.AddListener(CloseDialogue);
		}

		if (dialogBox != null)
		{
			dialogBox.SetActive(false);
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

		RefreshDialogueText();
	}

	private void RefreshDialogueText()
	{
		if (QuestManager.Instance == null)
		{
			if (dialogText != null)
			{
				dialogText.text = "Brak aktywnego QuestManagera w scenie.";
			}

			SetAcceptButtonVisible(false);
			return;
		}

		if (btnAccept != null)
		{
			btnAccept.onClick.RemoveAllListeners();
		}

		if (QuestManager.Instance.isQuestActive && QuestManager.Instance.hasCollectedItem)
		{
			if (dialogText != null)
			{
				dialogText.text = $"Widzę, że masz {QuestManager.Instance.targetItemName}. Oddaj przedmiot, a wypłacę {QuestManager.Instance.calculatedGoldReward}g.";
			}

			SetAcceptButtonVisible(true);
			SetAcceptButtonText("1. Proszę, oto przedmiot. [Oddaj]");
			btnAccept.onClick.AddListener(HandOverItem);
			SetLeaveButtonText("2. Jeszcze go potrzymam. [Wyjście]");
			return;
		}

		if (QuestManager.Instance.isQuestActive)
		{
			if (dialogText != null)
			{
				dialogText.text = $"Przypominam: szukasz {QuestManager.Instance.targetItemName}. Powinien być {QuestManager.Instance.biomeHint}.";
			}

			SetAcceptButtonVisible(false);
			SetLeaveButtonText("Rozumiem, szukam dalej. [Wyjście]");
			return;
		}

		if (dialogText != null)
		{
			dialogText.text = "Witaj wędrowcze. Potrzebuję rzadkiego artefaktu. Czy podejmiesz się poszukiwań za garść monet?";
		}

		SetAcceptButtonVisible(true);
		SetAcceptButtonText("1. Biorę to zadanie!");
		btnAccept.onClick.AddListener(AcceptNewQuest);
		SetLeaveButtonText("2. Muszę już iść. [Wyjście]");
	}

	private void AcceptNewQuest()
	{
		if (QuestManager.Instance == null)
		{
			Debug.LogWarning("[QUEST NPC] Brak QuestManager.Instance.");
			return;
		}

		if (!QuestManager.Instance.GenerateNewQuest())
		{
			RefreshDialogueText();
			return;
		}

		if (dialogText != null)
		{
			dialogText.text = $"Świetnie! Przynieś mi {QuestManager.Instance.targetItemName}. Krążą słuchy, że można go znaleźć {QuestManager.Instance.biomeHint}. Nagroda to {QuestManager.Instance.calculatedGoldReward}g.";
		}

		SetAcceptButtonVisible(false);
		SetLeaveButtonText("Jasne, ruszam w drogę. [Wyjście]");
	}

	private void HandOverItem()
	{
		if (QuestManager.Instance == null)
		{
			return;
		}

		if (!QuestManager.Instance.TryCompleteQuest(QuestManager.Instance.targetItemName))
		{
			RefreshDialogueText();
			return;
		}

		if (dialogText != null)
		{
			dialogText.text = "Dziękuję za pomoc, wędrowcze! Oto twoja zapłata.";
		}

		SetAcceptButtonVisible(false);
		SetLeaveButtonText("Cała przyjemność po mojej stronie. [Wyjście]");
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

	private void SetAcceptButtonVisible(bool visible)
	{
		if (btnAccept != null)
		{
			btnAccept.gameObject.SetActive(visible);
		}
	}

	private void SetLeaveButtonText(string text)
	{
		if (btnLeave == null)
		{
			return;
		}

		TMP_Text leaveLabel = btnLeave.GetComponentInChildren<TMP_Text>();
		if (leaveLabel != null)
		{
			leaveLabel.text = text;
		}
	}

	private void SetAcceptButtonText(string text)
	{
		if (btnAccept == null)
		{
			return;
		}

		TMP_Text acceptLabel = btnAccept.GetComponentInChildren<TMP_Text>();
		if (acceptLabel != null)
		{
			acceptLabel.text = text;
		}
	}
}
