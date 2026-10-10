using UnityEngine;
using UnityEngine.Events;

public class RedButton : Interactive
{
    public UnityEvent _onPressed;

    public override void Interact(GameObject interactor)
    {
        _onPressed?.Invoke();
    }
}
