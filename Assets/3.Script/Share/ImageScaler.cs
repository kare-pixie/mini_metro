using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ImageScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleFactor = 1.2f;
    public float inactiveAlpha = 0.5f; // 비활성 상태일 때의 투명도
    private Vector3 originalScale;
    private TMP_Text textComponent;
    private Color originalColor; // 원래 색상 저장용 변수

    [SerializeField] private AudioClip waitSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if(textComponent != null)
            originalColor = textComponent.color; // 원래 색상 저장
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스를 올렸을 때 해당 오브젝트의 transform 크기 증가
        transform.localScale = originalScale * scaleFactor;

        // 다른 오브젝트 찾아서 크기 감소 및 투명도 조정
        ImageScaler[] otherScalers = FindObjectsOfType<ImageScaler>();
        foreach (var scaler in otherScalers)
        {
            if (scaler != this)
            {
                scaler.transform.localScale = scaler.originalScale / scaleFactor;
                if(scaler.textComponent != null)
                    scaler.textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, inactiveAlpha);
            }
        }

        if (audioSource != null && waitSound != null)
        {
            audioSource.PlayOneShot(waitSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스를 벗어났을 때 해당 오브젝트의 transform 크기 원래대로 복구
        transform.localScale = originalScale;

        // 다른 오브젝트 찾아서 크기 및 투명도 원래대로 복구
        ImageScaler[] otherScalers = FindObjectsOfType<ImageScaler>();
        foreach (var scaler in otherScalers)
        {
            if (scaler != this)
            {
                scaler.transform.localScale = scaler.originalScale;
                if (scaler.textComponent != null)
                    scaler.textComponent.color = originalColor;
            }
        }
    }
}