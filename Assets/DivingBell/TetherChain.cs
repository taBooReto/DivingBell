
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TetherChain : MonoBehaviour
{
    public Rigidbody Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }
}
