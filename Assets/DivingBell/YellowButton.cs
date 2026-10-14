using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class YellowButton : MonoBehaviour, Interactive
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private UnityEvent _onPressed;
    [SerializeField] private UnityEvent _onReleased;

    private void Start()
    {
        _rb.sleepThreshold = 0;
    }

    public void Interact(GameObject interactor)
    {
        _onPressed?.Invoke();
        //Debug.Log("Yellow button on pressed");
    }

    public void Release(GameObject interactor)
    {
        _onReleased?.Invoke();
        Debug.Log("Yellow button on released");
    }
}
