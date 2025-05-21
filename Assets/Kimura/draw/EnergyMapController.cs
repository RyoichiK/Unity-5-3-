using UnityEngine;
using System.Collections.Generic;

public class EnergyMapController : MonoBehaviour
{
    public int resolution = 100;
    public float worldSize = 19f; // 描画範囲の物理サイズ
    public Material targetMaterial;
    public List<Transform> movingPeople;

    public Texture2D sidewalkMask; // 白黒マスク画像
    public float gamma = 0.5f;
    public float sigma = 10f;
    public float angle = 180f;

    private Texture2D energyTex;
    private Color[] pixels;

    void Start()
    {
        energyTex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        pixels = new Color[resolution * resolution];
        energyTex.wrapMode = TextureWrapMode.Clamp;
        targetMaterial.mainTexture = energyTex;
    }

    void Update()
    {
        if (movingPeople.Count == 0) return;

        // 基準位置（人物の中心座標）を取得
        Vector3 center = Vector3.zero;
        foreach (var p in movingPeople) center += p.position;
        center /= movingPeople.Count;

        // 描画エリア初期化
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 worldPos = PixelToWorld(x, y, center);
                if (IsInSidewalkArea(worldPos, center))
                    pixels[y * resolution + x] = Color.blue;
                else
                    pixels[y * resolution + x] = Color.clear;
            }
        }

        // 各人物に対してエネルギー描写
        foreach (var person in movingPeople)
        {
            Vector3 pos = person.position;
            float yDeg = person.eulerAngles.y;
            float rad = yDeg * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)).normalized;

            ApplyEnergy(pos, dir, center);
        }

        energyTex.SetPixels(pixels);
        energyTex.Apply();
    }

    void ApplyEnergy(Vector3 pos3D, Vector2 dir, Vector3 baseCenter)
    {
        Vector2 pos = new Vector2(pos3D.x, pos3D.z);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 worldPos = PixelToWorld(x, y, baseCenter);
                if (!IsInSidewalkArea(worldPos, baseCenter)) continue;

                Vector2 rel = worldPos - pos;
                float d = rel.magnitude;
                if (d < 0.01f) continue;

                float theta = Vector2.Angle(dir, rel);
                if (theta > angle / 2f) continue;

                float Atheta = Mathf.Pow(Mathf.Max(Vector2.Dot(dir, rel.normalized), 0), gamma);
                float Gdist = Mathf.Exp(-Mathf.Pow(d, 2) / (2 * Mathf.Pow(sigma, 2)));
                float coneMask = 1 - Mathf.Pow((theta / (angle / 2f)), 2);
                float energy = Atheta * Gdist * coneMask;
                energy = Mathf.Clamp01(energy);

                if (energy > 0.001f)
                {
                    int idx = y * resolution + x;

                    Color color;
                    if (energy < 0.5f)
                        color = Color.Lerp(Color.blue, Color.yellow, energy * 2f);
                    else
                        color = Color.Lerp(Color.yellow, Color.red, (energy - 0.5f) * 2f);

                    pixels[idx] = color;
                }
            }
        }
    }

    Vector2 PixelToWorld(int x, int y, Vector3 baseCenter)
    {
        float step = worldSize / resolution;
        return new Vector2(
            baseCenter.x - worldSize / 2 + x * step,
            baseCenter.z - worldSize / 2 + y * step
        );
    }

    bool IsInSidewalkArea(Vector2 worldPos, Vector3 baseCenter)
    {
        if (sidewalkMask == null) return true;

        float u = Mathf.InverseLerp(baseCenter.x - worldSize / 2, baseCenter.x + worldSize / 2, worldPos.x);
        float v = Mathf.InverseLerp(baseCenter.z - worldSize / 2, baseCenter.z + worldSize / 2, worldPos.y);

        Color c = sidewalkMask.GetPixelBilinear(u, v);
        return c.r > 0.5f;
    }
}
