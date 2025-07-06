using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static AvatarStateSaver;

public class AvatarStateLoader : MonoBehaviour
{
    [Header("EnergyMapDrawer を参照")]
    public EnergyMapDrawer_BrailleHighlight_FrontBack10m energyDrawer;

    [Header("読み込むJSONファイル名（.json無し）")]
    public string fileName = "pattern_01";  // ✅ 拡張子なしにする

    // ゲーム開始時に自動で読み込みを実行
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("▶ LoadAvatars() 呼び出し");
        LoadAvatars();
    }

    public void LoadAvatars()
    {
        Debug.Log("★ LoadAvatars 実行中");

        TextAsset jsonFile = Resources.Load<TextAsset>("AvatarPatterns/" + fileName);
        if (jsonFile == null)
        {
            Debug.LogWarning("⚠ JSONファイルが見つかりません: " + fileName);
            return;
        }

        AvatarConfig config = JsonUtility.FromJson<AvatarConfig>(jsonFile.text);
        int matchCount = 0;

        if (energyDrawer == null || energyDrawer.people == null)
        {
            Debug.LogWarning("⚠ EnergyDrawer または people が未設定");
            return;
        }

        List<Transform> targetAvatars = energyDrawer.people;

        foreach (var info in config.avatars)
        {
            Transform avatar = targetAvatars.Find(obj => obj.name.StartsWith(info.name));
            if (avatar == null)
            {
                Debug.LogWarning("⚠ 対象アバターが見つかりません: " + info.name);
                continue;
            }

            Vector3 pos = new Vector3(info.position[0], info.position[1], info.position[2]);
            Vector3 rot = new Vector3(info.rotation[0], info.rotation[1], info.rotation[2]);

            avatar.position = pos;
            avatar.rotation = Quaternion.Euler(rot);

            matchCount++;
        }

        Debug.Log($"✅ アバターの位置と回転を復元しました（{matchCount}/{config.avatars.Count}）");
    }
}
