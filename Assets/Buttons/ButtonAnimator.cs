using System.Collections;
using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Transform _button;
    [SerializeField] private Vector3 _pressOffset;
    [SerializeField] private float _animationDuration = 0.15f;

    private Vector3 _originalPosition;
    private Coroutine _animation;

    private void Awake()
    {
        _originalPosition = _button.localPosition;
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

        Vector3 targetPosition = pressed
            ? _originalPosition + _pressOffset
            : _originalPosition;

        _animation = StartCoroutine(Animate(targetPosition));
    }

    private IEnumerator PressAnimation()
    {
        yield return Animate(_originalPosition + _pressOffset);
        yield return Animate(_originalPosition);

        _animation = null;
    }

    private IEnumerator Animate(Vector3 targetPosition)
    {
        Vector3 startPosition = _button.localPosition;
        float elapsed = 0f;

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _animationDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            _button.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            yield return null;
        }

        _button.localPosition = targetPosition;
    }
}