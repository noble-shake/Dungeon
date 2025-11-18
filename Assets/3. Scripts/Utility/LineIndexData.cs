using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LineIndexData", menuName = "ScriptableObjects/LineIndexData")]
public class LineIndexData : ScriptableObject
{
    public List<int> verticalStartIndices = new List<int>();
    public List<int> verticalEndIndices = new List<int>();

    public List<int> horizontalStartIndices = new List<int>();
    public List<int> horizontalEndIndices = new List<int>();

    public void Clear()
    {
        Debug.Log("데이터가 삭제됨");
        verticalStartIndices.Clear();
        verticalEndIndices.Clear();
        horizontalStartIndices.Clear();
        horizontalEndIndices.Clear();
    }
}
