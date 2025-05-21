using UnityEngine;
using UnityEngine.UI;

public class CompassTestbike : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Transform player;
    [SerializeField] Text text;
    [SerializeField] float angleOffset = 0f;

    RectTransform rt;
    float resetOffset = 0f;

    void Start()
    {
        rt = image.rectTransform;
    }

    void Update()
    {
        // P キーで現在角度を基準としてリセット
        if (Input.GetKeyDown(KeyCode.P))
        {
            resetOffset = player.eulerAngles.y;
        }

        float yRotation = player.eulerAngles.y - resetOffset + angleOffset;

        if (text != null)
        {
            text.text = "Y Rotation: " + yRotation.ToString("F1") + "°";
        }

        // Z軸だけ回転
        rt.localRotation = Quaternion.Euler(0f, 0f, yRotation);
    }
}
