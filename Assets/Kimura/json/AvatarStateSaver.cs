using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AvatarStateSaver : MonoBehaviour
{
    [Header("EnergyMapDrawer を参照")]
    public EnergyMapDrawer_BrailleHighlight_FrontBack10m energyDrawer;

    [Header("保存ファイル名（.json含む）")]
    public string fileName = "pattern_01.json";

    // アバター1体分の情報
    [System.Serializable]
    public class AvatarInfo
    {
        public string name;
        public float[] position;
        public float[] rotation;
    }

    // 保存する構成全体
    [System.Serializable]
    public class AvatarConfig
    {
        public string scenario_name;
        public List<AvatarInfo> avatars = new List<AvatarInfo>();
    }

    void Start()
    {
        SaveAvatars(); // ゲーム開始時に保存
    }

    public void SaveAvatars()
    {
        AvatarConfig config = new AvatarConfig();
        config.scenario_name = Path.GetFileNameWithoutExtension(fileName);

        // ✅ EnergyDrawer から people リストを取得
        if (energyDrawer != null && energyDrawer.people != null)
        {
            foreach (var person in energyDrawer.people)
            {
                if (person == null) continue;

                AvatarInfo info = new AvatarInfo();
                info.name = person.name.Replace("(Clone)", "").Trim();

                Vector3 pos = person.position;
                Vector3 rot = person.rotation.eulerAngles;

                info.position = new float[] { pos.x, pos.y, pos.z };
                info.rotation = new float[] { rot.x, rot.y, rot.z };

                config.avatars.Add(info);
            }
        }
        else
        {
            Debug.LogWarning("⚠ EnergyMapDrawer または people リストが未設定です");
        }

        string folderPath = Path.Combine(Application.dataPath, "Resources/AvatarPatterns");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllText(fullPath, JsonUtility.ToJson(config, true));
        Debug.Log("✅ Avatar pattern saved to: " + fullPath);
    }
}
