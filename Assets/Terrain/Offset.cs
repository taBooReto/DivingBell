using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Offset", menuName = "Scriptable Objects/Offset")]
public class Offset : ScriptableObject
{
    public static float offsetX = 1488f;
    public static float offsetY = 1337f;

    private void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
    }
}
