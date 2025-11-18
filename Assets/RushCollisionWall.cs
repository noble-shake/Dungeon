using UnityEngine;

public class RushCollisionWall : MonoBehaviour, IDestructable
{


    public void BeingDestructed()
    {
    }

    public string GetDestructAnimation()
    {
        return string.Empty;
    }

    public bool isGimmickObject()
    {
        return false;
    }

}
