using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class RandomMoverManager_WithSpline : MonoBehaviour
{
    public SplineContainer splineContainer;
    public List<GameObject> avatars;
    public float moveSpeed = 1f;
    public float changeDirectionInterval = 2f;

    private PolygonCollider2D polygonCollider2D;
    private Dictionary<GameObject, float> timeSinceLastChange = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> moveDirections = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        CreatePolygonCollider();

        // 各アバターごとの初期化
        foreach (var avatar in avatars)
        {
            timeSinceLastChange[avatar] = 0f;
            moveDirections[avatar] = GetRandomDirection();
        }
    }

    void Update()
    {
        foreach (var avatar in avatars)
        {
            MoveAndRotate(avatar);
        }
    }

    private void MoveAndRotate(GameObject avatar)
    {
        timeSinceLastChange[avatar] += Time.deltaTime;

        // 一定時間ごとにランダム方向転換
        if (timeSinceLastChange[avatar] > changeDirectionInterval)
        {
            moveDirections[avatar] = GetRandomDirection();
            timeSinceLastChange[avatar] = 0f;
        }

        // 移動
        avatar.transform.position += moveDirections[avatar] * moveSpeed * Time.deltaTime;

        // 範囲外なら新しい方向へ変更
        if (!IsInsidePolygon(avatar.transform.position))
        {
            moveDirections[avatar] = GetRandomDirection();
            timeSinceLastChange[avatar] = 0f;
        }

        // 向きを移動方向に合わせる
        if (moveDirections[avatar] != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirections[avatar], Vector3.up);
            avatar.transform.rotation = Quaternion.Slerp(avatar.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// Spline から PolygonCollider2D を作成 (ワールド座標対応)
    /// </summary>
    private void CreatePolygonCollider()
    {
        GameObject colliderObject = new GameObject("SplinePolygonCollider");
        colliderObject.transform.SetParent(transform);  // 同階層に配置
        polygonCollider2D = colliderObject.AddComponent<PolygonCollider2D>();

        var spline = splineContainer.Spline;
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < spline.Count; i++)
        {
            var localPos = spline[i].Position;
            var worldPos = splineContainer.transform.TransformPoint(localPos);  // ローカル → ワールド変換
            points.Add(new Vector2(worldPos.x, worldPos.z));  // XZ 平面だけ使う
        }

        polygonCollider2D.SetPath(0, points.ToArray());
    }

    /// <summary>
    /// アバターが PolygonCollider2D 内かどうか
    /// </summary>
    private bool IsInsidePolygon(Vector3 position)
    {
        Vector2 pos2D = new Vector2(position.x, position.z);
        return polygonCollider2D.OverlapPoint(pos2D);
    }

    /// <summary>
    /// ランダム方向ベクトル
    /// </summary>
    private Vector3 GetRandomDirection()
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        return new Vector3(dir2D.x, 0, dir2D.y);
    }
}
