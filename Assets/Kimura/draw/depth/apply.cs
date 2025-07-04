using UnityEngine;

public class ApplyMaskCameraDepth : MonoBehaviour
{
    public Material targetMaterial;
    public RenderTexture maskCameraDepthTexture;

    void Start()
    {
        if (targetMaterial != null && maskCameraDepthTexture != null)
        {
            targetMaterial.SetTexture("_MaskCameraDepthTexture", maskCameraDepthTexture);
        }
        else
        {
            Debug.LogError("? targetMaterial または maskCameraDepthTexture が設定されていません。");
        }
    }
}
