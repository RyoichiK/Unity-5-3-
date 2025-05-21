using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassTestmain : MonoBehaviour
{
    [SerializeField] Image image;            // 回転させる矢印Image
    [SerializeField] Transform player;       // 回転の元（カメラやプレイヤー）
    [SerializeField] Text text;              // デバッグ表示用
    [SerializeField] float angleOffset = 0f; // 任意のオフセット（例：東を基準にしたい等）

    RectTransform rt;
    float resetOffset = 0f; // Pキーでリセットされる角度オフセット

    void Start()
    {
        rt = image.rectTransform;
    }

    void Update()
    {
        // Pキーが押されたら、現在の角度を基準（0度）にリセット
        if (Input.GetKeyDown(KeyCode.P))
        {
            resetOffset = player.eulerAngles.y;
        }

        // 現在の角度からリセットオフセットを引き、angleOffset を加算、0〜360に正規化
        float yaw = (player.eulerAngles.y - resetOffset + angleOffset + 360f) % 360f;

        // UIのZ軸のみ回転させる（2D UI的な挙動）
        rt.localEulerAngles = new Vector3(0f, 0f, -yaw);

        // テキスト表示（任意）
        if (text != null)
        {
            text.text = $"Yaw: {yaw:F1}°";
        }
    }
}
