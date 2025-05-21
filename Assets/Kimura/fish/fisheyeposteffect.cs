using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FisheyePostEffect : MonoBehaviour
{
    public Material fisheyeMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (fisheyeMaterial != null)
        {
            Graphics.Blit(src, dest, fisheyeMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
