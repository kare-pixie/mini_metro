using UnityEngine;
using UnityEngine.EventSystems;

public class ReturnOnClick : MonoBehaviour, IPointerUpHandler
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
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        Camera.main.orthographicSize = 2;
        Camera.main.transform.position = new Vector3(-0.6f, 0, -10);
        screenCapture.CaptureScreenshot("IntroScene", false);
    }
}
