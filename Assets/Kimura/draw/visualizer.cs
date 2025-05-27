using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class EnergyMapDrawer_BrailleHighlight_FrontBack10m : MonoBehaviour
{
    [Header("描画対象")]
    public Renderer targetRenderer;
    public List<Transform> people;

    [Header("テクスチャ設定")]
    public int textureSize = 512;

    [Header("エネルギー描画パラメータ")]
    public float sigma = 20f;
    public float gamma = 0.8f;
    public float beta = 0.6f;
    public float w_scale = 1.0f;
    public float alphaMax = 1.0f;
    public float maxDistance = 30f;

    [Header("カメラ距離制限")]
    public Transform cameraTransform;
    public float cameraRangeXZ = 30f;

    [Header("動的描写角度")]
    public float angleMin = 60f;
    public float angleMax = 120f;
    public float distMin = 5f;
    public float distMax = 13f;

    [Header("歩道エリア")]
    public List<SplineContainer> sidewalkPolygons;

    [Header("点字ブロックエリア")]
    public List<SplineContainer> braillePolygons;
    public float brailleForwardDistance = 10f;

    [Header("路面エリア")]
    public List<SplineContainer> roadPolygons;

    [Header("ログ連携")]
    public CompassLogger_WithBrailleFlag compassLogger;

    public int polygonSampleCount = 300;

    private Texture2D energyTexture;
    private float[,] energyBuffer;
    private bool[,] sidewalkCache;
    private bool[,] roadCache;
    private List<Vector2[]> sidewalkPointsList;
    private List<Vector2[]> braillePointsList;
    private List<Vector2[]> roadPointsList;

    private Vector3 planeCenter;
    private float planeSizeX;
    private float planeSizeZ;

    void Start()
    {
        planeCenter = targetRenderer.bounds.center;
        planeSizeX = targetRenderer.bounds.size.x;
        planeSizeZ = targetRenderer.bounds.size.z;

        targetRenderer.material.shader = Shader.Find("Unlit/Transparent");

        energyTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        energyTexture.wrapMode = TextureWrapMode.Clamp;
        targetRenderer.material.mainTexture = energyTexture;

        targetRenderer.material.SetFloat("_Mode", 3);
        targetRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        targetRenderer.material.renderQueue = 3000;

        sidewalkPointsList = CachePolygonPoints(sidewalkPolygons);
        braillePointsList = CachePolygonPoints(braillePolygons);
        roadPointsList = CachePolygonPoints(roadPolygons);

        sidewalkCache = new bool[textureSize, textureSize];
        roadCache = new bool[textureSize, textureSize];

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                Vector2 worldXY = PixelToWorld(x, y);
                sidewalkCache[x, y] = IsInsideAnyPolygon(worldXY, sidewalkPointsList);
                roadCache[x, y] = IsInsideAnyPolygon(worldXY, roadPointsList);
            }
        }
    }

    void Update()
    {
        DrawEnergyMap();
    }

    void DrawEnergyMap()
    {
        Color[] pixels = new Color[textureSize * textureSize];
        energyBuffer = new float[textureSize, textureSize];

        bool anyPersonInBrailleBlock = false;

        Camera targetCamera = cameraTransform.GetComponent<Camera>();
        bool isSolidColor = targetCamera != null && targetCamera.clearFlags == CameraClearFlags.SolidColor;

        foreach (Transform person in people)
        {
            Vector2 personXZ = new Vector2(person.position.x, person.position.z);
            Vector2 cameraXZ = new Vector2(cameraTransform.position.x, cameraTransform.position.z);
            float dist = Vector2.Distance(personXZ, cameraXZ);
            if (dist > cameraRangeXZ) continue;

            float dynamicAngle = Mathf.Lerp(angleMin, angleMax, Mathf.InverseLerp(distMin, distMax, dist));

            Vector2 apex;
            Vector2 dir;
            GetPersonApexAndDir(person, out apex, out dir);

            int matchedSplineIndex = GetContainingPolygonIndex(personXZ, braillePointsList);
            if (matchedSplineIndex != -1)
            {
                anyPersonInBrailleBlock = true;

                Vector2[] polygon = braillePointsList[matchedSplineIndex];
                for (int y = 0; y < textureSize; y++)
                {
                    for (int x = 0; x < textureSize; x++)
                    {
                        Vector2 worldXY = PixelToWorld(x, y);
                        if (!IsInsidePolygon(worldXY, polygon)) continue;

                        Vector2 rel = worldXY - personXZ;
                        float d = rel.magnitude;
                        float forwardDot = Vector2.Dot(dir.normalized, rel.normalized);
                        if (d <= brailleForwardDistance && Mathf.Abs(forwardDot) > 0.8f)
                        {
                            energyBuffer[x, y] = 1.0f;
                        }
                    }
                }
            }

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    if (!sidewalkCache[x, y]) continue; // 歩道エリアのみ描写

                    Vector2 pos = new Vector2(x, y);
                    Vector2 rel = pos - apex;
                    float d = rel.magnitude;
                    float theta = Vector2.Angle(dir, rel);
                    if (d < 0.01f || d > maxDistance || theta > dynamicAngle / 2f) continue;

                    float Atheta = Mathf.Pow(Mathf.Max(Vector2.Dot(dir, rel.normalized), 0f), gamma);
                    float Gdist = Mathf.Exp(-(d * d) / (2f * sigma * sigma));
                    float coneMask = 1f - Mathf.Pow(theta / (dynamicAngle / 2f), 2f);

                    float E = Atheta * Gdist * coneMask;
                    float energy = Mathf.Clamp01(E * w_scale);
                    energy = Mathf.Pow(energy, beta);
                    energyBuffer[x, y] += energy;
                }
            }
        }

        if (compassLogger != null)
        {
            compassLogger.isPersonInBrailleBlock = anyPersonInBrailleBlock;
        }

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                int idx = y * textureSize + x;
                if (roadCache[x, y])
                {
                    pixels[idx] = new Color(0.5f, 0.5f, 0.5f, 1f);
                    continue;
                }

                if (!sidewalkCache[x, y])
                {
                    pixels[idx] = new Color(0f, 0f, 1f, 0f);
                    continue;
                }

                float energy = Mathf.Clamp01(energyBuffer[x, y]);
                Color baseColor;
                if (energy < 0.5f)
                    baseColor = Color.Lerp(Color.blue, new Color(1f, 0.5f, 0f), energy * 2f);
                else
                    baseColor = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, (energy - 0.5f) * 2f);

                float alpha = isSolidColor ? 1f : (energy > 0f ? energy * alphaMax : 0.2f);

                pixels[idx] = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            }
        }

        energyTexture.SetPixels(pixels);
        energyTexture.Apply();
    }

    List<Vector2[]> CachePolygonPoints(List<SplineContainer> containers)
    {
        List<Vector2[]> list = new List<Vector2[]>();
        foreach (var container in containers)
        {
            if (container == null || container.Splines.Count == 0) continue;

            var spline = container.Splines[0];
            Vector2[] points = new Vector2[polygonSampleCount];
            for (int i = 0; i < polygonSampleCount; i++)
            {
                float t = i / (float)(polygonSampleCount - 1);
                Vector3 ptLocal = spline.EvaluatePosition(t);
                Vector3 ptWorld = container.transform.TransformPoint(ptLocal);
                points[i] = new Vector2(ptWorld.x, ptWorld.z);
            }
            list.Add(points);
        }
        return list;
    }

    bool IsInsideAnyPolygon(Vector2 point, List<Vector2[]> polygonList)
    {
        foreach (var polygon in polygonList)
        {
            if (IsInsidePolygon(point, polygon)) return true;
        }
        return false;
    }

    int GetContainingPolygonIndex(Vector2 point, List<Vector2[]> polygons)
    {
        for (int i = 0; i < polygons.Count; i++)
        {
            if (IsInsidePolygon(point, polygons[i]))
                return i;
        }
        return -1;
    }

    bool IsInsidePolygon(Vector2 point, Vector2[] polygon)
    {
        int count = polygon.Length;
        bool inside = false;
        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            if ((polygon[i].y > point.y) != (polygon[j].y > point.y) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) /
                 (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    void GetPersonApexAndDir(Transform person, out Vector2 apex, out Vector2 dir)
    {
        float u = Mathf.InverseLerp(planeCenter.x + planeSizeX / 2f, planeCenter.x - planeSizeX / 2f, person.position.x);
        float v = Mathf.InverseLerp(planeCenter.z + planeSizeZ / 2f, planeCenter.z - planeSizeZ / 2f, person.position.z);

        apex = new Vector2(u * textureSize, v * textureSize);
        apex.x = Mathf.Clamp(apex.x, 0, textureSize - 1);
        apex.y = Mathf.Clamp(apex.y, 0, textureSize - 1);

        Vector3 localDir = targetRenderer.transform.InverseTransformDirection(person.forward);
        dir = -new Vector2(localDir.x, localDir.z).normalized;
    }

    Vector2 PixelToWorld(int x, int y)
    {
        float worldX = Mathf.Lerp(planeCenter.x + planeSizeX / 2f, planeCenter.x - planeSizeX / 2f, x / (float)textureSize);
        float worldZ = Mathf.Lerp(planeCenter.z + planeSizeZ / 2f, planeCenter.z - planeSizeZ / 2f, y / (float)textureSize);
        return new Vector2(worldX, worldZ);
    }
}
