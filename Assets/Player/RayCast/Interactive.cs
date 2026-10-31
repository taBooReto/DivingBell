using UnityEngine;

public interface Interactive
{
    bool Interaction(GameObject interactor) => false;

    void Release() { }
}