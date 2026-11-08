using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, Interactive 
{
    [SerializeField] private ButtonAnimator _buttonAnimator;
    [SerializeField] private UnityEvent _onInteract;
   
    public bool Interaction(GameObject interactor) 
    { 
        _onInteract?.Invoke();

        if(_buttonAnimator) {_buttonAnimator.Press();}

        return true;
    }
}
