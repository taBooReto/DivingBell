using UnityEngine;

public class ShipPlayerAdapter : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void ApplyShipRotation(Quaternion deltaRotation)
    {

        Quaternion cameraRotation = _playerController.PlayerCamera.Rotation;

        // Обертаємо фізичне тіло разом із кораблем
        _rb.MoveRotation(deltaRotation * _rb.rotation);

        // Синхронізуємо внутрішні кути PlayerCamera
        // через її наявний публічний метод
        _playerController.PlayerCamera.SetRotation(deltaRotation * cameraRotation);
    }
}