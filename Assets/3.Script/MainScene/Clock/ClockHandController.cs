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
    private bool imageChanged = false; // 이미지가 변경되었는지 여부를 추적
    private bool rotationTriggered = false; // 회전이 트리거 되었는지 추적

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
        // 매 프레임마다 시계 바늘을 회전
        clockHand.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);

        // 현재 Z축 회전 각도
        float zRotation = clockHand.transform.eulerAngles.z;

        // 반 바퀴(180도) 돌았을 때 이미지를 변경
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
            rotationTriggered = true; // 트리거 설정
            GameManager.Instance.AddDay(1);
        }

        // 한 바퀴(360도) 돌았을 때 트리거 초기화
        if (rotationTriggered && (zRotation >= 0f && zRotation <= 1.2f))
        {
            rotationTriggered = false; // 트리거 초기화
        }
    }

    private void ChangeClockHandImage(GameObject gameObject, Sprite sprite)
    {
        // clockHand 오브젝트의 Image 컴포넌트를 찾아 이미지 변경
        Image imageComponent = gameObject.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = sprite;
        }
    }
}
