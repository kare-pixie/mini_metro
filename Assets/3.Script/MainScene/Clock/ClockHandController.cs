using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockHandController : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private GameObject clockFace;
    [SerializeField] private Sprite face_Sprite1;
    [SerializeField] private Sprite face_Sprite2;
    [SerializeField] private Sprite hand_Sprite1;
    [SerializeField] private Sprite hand_Sprite2;
    private float rotationSpeed = 24f;
    private bool imageChanged = false; // �̹����� ����Ǿ����� ���θ� ����
    private bool rotationTriggered = false; // ȸ���� Ʈ���� �Ǿ����� ����

    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void SetSpeed(int speed)
    {
        rotationSpeed = speed * 24;
        GameManager.Instance.Speed = speed;

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;
        // �� �����Ӹ��� �ð� �ٴ��� ȸ��
        clockHand.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);

        // ���� Z�� ȸ�� ����
        float zRotation = clockHand.transform.eulerAngles.z;

        // �� ����(180��) ������ �� �̹����� ����
        //if (zRotation >= 179.4f && zRotation <= 180.6f && !imageChanged)

        if (!rotationTriggered && (zRotation >= 179f && zRotation < 180f))
        {
            if (!imageChanged)
            {
                ChangeClockHandImage(clockFace, face_Sprite2);
                ChangeClockHandImage(clockHand, hand_Sprite2);
            }
            else
            {
                ChangeClockHandImage(clockFace, face_Sprite1);
                ChangeClockHandImage(clockHand, hand_Sprite1);
            }
            imageChanged = !imageChanged;
            rotationTriggered = true; // Ʈ���� ����
            GameManager.Instance.AddDay(1);
        }

        // �� ����(360��) ������ �� Ʈ���� �ʱ�ȭ
        if (rotationTriggered && (zRotation >= 0f && zRotation <= 1.2f))
        {
            rotationTriggered = false; // Ʈ���� �ʱ�ȭ
        }
    }

    private void ChangeClockHandImage(GameObject gameObject, Sprite sprite)
    {
        // clockHand ������Ʈ�� Image ������Ʈ�� ã�� �̹��� ����
        Image imageComponent = gameObject.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = sprite;
        }
    }
}
