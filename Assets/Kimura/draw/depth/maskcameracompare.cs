using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MaskCameraDepthExporter : MonoBehaviour
{
    public Shader depthShader; // Custom/DepthExportShader
    public RenderTexture depthRenderTexture;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (depthShader == null)
        {
            Debug.LogError("❌ DepthExportShader が設定されていません。");
            return;
        }
        if (depthRenderTexture == null)
        {
            Debug.LogError("❌ depthRenderTexture が設定されていません。");
            return;
        }

        cam.targetTexture = depthRenderTexture;
        cam.SetReplacementShader(depthShader, null);
    }
}
