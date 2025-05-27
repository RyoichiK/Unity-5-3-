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
            Debug.LogError("? targetMaterial ‚Ü‚½‚Í maskCameraDepthTexture ‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñB");
        }
    }
}
