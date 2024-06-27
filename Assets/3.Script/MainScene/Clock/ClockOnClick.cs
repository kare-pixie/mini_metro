using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockOnClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField] private float scaleFactor = 1.2f;
    private Vector3 originalScale;
    private Vector3 clock_originalScale;
    [SerializeField] GameObject clockObject;
    private bool isOpen = false;
    [SerializeField] GameObject pauseObject;
    [SerializeField] GameObject playObject;
    [SerializeField] GameObject fastObject;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        clock_originalScale = clockObject.transform.localScale;

        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            pauseObject.SetActive(true);
            playObject.SetActive(true);
            fastObject.SetActive(true);
        }
        else
        {
            pauseObject.SetActive(false);
            playObject.SetActive(false);
            fastObject.SetActive(false);
        }

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스를 올렸을 때 해당 오브젝트의 transform 크기 증가
        transform.localScale = originalScale * scaleFactor;
        clockObject.transform.localScale = clock_originalScale * scaleFactor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스를 벗어났을 때 해당 오브젝트의 transform 크기 원래대로 복구
        transform.localScale = originalScale;
        clockObject.transform.localScale = clock_originalScale;
    }
}
