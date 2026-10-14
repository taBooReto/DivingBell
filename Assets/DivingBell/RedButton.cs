using UnityEngine;
using UnityEngine.Events;

public class RedButton : MonoBehaviour, Interactive
{
    public UnityEvent _onPressed;

    public void Interact(GameObject interactor)
    {
        _onPressed?.Invoke();
    }

    public void Release(GameObject interactor)
    {
        // No action needed on release for the red button
    }
}
