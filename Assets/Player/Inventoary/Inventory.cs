
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InputAction _switchAction;
    [SerializeField] private InputAction _dropAction;
    [SerializeField] private InventorySlot[] _slots;
    [SerializeField] private RectTransform _frame;
    [SerializeField] private Transform _dropPoint;
    [SerializeField] private float _dropForce;
    int activeSlotIndex;

    private void OnEnable()
    {
        _switchAction.Enable();
        _dropAction.Enable();
        _switchAction.performed += OnSwitch;
        _dropAction.performed += OnDrop;
    }
    private void OnDisable()
    {
        _switchAction.Disable();
        _dropAction.Disable();
        _switchAction.performed -= OnSwitch;
        _dropAction.performed -= OnDrop;
    }

    private void OnSwitch(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();
        if (scroll == 0) return;

        // Конвертуємо у int (-1 або +1)
        int delta = scroll > 0 ? 1 : -1;

        activeSlotIndex = (activeSlotIndex + delta + _slots.Length) % _slots.Length;

        // Рух рамки до активного слоту (UI Canvas)
        _frame.anchoredPosition = _slots[activeSlotIndex].GetComponent<RectTransform>().anchoredPosition;
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        GameObject dropedItem = _slots[activeSlotIndex].Item.gameObject;
        dropedItem.transform.SetPositionAndRotation(_dropPoint.position, _dropPoint.rotation);
        dropedItem.SetActive(true);
        _slots[activeSlotIndex].Clean();

        Rigidbody itemRb;
        if (dropedItem.TryGetComponent<Rigidbody>(out itemRb))
        {
            itemRb.AddForce(_dropPoint.forward * _dropForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Dropped item does not have a Rigidbody component.");
        }
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Item == null)
            {
                _slots[i].AddItem(item);
                return;
            }
        }

        //Instantiate(item.ItemObject);
    }
}
