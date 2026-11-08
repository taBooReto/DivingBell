using System.Collections.Generic;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    [System.Serializable]
    private class RopeChain
    {
        public Transform[] points;

        [HideInInspector] public List<Transform> segments = new();
        [HideInInspector] public List<Material> materials = new();
        [HideInInspector] public List<float> previousLengths = new();
        [HideInInspector] public List<float> initialTilings = new();
    }

    [Header("Rope")]
    [SerializeField] private GameObject _rope;
    [SerializeField] private RopeChain[] _chains;

    [Header("Texture")]
    [SerializeField] private float _tilingPerUnit = 1f;

    private float _scrollOffset;

    private const string TextureName = "_BaseColorMap";
    private const float BaseSegmentLength = 1f;

    private void Start()
    {
        foreach (RopeChain chain in _chains)
        {
            for (int i = 0; i < chain.points.Length - 1; i++)
            {
                GameObject rope = Instantiate(_rope);

                chain.segments.Add(rope.transform);

                Renderer renderer = rope.GetComponentInChildren<Renderer>();

                Material material = new Material(renderer.sharedMaterial);
                renderer.sharedMaterial = material;

                chain.materials.Add(material);

                float length = Vector3.Distance(
                    chain.points[i].position,
                    chain.points[i + 1].position
                );

                chain.previousLengths.Add(length);

                float initialTiling = material.GetTextureScale(TextureName).y;
                chain.initialTilings.Add(initialTiling);
            }
        }
    }

    private void Update()
    {
        float totalDelta = 0f;

        foreach (RopeChain chain in _chains)
        {
            for (int i = 0; i < chain.segments.Count; i++)
            {
                float length = Vector3.Distance(
                    chain.points[i].position,
                    chain.points[i + 1].position
                );

                totalDelta += length - chain.previousLengths[i];
                chain.previousLengths[i] = length;
            }
        }

        _scrollOffset -= totalDelta * _tilingPerUnit;

        float accumulatedLength = 0f;

        foreach (RopeChain chain in _chains)
        {
            for (int i = 0; i < chain.segments.Count; i++)
            {
                Vector3 start = chain.points[i].position;
                Vector3 end = chain.points[i + 1].position;

                Vector3 direction = end - start;
                float length = direction.magnitude;

                Transform segment = chain.segments[i];

                segment.position = start;

                if (length > 0f)
                {
                    segment.rotation = Quaternion.FromToRotation(
                        Vector3.up,
                        direction
                    );
                }

                Vector3 scale = segment.localScale;
                scale.y = length;
                segment.localScale = scale;

                Material material = chain.materials[i];

                // Tiling відносно базової довжини 1
                float tilingY = chain.initialTilings[i]
                    * (length / BaseSegmentLength);

                material.SetTextureScale(
                    TextureName,
                    new Vector2(1f, tilingY)
                );

                // Зміщення текстури
                float offsetY = _scrollOffset
                    + accumulatedLength * _tilingPerUnit;

                material.SetTextureOffset(
                    TextureName,
                    new Vector2(0f, offsetY)
                );

                accumulatedLength += length;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (RopeChain chain in _chains)
        {
            foreach (Material material in chain.materials)
            {
                if (material != null)
                    Destroy(material);
            }
        }
    }
}