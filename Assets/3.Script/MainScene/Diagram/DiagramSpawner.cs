using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagramSpawner : MonoBehaviour
{
    [SerializeField] private GameObject DiagramPrefabs;
    [SerializeField] private MapData mapData;
    [SerializeField] private MapData startMapData;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject circularSlider;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private AudioClip spawnSound;
    private AudioSource audioSource;

    private List<GameObject> _diagramPrefabsList = new List<GameObject>();
    public List<GameObject> DiagramPrefabsList => _diagramPrefabsList;
    private int totalTime, circleCnt, triangleCnt, squreCnt, totalCnt;
    private bool[] isDiagramExits;
    public bool[] IsDiagramExits => isDiagramExits;
    public int CircleCnt => circleCnt;
    public int TriangleCnt => triangleCnt;
    public int SqureCnt => squreCnt;
    private Coroutine adjustCameraCoroutine;

    private void Awake()
    {
        totalTime = 0;
        circleCnt = 0;
        triangleCnt = 0;
        squreCnt = 0;
        totalCnt = 0;
        isDiagramExits = new bool[5] { false, false, false, false, false };

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnInitialDiagram();
        }

        StartCoroutine(SpawnDiagram_co());
    }

    private IEnumerator SpawnDiagram_co()
    {
        while (true)
        {
            if (totalCnt > 20 || GameManager.Instance.isGameOver)
                yield break;
            if (GameManager.Instance != null && GameManager.Instance.Speed > 0)
            {
                totalTime++;

                if (totalTime >= 6000 / GameManager.Instance.Speed)
                {
                    totalTime = 0;
                    SpawnDiagram();
                }
            }

            yield return null;
        }
    }

    private void SpawnDiagram()
    {
        Vector3 position = GetRandomPositionNearExistingDiagram();

        if (position != Vector3.zero)
        {
            GameObject diagram = Instantiate(DiagramPrefabs, position, Quaternion.identity);
            AssignDiagramType(diagram);
            _diagramPrefabsList.Add(diagram);
            AdjustCamera();
            Spawn_CircleSlider(diagram.GetComponent<DiagramControll>());

            if (audioSource != null && spawnSound != null)
            {
                audioSource.PlayOneShot(spawnSound);
            }
        }
    }
    private void Spawn_CircleSlider(DiagramControll diagram)
    {
        GameObject SliderClone = Instantiate(circularSlider);

        SliderClone.transform.SetParent(Canvas.transform);
        SliderClone.transform.localScale = Vector3.one;

        SliderClone.GetComponent<AttachCircularSlider>().Setup(diagram.gameObject);
        SliderClone.GetComponent<CircularSlider>().Setup(diagram);
    }

    private void SpawnInitialDiagram()
    {
        Vector3 position = GetInitialPosition();

        if (position != Vector3.zero)
        {
            GameObject diagram = Instantiate(DiagramPrefabs, position, Quaternion.identity);
            AssignInitialDiagramType(diagram);
            _diagramPrefabsList.Add(diagram);
            AdjustCamera();
            Spawn_CircleSlider(diagram.GetComponent<DiagramControll>());

            if (audioSource != null && spawnSound != null)
            {
                audioSource.PlayOneShot(spawnSound);
            }
        }
    }

    private Vector3 GetRandomPositionNearExistingDiagram()
    {
        GameObject lastDiagram = _diagramPrefabsList[Random.Range(0, _diagramPrefabsList.Count)];
        Vector3 lastPosition = lastDiagram.transform.position;

        for (int i = 0; i < 10; i++)
        {
            float offsetX = Random.Range(-2f, 2f);
            float offsetY = Random.Range(-2f, 2f);
            Vector3 position = new Vector3(lastPosition.x + offsetX, lastPosition.y + offsetY, 0);

            if (IsValidPosition(position))
            {
                return position;
            }
        }
        return Vector3.zero;
    }

    private Vector3 GetInitialPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            float positionX = Random.Range(startMapData.LimitMin.x, startMapData.LimitMax.x);
            float positionY = Random.Range(startMapData.LimitMin.y, startMapData.LimitMax.y);
            Vector3 position = new Vector3(positionX, positionY, 0);
            
            if (!Physics2D.OverlapCircle(position, 0.4f, LayerMask.GetMask("Water")) && !Physics2D.OverlapCircle(position, 2f, LayerMask.GetMask("Station")))
            {
                return position;
            }
        }
        return Vector3.zero;
    }

    private bool IsValidPosition(Vector3 position)
    {
        bool isWithinBounds = position.x >= mapData.LimitMin.x && position.x <= mapData.LimitMax.x &&
                              position.y >= mapData.LimitMin.y && position.y <= mapData.LimitMax.y;

        bool isWater = Physics2D.OverlapCircle(position, 0.4f, LayerMask.GetMask("Water"));
        bool isStation = Physics2D.OverlapCircle(position, 0.6f, LayerMask.GetMask("Station"));
        return isWithinBounds && !isWater && !isStation;
    }

    private void AssignDiagramType(GameObject diagram)
    {
        if (circleCnt / 2 > triangleCnt || triangleCnt == 0)
        {
            triangleCnt++;
            totalCnt++;
            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.triangle);
        }
        else if (triangleCnt / 2 > squreCnt || squreCnt == 0)
        {
            squreCnt++;
            totalCnt++;
            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.squre);
        }
        else
        {
            if(circleCnt > 10)
            {
                int diagramIdx = Random.Range(0, isDiagramExits.Length);
                if (!isDiagramExits[diagramIdx])
                {
                    isDiagramExits[diagramIdx] = true;
                    switch (diagramIdx)
                    {
                        case 0:
                            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.cross);
                            break;
                        case 1:
                            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.star);
                            break;
                        case 2:
                            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.pentagon);
                            break;
                        case 3:
                            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.sector);
                            break;
                        case 4:
                            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.rhombus);
                            break;
                    }
                    totalCnt++;
                }
                else
                {
                    circleCnt++;
                    totalCnt++;
                }
            }
            else
            {
                circleCnt++;
                totalCnt++;
            }
        }
    }

    private void AssignInitialDiagramType(GameObject diagram)
    {
        if (circleCnt == 0)
        {
            circleCnt++;
            totalCnt++;
        }
        else if (triangleCnt == 0)
        {
            triangleCnt++;
            totalCnt++;
            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.triangle);
        }
        else
        {
            squreCnt++;
            totalCnt++;
            diagram.GetComponent<DiagramControll>().DiagramChange(Diagram.Element.squre);
        }
    }
    private void AdjustCamera()
    {
        if (adjustCameraCoroutine != null)
        {
            StopCoroutine(adjustCameraCoroutine);
        }

        adjustCameraCoroutine = StartCoroutine(AdjustCamera_co());
    }

    private IEnumerator AdjustCamera_co()
    {
        Vector3 minPosition = new Vector3(float.MaxValue, float.MaxValue, 0);
        Vector3 maxPosition = new Vector3(float.MinValue, float.MinValue, 0);

        foreach (GameObject diagram in _diagramPrefabsList)
        {
            Vector3 position = diagram.transform.position;
            if (position.x < minPosition.x) minPosition.x = position.x;
            if (position.y < minPosition.y) minPosition.y = position.y;
            if (position.x > maxPosition.x) maxPosition.x = position.x;
            if (position.y > maxPosition.y) maxPosition.y = position.y;
        }

        Vector3 centerPosition = (minPosition + maxPosition) / 2f;
        float cameraWidth = maxPosition.x - minPosition.x;
        float cameraHeight = maxPosition.y - minPosition.y;
        float targetSize = Mathf.Max(cameraWidth / mainCamera.aspect, cameraHeight) / 2f;

        while (Mathf.Abs(mainCamera.orthographicSize - targetSize) > 0.01f ||
               (mainCamera.transform.position - new Vector3(centerPosition.x, centerPosition.y, mainCamera.transform.position.z)).magnitude > 0.01f)
        {
            if (GameManager.Instance.isGameOver)
                yield break;
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize + 0.5f, Time.deltaTime);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(centerPosition.x, centerPosition.y, mainCamera.transform.position.z), Time.deltaTime);
            yield return null;
        }

        mainCamera.orthographicSize = targetSize + 0.5f;
        mainCamera.transform.position = new Vector3(centerPosition.x, centerPosition.y, mainCamera.transform.position.z);
    }
}