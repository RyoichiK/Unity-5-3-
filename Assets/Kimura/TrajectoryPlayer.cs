using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrajectoryPlayer : MonoBehaviour
{
    [Header("CSVファイル名（Assetsフォルダ内）")]
    public string csvFileName = "compass_with_braille_log.csv";

    [Header("再生対象（位置と回転を反映）")]
    public Transform target;

    [Header("再生スピード倍率（1 = 実時間）")]
    public float timeScale = 1.0f;

    private List<float> timeList = new List<float>();
    private List<Vector3> positionList = new List<Vector3>();
    private List<Vector3> rotationList = new List<Vector3>(); // ← 回転情報

    private float playbackTime = 0f;
    private int currentIndex = 0;
    private bool isPlaying = false;

    void Start()
    {
        LoadCSV();
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying || timeList.Count == 0 || target == null) return;

        playbackTime += Time.deltaTime * timeScale;

        while (currentIndex < timeList.Count - 1 && playbackTime > timeList[currentIndex + 1])
        {
            currentIndex++;
        }

        if (currentIndex >= timeList.Count - 1)
        {
            isPlaying = false;
            return;
        }

        // --- 位置補間 ---
        float t0 = timeList[currentIndex];
        float t1 = timeList[currentIndex + 1];
        Vector3 p0 = positionList[currentIndex];
        Vector3 p1 = positionList[currentIndex + 1];
        float lerpFactor = (playbackTime - t0) / (t1 - t0);
        target.position = Vector3.Lerp(p0, p1, lerpFactor);

        // --- 回転補間 ---
        Vector3 r0 = rotationList[currentIndex];
        Vector3 r1 = rotationList[currentIndex + 1];
        Quaternion q0 = Quaternion.Euler(r0);
        Quaternion q1 = Quaternion.Euler(r1);
        target.rotation = Quaternion.Slerp(q0, q1, lerpFactor);  // ← スムーズな球面線形補間
    }

    void LoadCSV()
    {
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"❌ CSVファイルが見つかりません: {path}");
            return;
        }

        try
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fs))
            {
                bool isFirstLine = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (isFirstLine)
                    {
                        isFirstLine = false; // ヘッダー行スキップ
                        continue;
                    }

                    var values = line.Split(',');

                    float time = float.Parse(values[1]);
                    float x = float.Parse(values[4]);
                    float y = float.Parse(values[5]);
                    float z = float.Parse(values[6]);

                    float rx = float.Parse(values[7]);
                    float ry = float.Parse(values[8]);
                    float rz = float.Parse(values[9]);

                    timeList.Add(time);
                    positionList.Add(new Vector3(x, y, z));
                    rotationList.Add(new Vector3(rx, ry, rz));
                }
            }

            Debug.Log($"✅ 位置＋回転の軌跡データ {timeList.Count} フレーム分 読み込み完了");
        }
        catch (IOException e)
        {
            Debug.LogError($"📛 CSV読み込みエラー: {e.Message}");
        }
    }
}
