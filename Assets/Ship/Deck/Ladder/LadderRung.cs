
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LadderRung : MonoBehaviour, Interactive
{
    [Header("Climbing Settings")]
    [SerializeField] private float _climbSpeed = 2f;

    [Header("Target Position")]
    [SerializeField] private Vector3 _targetOffset = Vector3.zero;

    [Header("Gizmos")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private Color _gizmoColor = Color.green;
    [SerializeField] private float _gizmoSize = 0.1f;

    private static readonly Dictionary<PlayerController, LadderRung>
        _activeRungs = new();

    private PlayerController _player;
    private Coroutine _climbCoroutine;

    private Vector3 TargetPosition =>
        transform.TransformPoint(_targetOffset);

    public bool Interaction(GameObject interactor)
    {
        PlayerController player = interactor.GetComponent<PlayerController>();

        if (!player)
            return false;

        if (_climbCoroutine != null)
            return false;

        // Якщо гравець уже тримається за іншу перекладину
        if (_activeRungs.TryGetValue(player, out LadderRung previousRung))
        {
            previousRung.StopClimbing(false);
        }

        _player = player;
        _activeRungs[player] = this;

        _player.CancelAction.performed += CancelClimb;

        _climbCoroutine = StartCoroutine(ClimbLadder());

        return true;
    }

    private void CancelClimb(InputAction.CallbackContext context)
    {
        StopClimbing(true);
    }

    private void StopClimbing(bool restoreMovement)
    {
        if (!_player)
            return;

        PlayerController player = _player;

        player.CancelAction.performed -= CancelClimb;

        if (_climbCoroutine != null)
        {
            StopCoroutine(_climbCoroutine);
            _climbCoroutine = null;
        }

        if (_activeRungs.TryGetValue(player, out LadderRung rung)
            && rung == this)
        {
            _activeRungs.Remove(player);
        }

        _player = null;

        if (restoreMovement)
        {
            player.SetMoveability(true);
            player.SetWalkAbility(true);
        }

    }

    private IEnumerator ClimbLadder()
    {
        _player.SetMoveability(false);

        Rigidbody rb = _player.Rb;

        // Наближення до перекладини
        while (Vector3.Distance(rb.position, TargetPosition) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(
                rb.position,
                TargetPosition,
                _climbSpeed * Time.fixedDeltaTime
            );

            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }

        // Постійне слідування за перекладиною
        while (true)
        {
            rb.MovePosition(TargetPosition);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDisable()
    {
        StopClimbing(true);
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = _gizmoColor;

        Gizmos.DrawLine(transform.position, TargetPosition);
        Gizmos.DrawWireSphere(TargetPosition, _gizmoSize);
    }
}
