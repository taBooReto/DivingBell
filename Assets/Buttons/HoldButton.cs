using UnityEngine;
using UnityEngine.Events;

public class HoldButton : MonoBehaviour, Interactive
{
    [SerializeField] private ButtonAnimator _buttonAnimator;

    [SerializeField] private UnityEvent _onPressed;
    [SerializeField] private UnityEvent _onReleased;

    void Start()
    {
        _buttonAnimator = GetComponent<ButtonAnimator>();
    }

    public bool Interaction(GameObject interactor)
    {
        _onPressed?.Invoke();

        if(_buttonAnimator) {_buttonAnimator.SetPressed(true);}

        return true;
    }

    public void Release()
    {
        _onReleased?.Invoke();

        if(_buttonAnimator) {_buttonAnimator.SetPressed(false);}
    }
}