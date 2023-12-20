using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAlphaControl : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    private Material material;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    public void SetAlpha(bool action)
    {
        if (action)
        {
            material.SetColor("_Color",new Color(1f,1f,1f,0f));
        }
        else
        {
            material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
        }
    }
}
