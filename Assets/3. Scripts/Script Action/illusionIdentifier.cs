using UnityEngine;

public class illusionIdentifier : MonoBehaviour
{
    private SkinnedMeshRenderer[] m_renderers;

    private void Start()
    {
        m_renderers = this.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var msh in m_renderers)
        { 
            Material newMat = Instantiate(msh.material);
            newMat.color = Color.black;
            msh.material = newMat;
        }
    }
}