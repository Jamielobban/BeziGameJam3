using UnityEngine;
using UnityEngine.UI;

public class MainTexSet : MonoBehaviour
{
    public Material crtMaterial;       // Assign this in the Inspector
    public RenderTexture gameTexture;  // Your Render Texture (usually assigned via Inspector too)

    public RawImage rawImage;
    void Start()
    {
        rawImage.material.SetTexture("_MainTex", gameTexture);

    }
}
