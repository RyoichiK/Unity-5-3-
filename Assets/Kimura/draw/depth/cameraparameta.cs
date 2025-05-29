//using UnityEngine;

//public class CameraParameterLogger : MonoBehaviour
//{
//    public Camera targetCamera;

//    void Start()
//    {
//        if (targetCamera == null)
//        {
//            Debug.LogError("⚠ targetCamera が設定されていません。");
//            return;
//        }

//        // ワールド位置
//        Vector3 pos = targetCamera.transform.position;
//        Debug.Log($"📍 Position (world): {pos.x}, {pos.y}, {pos.z}");

//        // ワールド回転（Quaternion）
//        Quaternion rot = targetCamera.transform.rotation;
//        Debug.Log($"📍 Rotation (Quaternion): w={rot.w}, x={rot.x}, y={rot.y}, z={rot.z}");

//        // ワールド変換行列（4x4）
//        Matrix4x4 localToWorld = targetCamera.transform.localToWorldMatrix;
//        Debug.Log("📍 LocalToWorldMatrix:");
//        Debug.Log(localToWorld);

//        // Projection Matrix
//        Matrix4x4 projMatrix = targetCamera.projectionMatrix;
//        Debug.Log("📍 ProjectionMatrix:");
//        Debug.Log(projMatrix);

//        // カメラパラメータ
//        float fov = targetCamera.fieldOfView;
//        float aspect = targetCamera.aspect;
//        Debug.Log($"📍 Field of View (vertical): {fov} degrees");
//        Debug.Log($"📍 Aspect Ratio: {aspect} (width/height)");
//    }
//}
//using UnityEngine;

//public class DualCameraCalibrationLogger : MonoBehaviour
//{
//    public Camera topCamera;     // 真上カメラ
//    public Camera frontCamera;   // 前方カメラ
//    public Vector3[] worldPoints;

//    void Start()
//    {
//        if (topCamera == null || frontCamera == null)
//        {
//            Debug.LogError("⚠ topCamera または frontCamera が設定されていません。");
//            return;
//        }

//        Debug.Log("=== キャリブレーションデータ出力開始 ===");

//        foreach (Vector3 worldPoint in worldPoints)
//        {
//            Vector3 screenPointTop = topCamera.WorldToScreenPoint(worldPoint);
//            Vector3 screenPointFront = frontCamera.WorldToScreenPoint(worldPoint);

//            Debug.Log($"World: {worldPoint}");
//            Debug.Log($"  TopCamera Screen: {screenPointTop}");
//            Debug.Log($"  FrontCamera Screen: {screenPointFront}");
//        }

//        Debug.Log($"TopCamera Screen Resolution: {Screen.width} x {Screen.height}");
//        Debug.Log($"FrontCamera Screen Resolution: {Screen.width} x {Screen.height}");
//        Debug.Log("=== キャリブレーションデータ出力完了 ===");
//    }
//}

using UnityEngine;

public class CameraMatrixLogger : MonoBehaviour
{
    public Camera targetCamera;
    public Vector3[] worldPoints;

    void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogError("⚠ targetCamera が設定されていません。");
            return;
        }

        Debug.Log("=== Projection Matrix ===");
        Matrix4x4 proj = targetCamera.projectionMatrix;
        for (int i = 0; i < 4; i++)
        {
            string row = "";
            for (int j = 0; j < 4; j++)
            {
                row += proj[i, j].ToString("F6") + " ";
            }
            Debug.Log(row);
        }

        Debug.Log("=== WorldToScreenPoint Results ===");
        foreach (Vector3 worldPoint in worldPoints)
        {
            Vector3 screenPoint = targetCamera.WorldToScreenPoint(worldPoint);
            Debug.Log($"World: {worldPoint} → Screen: {screenPoint}");
        }

        Debug.Log($"Screen Resolution: {Screen.width} x {Screen.height}");
    }
}
