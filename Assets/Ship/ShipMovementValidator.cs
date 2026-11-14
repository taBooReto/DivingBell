
using UnityEngine;

public class ShipMovementValidator : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private BellWinchController _bellWinch;
    [SerializeField] private Switch _helmSwitch;
    [SerializeField] private ShipController _shipController;

    [Header("Status Lights")]
    [SerializeField] private Renderer _bellWinchStatusLight;
    [SerializeField] private Renderer _helmStationStatusLight;

    [Header("Status Materials")]
    [SerializeField] private Material _statusLightRed;
    [SerializeField] private Material _statusLightGreen;

    private void Update()
    {
        bool isStowed = _bellWinch.IsStowed;
        bool helmEnabled = isStowed && _helmSwitch.IsOn;

        // Ship movement
        _shipController.SetAllowMovement(helmEnabled);

        // Lock controls
        _bellWinch.SetLocked(helmEnabled);
        _helmSwitch.SetLocked(!isStowed);

        // Status lights
        _bellWinchStatusLight.sharedMaterial =
            helmEnabled ? _statusLightRed : _statusLightGreen;

        _helmStationStatusLight.sharedMaterial =
            isStowed ? _statusLightGreen : _statusLightRed;
    }
}
