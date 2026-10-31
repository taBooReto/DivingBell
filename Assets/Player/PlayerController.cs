using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerCamera))]
[RequireComponent(typeof(PlayerWalk))]
[RequireComponent(typeof(PlayerSwim))]
[RequireComponent(typeof(PlayerRayCast))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Inputs")]
    [SerializeField] private InputAction _lookAction;
    [SerializeField] private InputAction _interactAction;
    [SerializeField] private InputAction _moveInput;
    [SerializeField] private InputAction _swimVerticalAction;

    [Header("Player Components")]
    private PlayerCamera _playerCamera;
    public PlayerCamera PlayerCamera => _playerCamera;

    private PlayerWalk _playerWalk;
    public PlayerWalk PlayerWalk => _playerWalk;

    private PlayerSwim _playerSwim;
    public PlayerSwim PlayerSwim => _playerSwim;

    private PlayerRayCast _playerRayCast;
    public PlayerRayCast PlayerRayCast => _playerRayCast;

    private Rigidbody _rb;
    public Rigidbody Rb => _rb;

    private bool _canWalk = true;
    private bool _canSwim = true;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _playerCamera = GetComponent<PlayerCamera>();
        _playerWalk = GetComponent<PlayerWalk>();
        _playerSwim = GetComponent<PlayerSwim>();
        _playerRayCast = GetComponent<PlayerRayCast>();
        _rb = GetComponent<Rigidbody>();

        Debug.Assert(_playerCamera != null, "PlayerCamera component is missing!");
        Debug.Assert(_playerWalk != null, "PlayerWalk component is missing!");
        Debug.Assert(_playerSwim != null, "PlayerSwim component is missing!");
        Debug.Assert(_playerRayCast != null, "PlayerRayCast component is missing!");
        Debug.Assert(_rb != null, "Rigidbody component is missing!");
    }

    void OnEnable()
    {
        _lookAction.Enable();
        _interactAction.Enable();
        _moveInput.Enable();
        _swimVerticalAction.Enable();

        _interactAction.performed += OnInteract;
        _interactAction.canceled += OnInteractReleased;
        _playerSwim.OnDiveStateChanged += ChangDiveState;
    }

    void OnDisable()
    {
        _lookAction.Disable();
        _interactAction.Disable();
        _moveInput.Disable();
        _swimVerticalAction.Disable();

        _interactAction.performed -= OnInteract;
        _interactAction.canceled -= OnInteractReleased;
        _playerSwim.OnDiveStateChanged -= ChangDiveState;
    }

    private void Update()
    {
        _playerCamera.Look(_lookAction);

        _playerSwim.UpdateDiveState();
    }

    private void FixedUpdate()
    {
        _playerWalk.Walk(_moveInput, _rb);
        _playerSwim.Swim(_moveInput, _swimVerticalAction, _playerCamera.CameraTransform, _rb);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        _playerRayCast.OnInteract();
    }
    private void OnInteractReleased(InputAction.CallbackContext context)
    {
        _playerRayCast.OnInteractReleased();
    }

    private void ChangDiveState(bool isDived)
    {
       SetWalkAbility(!isDived);
        _rb.useGravity = !isDived;
    }

    public void SetCameraRotationLocked(bool locked)
    {
        _playerCamera.SetRotationLocked(locked);
    }

    public void SetWalkAbility(bool canWalk)
    {
        _canWalk = canWalk;
        _playerWalk.SetWalkAbility(canWalk);
    }

    public void SetSwimAbility(bool canSwim)
    {
        _canSwim = canSwim;
        _playerSwim.SetSwimAbility(canSwim);
    }

    public void SetMoveability(bool canMove)
    {
        if(_canWalk) _playerWalk.SetWalkAbility(canMove);

        if (_canSwim) _playerSwim.SetSwimAbility(canMove);

        _rb.isKinematic = !canMove;
    }

    public void SetStabilizeCameraRotation(bool enabled)
    {
        _playerCamera.SetStabilizeRotation(enabled);
    }
}
