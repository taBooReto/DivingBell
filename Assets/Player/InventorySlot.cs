
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    [HideInInspector] public Item Item;
    [HideInInspector] public Image Icon;

    public void AddItem(Item newItem)
    {
        Item = newItem;

        _image.sprite = Item.Icon;
    }

    public void Clean()
    {
        Item = null;
        _image.sprite = null;
    }
}