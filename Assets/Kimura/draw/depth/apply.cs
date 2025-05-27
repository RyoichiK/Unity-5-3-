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
            Debug.LogError("? targetMaterial �܂��� maskCameraDepthTexture ���ݒ肳��Ă��܂���B");
        }
    }
}
