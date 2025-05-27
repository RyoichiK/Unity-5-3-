using UnityEngine;

public class MaskCameraSetup : MonoBehaviour
{
    public Camera maskCamera;

    void Start()
    {
        if (maskCamera != null)
        {
            maskCamera.depthTextureMode = DepthTextureMode.Depth;
            Debug.Log("✅ DepthTextureMode set to Depth.");
        }
        else
        {
            Debug.LogWarning("⚠ maskCamera is not assigned.");
        }
    }
}
