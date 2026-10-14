using UnityEngine;
using UnityEngine.Events;

public class DivingBellTransition : MonoBehaviour
{
    [SerializeField] private UnityEvent _onExit;
    [SerializeField] private UnityEvent _onEnter;

    public void ExitBell()
    {
        _onExit?.Invoke();
    }

    public void EnterBell()
    {
        _onEnter?.Invoke();
    }
}
