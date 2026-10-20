using UnityEngine;

public class Item : MonoBehaviour, Interactive
{
    public Sprite Icon;
    public bool Interaction(GameObject interactor)
    {
        if (interactor.TryGetComponent<Inventory>(out Inventory inventory))
        {
            inventory.AddItem(this);
            gameObject.SetActive(false);
            return true;
        }

        return false;
    }
}
