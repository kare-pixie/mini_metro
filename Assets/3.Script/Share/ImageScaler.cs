using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ImageScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleFactor = 1.2f;
    public float inactiveAlpha = 0.5f; // ��Ȱ�� ������ ���� ����
    private Vector3 originalScale;
    private TMP_Text textComponent;
    private Color originalColor; // ���� ���� ����� ����

    [SerializeField] private AudioClip waitSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if(textComponent != null)
            originalColor = textComponent.color; // ���� ���� ����
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺�� �÷��� �� �ش� ������Ʈ�� transform ũ�� ����
        transform.localScale = originalScale * scaleFactor;

        // �ٸ� ������Ʈ ã�Ƽ� ũ�� ���� �� ���� ����
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
        // ���콺�� ����� �� �ش� ������Ʈ�� transform ũ�� ������� ����
        transform.localScale = originalScale;

        // �ٸ� ������Ʈ ã�Ƽ� ũ�� �� ���� ������� ����
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