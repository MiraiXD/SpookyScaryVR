using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastShadowsOff : MonoBehaviour
{
    public string materialName = "szyby";
    public bool off = false;
    public bool allOn = false;
    private void OnValidate()
    {
        if (off || allOn)
        {
            foreach (MeshRenderer m in FindObjectsOfType<MeshRenderer>())
            {
                if (off)
                {
                    foreach (var mat in m.sharedMaterials)
                    {
                        if (mat.name == materialName)
                            m.enabled = false;
                    }
                }
                if (allOn) m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }      
    }
}