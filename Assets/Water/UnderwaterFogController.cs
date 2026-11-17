
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class UnderwaterFogController : MonoBehaviour
{
    [SerializeField] private GameObject _plane;
    [SerializeField] private WaterSystems waterSystems;
    [SerializeField] private CustomPassVolume fogVolume;
    [SerializeField] private Transform playerCamera;

    private void LateUpdate()
    {
        if (fogVolume == null || playerCamera == null)
            return;

        bool isUnderwater = waterSystems.GetWaterLevel(playerCamera.position) > playerCamera.position.y;

        if (fogVolume.enabled != isUnderwater)
            fogVolume.enabled = isUnderwater;

        if(isUnderwater)
        {
            _plane.SetActive(true);
            _plane.transform.position = new Vector3(playerCamera.position.x, 12, playerCamera.position.z);
        }
        else
        {
            _plane.SetActive(false);
        }
    }
}
