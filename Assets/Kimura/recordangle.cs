using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;  // Playモード終了用
#endif

public class CompassLogger_WithBrailleFlag : MonoBehaviour
{
    [Header("回転UI矢印")]
    [SerializeField] RectTransform compassImage;

    [Header("CSVファイル名")]
    [SerializeField] string fileName = "compass_with_braille_log.csv";

    [Header("点字ブロックに人物がいるか（毎フレーム更新）")]
    public bool isPersonInBrailleBlock = false;

    [Header("追跡対象（位置＋回転）")]
    public Transform trackedTarget;

    private List<string> logLines = new List<string>();
    private int frameCount = 0;

    void Start()
    {
        logLines.Add("Frame,Time,Angle,BrailleFlag,PosX,PosY,PosZ,RotX,RotY,RotZ");
    }

    void Update()
    {
        float rawAngle = compassImage.eulerAngles.z;
        float angle = NormalizeAngle180(rawAngle);
        float time = Time.time;

        int brailleFlag = isPersonInBrailleBlock ? 1 : 0;

        Vector3 pos = trackedTarget != null ? trackedTarget.position : Vector3.zero;
        Vector3 rot = trackedTarget != null ? trackedTarget.eulerAngles : Vector3.zero;

        logLines.Add($"{frameCount},{time:F3},{angle:F2},{brailleFlag},{pos.x:F3},{pos.y:F3},{pos.z:F3},{rot.x:F2},{rot.y:F2},{rot.z:F2}");

        frameCount++;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveToCSV();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }

    void SaveToCSV()
    {
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllLines(path, logLines, System.Text.Encoding.UTF8);
        Debug.Log($"📁 ログを保存しました: {path}");
    }

    float NormalizeAngle180(float angle)
    {
        angle = (angle + 180f) % 360f;
        if (angle < 0) angle += 360f;
        return angle - 180f;
    }
}
