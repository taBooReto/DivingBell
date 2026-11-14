
using System.Collections;
using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    public enum AnimationMode
    {
        Move,
        Rotate
    }

    [Header("Animation")]
    [SerializeField] private Transform _button;
    [SerializeField] private AnimationMode _animationMode = AnimationMode.Move;

    [Header("Move Settings")]
    [SerializeField] private Vector3 _pressOffset;

    [Header("General")]
    [SerializeField] private float _animationDuration = 0.15f;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Coroutine _animation;

    private void Awake()
    {
        _originalPosition = _button.localPosition;
        _originalRotation = _button.localRotation;
    }

    public void Press()
    {
        if (_animation != null)
            StopCoroutine(_animation);

        _animation = StartCoroutine(PressAnimation());
    }

    public void SetPressed(bool pressed)
    {
        if (_animation != null)
            StopCoroutine(_animation);

        _animation = StartCoroutine(Animate(pressed));
    }

    private IEnumerator PressAnimation()
    {
        yield return Animate(true);
        yield return Animate(false);

        _animation = null;
    }

    private IEnumerator Animate(bool pressed)
    {
        Vector3 startPosition = _button.localPosition;
        Quaternion startRotation = _button.localRotation;

        Vector3 targetPosition = pressed
            ? _originalPosition + _pressOffset
            : _originalPosition;

        Quaternion targetRotation = pressed
            ? _originalRotation * Quaternion.Euler(_pressOffset)
            : _originalRotation;

        float elapsed = 0f;

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _animationDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            switch (_animationMode)
            {
                case AnimationMode.Move:
                    _button.localPosition = Vector3.Lerp(
                        startPosition,
                        targetPosition,
                        t
                    );
                    break;

                case AnimationMode.Rotate:
                    _button.localRotation = Quaternion.Slerp(
                        startRotation,
                        targetRotation,
                        t
                    );
                    break;
            }

            yield return null;
        }

        if (_animationMode == AnimationMode.Move)
            _button.localPosition = targetPosition;
        else
            _button.localRotation = targetRotation;
    }
}
