using UnityEngine;

public interface IDestructable
{
    public bool isGimmickObject();
    public void BeingDestructed();
    public string GetDestructAnimation();
}
