using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage; 
    public bool isFull = false;
    public string itemCategory = ""; 

    public void AddItem(Sprite newIcon, string category)
    {
        if (iconImage == null) {
            Debug.LogError($"BŁĄD: Slot o nazwie {gameObject.name} nie ma przypisanego Icon Image w Inspektorze!");
            return; 
        }

        iconImage.sprite = newIcon;
        iconImage.color = new Color(1f, 1f, 1f, 1f); 
        itemCategory = category;
        isFull = true;
        
        Debug.Log($"Sukces! Dodano przedmiot kategorii {category} do slotu {gameObject.name}");
    }
}