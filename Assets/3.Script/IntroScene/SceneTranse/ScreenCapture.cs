// ScreenCapture.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenCapture : MonoBehaviour
{
    public RawImage image1;
    public RawImage image2;
    public Camera mainCamera;
    public float animationDuration = 1f; // 애니메이션 지속 시간
    public RenderTexture screenshotTexture;
    private CircularSlider[] circularSlider;

    public void CaptureScreenshot(string sceneName, bool isNext)
    {
        StartCoroutine(CaptureScreenshot_co(sceneName, isNext));
    }
    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    private IEnumerator CaptureScreenshot_co(string sceneName, bool isNext)
    {
        // 현재 프레임 렌더링이 완료될 때까지 대기
        yield return new WaitForEndOfFrame();

        // 기존 카메라 설정을 저장합니다.
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        mainCamera.targetTexture = renderTexture;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        mainCamera.Render();

        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        mainCamera.targetTexture = null;
        RenderTexture.active = null; // 추가: RenderTexture.active를 null로 설정합니다.
        Destroy(renderTexture);

        // 이미지 크기를 화면 크기에 맞게 조절
        image1.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        image2.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        // 캡처한 텍스처를 image1에 할당
        image1.texture = screenShot;


        // 이미지1과 이미지2를 활성화
        image1.gameObject.SetActive(true);
        image2.gameObject.SetActive(true);

        // 이미지를 이동시키는 애니메이션 실행
        StartCoroutine(AnimateImage2_co(sceneName, isNext));
    }

    private IEnumerator AnimateImage2_co(string sceneName, bool isNext)
    {
        circularSlider = FindObjectsOfType<CircularSlider>();
        for (int i = 0; i < circularSlider.Length; i++)
        {
            if (circularSlider[i] != null && circularSlider[i].gameObject.activeSelf)
            {
                circularSlider[i].gameObject.SetActive(false);
            }
        }

        Vector2 startPosition = image2.rectTransform.anchoredPosition;
        Vector2 endPosition;
        if (isNext)
            endPosition = startPosition + new Vector2(-Screen.width, 0); // 옆으로 가는거
        else
            endPosition = startPosition + new Vector2(0, Screen.height); // 밑으로 가는거

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            image1.rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            image2.rectTransform.anchoredPosition = Vector2.Lerp(-endPosition, startPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image1.rectTransform.anchoredPosition = endPosition;
        image2.rectTransform.anchoredPosition = startPosition;
        image1.gameObject.SetActive(false);

        SceneManager.LoadScene(sceneName);
    }
}