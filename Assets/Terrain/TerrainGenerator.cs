using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 256; 
    public int height = 256; 

    public int depth = 20;

    public float scale = 20f;

    // public float offsetX = 100f;
    // public float offsetY = 100f;

    [SerializeField] private float terrainOffsetX;
    [SerializeField] private float terrainOffsetY;

    [SerializeField] private float OffX;
    [SerializeField] private float OffY;


    private void Start()
    {
        // offsetX = Random.Range(0f, 9999f);
        // offsetY = Random.Range(0f, 9999f);

        //OffX - y coord
        //OffY - X coord
        
        terrainOffsetX = transform.position.x;
        terrainOffsetY = transform.position.z;
        
        Debug.Log(terrainOffsetX);
        Debug.Log(terrainOffsetY);


        OffX = Offset.offsetX + terrainOffsetX;
        OffY = Offset.offsetY + terrainOffsetY;

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }


    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
        
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width + 1, height + 1];
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = ((float)x + terrainOffsetY) / width * scale + Offset.offsetY;
        float yCoord = ((float)y + terrainOffsetX) / height * scale + Offset.offsetX;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }


    
}
