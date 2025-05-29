using UnityEngine;
using System.Collections.Generic;

public class PathLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector3> pathPositions = new List<Vector3>();
    public Transform bicycleTransform;
    public float forwardDistance = 10f; // 左方向10mの範囲
    public float verticalOffset = 0.14f; // 線を地面から浮かせる高さ（調整済み）
    public float lineWidth = 0.1f; // 線の太さ（調整済み）

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // LineRenderer設定
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCornerVertices = 5;

        lineRenderer.alignment = LineAlignment.View;

        lineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0, 1f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1, 1f)
        );
    }

    void Update()
    {
        if (bicycleTransform == null || pathPositions.Count < 2) return;

        Vector3 bikePos = bicycleTransform.position;
        Vector3 bikeForward = new Vector3(bicycleTransform.forward.x, 0f, bicycleTransform.forward.z).normalized;

        // 左方向（forwardから-90°回転）
        Vector3 bikeLeft = Quaternion.AngleAxis(-90f, Vector3.up) * bikeForward;

        List<Vector3> filteredPositions = new List<Vector3>();

        foreach (var pos in pathPositions)
        {
            Vector3 toPoint = new Vector3(pos.x - bikePos.x, 0f, pos.z - bikePos.z);
            float distance = toPoint.magnitude;

            if (distance <= forwardDistance) // 左側前方180度内（条件緩和）
            {
                filteredPositions.Add(new Vector3(pos.x, pos.y + verticalOffset, pos.z));
            }
        }

        if (filteredPositions.Count >= 2)
        {
            lineRenderer.positionCount = filteredPositions.Count;
            lineRenderer.SetPositions(filteredPositions.ToArray());
            Debug.Log($"[PathLineRenderer] Drawing {filteredPositions.Count} leftward points within {forwardDistance}m (180 degrees).");
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
