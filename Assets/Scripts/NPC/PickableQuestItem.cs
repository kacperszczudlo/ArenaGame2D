using UnityEngine;

public class PickableQuestItem : MonoBehaviour
{
	[HideInInspector] public string itemName;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Player"))
		{
			return;
		}

		if (QuestManager.Instance == null || !QuestManager.Instance.isQuestActive || QuestManager.Instance.hasCollectedItem)
		{
			return;
		}

		string deliveredName = string.IsNullOrWhiteSpace(itemName) ? gameObject.name : itemName;
		if (!QuestManager.Instance.ItemCollectedByPlayer(deliveredName))
		{
			return;
		}

		Destroy(gameObject);
	}
}
