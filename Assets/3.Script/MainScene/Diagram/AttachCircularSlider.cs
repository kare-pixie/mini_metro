using UnityEngine;

public class AttachCircularSlider : MonoBehaviour
{
    private GameObject Target;
    private RectTransform UITransform;
    private Camera mainCamera;
    public float baseSize = 1.0f; // 기본 크기 조정 값

    public void Setup(GameObject target)
    {
        Target = target;
        UITransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (Target != null)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(Target.transform.position);
            UITransform.position = screenPosition;

            float cameraSizeFactor = baseSize / mainCamera.orthographicSize;
            UITransform.localScale = new Vector3(cameraSizeFactor, cameraSizeFactor, 1);
        }
    }
}
