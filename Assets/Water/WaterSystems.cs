
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class WaterSystems : MonoBehaviour
{
    [SerializeField] WaterSurface _waterSurface;

    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    public float GetWaterLevel(Vector3 position)
    {
        // Find the water level at a specific point
        searchParameters.startPositionWS = position;
        searchParameters.targetPositionWS = position;
        searchParameters.error = 0.01f;
        searchParameters.maxIterations = 8;

        _waterSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult);
        return searchResult.projectedPositionWS.y;
    }

    public void SetUnderwater(bool enabled)
    {
        _waterSurface.underWater = enabled;
    }
}
