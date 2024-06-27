using UnityEngine;
using UnityEngine.EventSystems;

public class QuitOnClick : MonoBehaviour, IPointerUpHandler
{
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
        //OnPointerUp 포인터를 오브젝트 에서 뗄 때 호출
        QuitGame();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}