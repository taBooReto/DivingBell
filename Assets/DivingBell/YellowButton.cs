using UnityEngine;
using UnityEngine.Events;

public class YellowButton : Interactive
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private UnityEvent _onPressed;

    private void Start()
    {
        _rb.sleepThreshold = 0;
    }

    public override void Interact(GameObject interactor)
    {
        _onPressed?.Invoke();
        //Debug.Log("Yellow button on pressed");
    }
}
