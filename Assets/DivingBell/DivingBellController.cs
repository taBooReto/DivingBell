using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class DivingBellController : MonoBehaviour
{
    [Header("Diving")]
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private Rigidbody[] _divingBodies;
    [SerializeField] private float _divingSpeed = 0.25f;
    [SerializeField] private float _minRopeLength = 1f;

    private bool _isDiving = false;
    private bool _isSurfacing = false;

    [Header("Bell")]
    [SerializeField] private Transform _collider;
    [SerializeField] private Transform _mesh;

    [Header("Player")]
    [SerializeField] private Transform _playerAnchor;
    [SerializeField] private Transform _exitAnchor;
    [SerializeField] private float _playerTransitionDuration = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent _onPlayerEnteredBell;
    [SerializeField] private UnityEvent _onPlayerExitedBell;

    [SerializeField] private UnityEvent _onMovementStarted;
    [SerializeField] private UnityEvent _onMovementStopped;
    private PlayerController _playerInside;

    private void FixedUpdate()
    {
        _collider.position = _mesh.position;

        if (_isDiving)
            Dive();
        else if (_isSurfacing)
            Surface();
    }

    private void Dive()
    {
        float delta = _divingSpeed * Time.fixedDeltaTime;

        ChangeRopeLength(delta);
        MoveDivingBodies(-delta);
    }

    private void Surface()
    {
        float currentLength = _joint.linearLimit.limit;

        float delta = Mathf.Min(
            _divingSpeed * Time.fixedDeltaTime,
            Mathf.Max(0f, currentLength - _minRopeLength)
        );

        if (delta <= 0f)
        {
            SetSurfacing(false);
            return;
        }

        ChangeRopeLength(-delta);
        MoveDivingBodies(delta);

        if (_joint.linearLimit.limit <= _minRopeLength)
            SetSurfacing(false);
    }

    private void ChangeRopeLength(float delta)
    {
        SoftJointLimit limit = _joint.linearLimit;
        limit.limit += delta;
        _joint.linearLimit = limit;
    }

    private void MoveDivingBodies(float delta)
    {
        for (int i = 0; i < _divingBodies.Length; i++)
        {
            _divingBodies[i].MovePosition(
                _divingBodies[i].position + Vector3.up * delta
            );
        }
    }

    public void SetDiving(bool value)
    {
        SetMovement(value, false);
    }

    public void SetSurfacing(bool value)
    {
        SetMovement(false, value);
    }

    private void SetMovement(bool diving, bool surfacing)
    {
        bool wasMoving = _isDiving || _isSurfacing;

        _isDiving = diving;
        _isSurfacing = surfacing;

        bool isMoving = _isDiving || _isSurfacing;

        if (!wasMoving && isMoving) _onMovementStarted?.Invoke();
        else if (wasMoving && !isMoving) _onMovementStopped?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_playerInside) return;

        if (other.CompareTag("Player"))
        {
            _playerInside = other.GetComponentInParent<PlayerController>();
            _playerInside.transform.SetParent(transform);
            _playerInside.SetMoveability(false);

            StartCoroutine(MovePlayer(_playerInside, _playerAnchor));
            _onPlayerEnteredBell?.Invoke();
        }

    }

    public void ExitDivingBell()
    {
        StartCoroutine(ExitDivingBellCoroutine(_playerInside));
    }

    private IEnumerator ExitDivingBellCoroutine(PlayerController player)
    {
        _onPlayerExitedBell?.Invoke();
        yield return StartCoroutine(MovePlayer(player, _exitAnchor));

        player.SetMoveability(true);
        player.transform.SetParent(null);
        _playerInside = null;
    }

    private IEnumerator MovePlayer(PlayerController player, Transform targetAnchor)
    {
        Rigidbody rb = player.Rb;
        rb.isKinematic = true;

        Transform playerTransform = player.transform;
        Vector3 startPosition = playerTransform.position;

        float elapsed = 0f;

        while (elapsed < _playerTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _playerTransitionDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            playerTransform.position = Vector3.Lerp(
                startPosition,
                targetAnchor.position,
                t
            );

            yield return null;
        }

        playerTransform.position = targetAnchor.position;
        playerTransform.SetParent(transform, true);
    }
}
