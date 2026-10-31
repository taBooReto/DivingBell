using UnityEngine;
using UnityEngine.Events;

public class HoldButton : MonoBehaviour, Interactive
{
    [SerializeField] private UnityEvent _onPressed;
    [SerializeField] private UnityEvent _onReleased;

    public bool Interaction(GameObject interactor)
    {
        _onPressed?.Invoke();
        return true;
    }

    public void Release()
    {
        _onReleased?.Invoke();
    }
}