using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAlignerNoOverlap : MonoBehaviour
{
    public GameObject[] itemsToPickFrom;
    public float raycastDistance = 100f;
    public float overlapTestBoxSize = 1f;
    public LayerMask spawnedObjectLayer;
    public int numberoftrees = 10;
    public float spawnRadius = 50f;

    // Start is called before the first frame update
    void Update()
    {
        if (numberoftrees >= 1)
        {
            PositionRaycast();
        }
    }

    void PositionRaycast()
    {
        Vector3 randomPos = new Vector3
        (Random.Range(-spawnRadius, spawnRadius), transform.position.y, Random.Range(-spawnRadius, spawnRadius));
        
        
        
        RaycastHit hit;

        if (Physics.Raycast(randomPos, Vector3.down, out hit, raycastDistance))
        {

            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Vector3 overlapTestBoxScale = new Vector3(overlapTestBoxSize, overlapTestBoxSize, overlapTestBoxSize);
            Collider[] collidersInsideOverlapBox = new Collider[1];
            int numberOfCollidersFound = Physics.OverlapBoxNonAlloc(hit.point, overlapTestBoxScale, collidersInsideOverlapBox, spawnRotation, spawnedObjectLayer);
            
            if (numberOfCollidersFound == 0 && numberoftrees >= 1)
            {
                Pick(hit.point, spawnRotation);
                numberoftrees -= 1;
                Debug.Log(numberoftrees);
            }
        }
    }

    void Pick(Vector3 positionToSpawn, Quaternion rotationToSpawn)
    {
        int randomIndex = Random.Range(0, itemsToPickFrom.Length);
        rotationToSpawn = Quaternion.Euler(0, Random.Range(360, 0), 0);
        GameObject clone = Instantiate(itemsToPickFrom[randomIndex], positionToSpawn, rotationToSpawn);
    }
    // void SpawnTrees()
    // {
    //     for(int i = 0; i < numberoftrees; i++)
    //     {
    //         PositionRaycast();
    //     }
    // }
}