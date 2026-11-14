
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour, Interactive
{

    [Header("Settings")]
    [SerializeField] private ButtonAnimator _buttonAnimator;

    [Header("Events")]
    [SerializeField] private UnityEvent _onSwitchOn;
    [SerializeField] private UnityEvent _onSwitchOff;

    private bool _isOn;
    public bool IsOn => _isOn;

    private bool _isLocked;
    public bool IsLocked => _isLocked;

    private void Start()
    {
        if (_buttonAnimator)
            _buttonAnimator.SetPressed(_isOn);
    }

    public bool Interaction(GameObject interactor)
    {
        if (_isLocked) return false;

        SetState(!_isOn);
        return true;
    }

    public void SetState(bool isOn)
    {
        if(_isLocked) return;

        if (_isOn == isOn) return;

        _isOn = isOn;

        if (_buttonAnimator)
        {
            _buttonAnimator.SetPressed(_isOn);
        }

        if (_isOn)
        {
            _onSwitchOn?.Invoke();
        }
        else 
        {
            _onSwitchOff?.Invoke();
        }
    }

    public void Toggle()
    {
        SetState(!_isOn);
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
    }
}
