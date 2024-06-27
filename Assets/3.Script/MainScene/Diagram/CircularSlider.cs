using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircularSlider : MonoBehaviour
{
    private DiagramControll diagram;
    public Image cicleImage;
    private float maxValue = 60f;
    private float currentValue = 0f;
    private float changeSpeed = 0.5f; // 슬라이더 변화 속도
    public float targetSize = 1f; // 목표 카메라 확대 크기
    public float targetRotationAngle = 90f; // 목표 회전 각도

    private float targetValue = 0f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(Stop_co());
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;

        currentValue = diagram.GameoverCount;
        currentValue = Mathf.MoveTowards(currentValue, targetValue, Time.deltaTime * changeSpeed);
        UpdateSlider(currentValue / maxValue);
        if (currentValue >= maxValue)
        {
            GameManager.Instance.isGameOver = true;
            StartCoroutine(ZoomRotateAndCenterCamera());
        }
    }
    public void Setup(DiagramControll diagram)
    {
        this.diagram = diagram;
    }
    private IEnumerator ZoomRotateAndCenterCamera()
    {
        Vector3 targetPosition = new Vector3(diagram.transform.position.x, diagram.transform.position.y, mainCamera.transform.position.z);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotationAngle);

        while (Mathf.Abs(mainCamera.orthographicSize - targetSize) > 0.01f ||
               (mainCamera.transform.position - targetPosition).magnitude > 0.01f ||
               Quaternion.Angle(mainCamera.transform.rotation, targetRotation) > 0.01f)
        {
            mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, targetSize, Time.deltaTime * changeSpeed);
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, targetPosition, Time.deltaTime * changeSpeed * 10f);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, targetRotation, Time.deltaTime * changeSpeed * 100f);
            yield return null;
        }

        mainCamera.orthographicSize = targetSize;
        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }
    private IEnumerator Stop_co()
    {
        while (true)
        {
            if (GameManager.Instance.Speed.Equals(0))
                yield return null;
            else
            {
                targetValue += 0.1f;
                yield return new WaitForSeconds(0.1f / GameManager.Instance.Speed);
            }
            if (targetValue > maxValue)
                yield break;
        }
    }
    public void UpdateSlider(float value)
    {
        value = Mathf.Clamp01(value);

        if (cicleImage != null)
        {
            cicleImage.fillAmount = value;
        }
    }

    public void SetValue(float value)
    {
        targetValue = Mathf.Clamp(value, 0, maxValue);
    }
}
