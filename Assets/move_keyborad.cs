using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 10.0f;

    private float currentMove = 0f;
    private float currentTurn = 0f;
    public float smoothTime = 0.1f; // �X���[�W���O�̑���

    void Update()
    {
        float targetMove = -Input.GetAxisRaw("Vertical");
        float targetTurn = Input.GetAxisRaw("Horizontal");

        // ���X�Ɋ��炩�ɒl��␳����
        currentMove = Mathf.Lerp(currentMove, targetMove, Time.deltaTime / smoothTime);
        currentTurn = Mathf.Lerp(currentTurn, targetTurn, Time.deltaTime / smoothTime);

        transform.Translate(Vector3.right * currentMove * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, currentTurn * rotationSpeed * Time.deltaTime);
    }
}
