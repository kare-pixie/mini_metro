using UnityEngine;
using UnityEngine.EventSystems;

public class PlayOnClick : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private ScreenCapture screenCapture;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        screenCapture.CaptureScreenshot("MainScene", false);
    }
}
