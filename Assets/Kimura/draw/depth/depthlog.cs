using UnityEngine;

public class ApplyMaskCameraTexture : MonoBehaviour
{
    public Material targetMaterial;
    public RenderTexture maskCameraRenderTexture;

    void Start()
    {
        if (targetMaterial != null && maskCameraRenderTexture != null)
        {
            targetMaterial.SetTexture("_MaskCameraTexture", maskCameraRenderTexture);
        }
        else
        {
            Debug.LogError("❌ targetMaterial または maskCameraRenderTexture が設定されていません。");
        }
    }
}
