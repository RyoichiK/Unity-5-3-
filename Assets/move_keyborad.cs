using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 移動速度
    public float rotationSpeed = 700.0f; // 回転速度

    void Update()
    {
        // キーボード入力を取得
        float moveDirection = -Input.GetAxis("Vertical"); // 前後移動（逆にするためにマイナスを掛ける）
        float turnDirection = Input.GetAxis("Horizontal"); // 左右回転

        // 移動処理
        transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);

        // 回転処理
        transform.Rotate(Vector3.up, turnDirection * rotationSpeed * Time.deltaTime);
    }
}