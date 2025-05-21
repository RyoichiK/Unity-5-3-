using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // �ړ����x
    public float rotationSpeed = 700.0f; // ��]���x

    void Update()
    {
        // �L�[�{�[�h���͂��擾
        float moveDirection = -Input.GetAxis("Vertical"); // �O��ړ��i�t�ɂ��邽�߂Ƀ}�C�i�X���|����j
        float turnDirection = Input.GetAxis("Horizontal"); // ���E��]

        // �ړ�����
        transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);

        // ��]����
        transform.Rotate(Vector3.up, turnDirection * rotationSpeed * Time.deltaTime);
    }
}