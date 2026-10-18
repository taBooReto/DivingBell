using UnityEngine;

public class ShipController : MonoBehaviour
{
    public Transform enginePoint;
    public Rigidbody rb;
    public WaterSystems waterSystems;
    public float speed;
    public Vector2 input;

    private void FixedUpdate()
    {
        if (input != Vector2.zero)
        {
            float waterLevel = waterSystems.GetWaterLevel(enginePoint.position);
            if (waterLevel - enginePoint.position.y > 0)
            {
                Vector3 direction = transform.right * input.x;
                direction += transform.forward * input.y;
                rb.AddForceAtPosition(direction * speed, enginePoint.position);
            }
        }
    }

}
