using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, Interactive 
{
    [SerializeField] private UnityEvent _onInteract;
    public bool Interaction(GameObject interactor) 
    { 
        _onInteract?.Invoke();
        return true;
    }
}
