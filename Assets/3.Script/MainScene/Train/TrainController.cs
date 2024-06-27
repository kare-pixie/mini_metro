using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [SerializeField] private DiagramSpawner diagramSpawner;
    [SerializeField] private ClockHandController clockHandController;
    public LineRenderer lineRenderer; // 라인 렌더러 참조

    private int currentPointIndex = 0; // 현재 이동 중인 라인 렌더러의 점 인덱스
    private float progress = 0f; // 두 점 사이의 진행 상태
    private bool movingForward = true; // 이동 방향
    private Vector3 savePoint;
    private bool isStopped = false; // 열차가 멈췄는지 여부

    [SerializeField] private int passenger_max = 6;
    public int Passenger_Max => passenger_max;

    public List<PassengerDiagramControll> TrainPassengerData_List;
    private List<GameObject> Station_List;

    private GameObject stationObject; // 스테이션 오브젝트를 저장할 변수
    private LineManager lineManager;

    private void Awake()
    {
        TrainPassengerData_List = new List<PassengerDiagramControll>();
        savePoint = new Vector3(11, 0);
    }
    private void Start()
    {
        lineManager = FindObjectOfType<LineManager>();
    }

    private void Update()
    {
        if (lineRenderer == null || lineRenderer.positionCount < 2)
        {
            transform.position = savePoint;
            return; // 라인 렌더러가 없거나 점이 2개 이상 있어야 함
        }

        if (GameManager.Instance != null && (GameManager.Instance.Speed.Equals(0) || GameManager.Instance.isGameOver))
            return;

        if (isStopped)
            return;

        Station_List = diagramSpawner.DiagramPrefabsList;

        // 승객 위치 정렬
        ArrangePassengerTrainPositions();

        // 현재 점 인덱스가 유효한지 확인
        if (currentPointIndex < 0 || currentPointIndex > lineRenderer.positionCount - 1)
            return;

        Vector3 startPoint = lineRenderer.GetPosition(currentPointIndex);
        Vector3 endPoint = movingForward ? lineRenderer.GetPosition(currentPointIndex + 1) : lineRenderer.GetPosition(currentPointIndex - 1);

        progress += GameManager.Instance.Speed * Time.deltaTime / Vector3.Distance(startPoint, endPoint);

        if (progress >= 1f)
        {
            progress = 0f;
            if (movingForward)
            {
                currentPointIndex++;
                if (currentPointIndex >= lineRenderer.positionCount - 1)
                {
                    currentPointIndex = lineRenderer.positionCount - 1;
                    movingForward = false;
                }
            }
            else
            {
                currentPointIndex--;
                if (currentPointIndex <= 0)
                {
                    currentPointIndex = 0;
                    movingForward = true;
                }
            }

            // 스테이션에 도달했는지 확인
            if (stationObject != null && stationObject.CompareTag("Station"))
            {
                StartCoroutine(StopAtStation_co()); // 스테이션에 도달하면 멈춤
            }

            // 업데이트된 currentPointIndex에 따라 startPoint와 endPoint 재설정
            if (currentPointIndex >= 0 && currentPointIndex <= lineRenderer.positionCount - 1)
            {
                startPoint = lineRenderer.GetPosition(currentPointIndex);
                endPoint = movingForward ? lineRenderer.GetPosition(currentPointIndex + 1) : lineRenderer.GetPosition(currentPointIndex - 1);
            }
        }

        transform.position = Vector3.Lerp(startPoint, endPoint, progress);

        // 스테이션 오브젝트 확인 및 할당
        stationObject = GetObjectAtPosition(transform.position);

        // 열차가 이동 방향을 바라보도록 설정
        Vector3 direction = (endPoint - startPoint).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation *= Quaternion.Euler(0, -90, 0); // 기본 방향을 오른쪽에서 왼쪽으로 변경
        transform.rotation = rotation;
    }

    private GameObject GetObjectAtPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        if (colliders.Length > 0)
        {
            return colliders[0].gameObject; // 가장 가까운 Collider를 가진 오브젝트 반환
        }
        return null; // Collider가 없는 경우 null 반환
    }

    private IEnumerator StopAtStation_co() // 열차 정차
    {
        isStopped = true;
        DiagramControll diagramControll = null;
        for (int i = 0; i< Station_List.Count; i++)
        {
            if(Station_List[i].Equals(stationObject))
            {
                diagramControll = Station_List[i].GetComponent<DiagramControll>();
                break;
            }
        }

        if (diagramControll != null)
        {
            //승객 하차
            for (int i = 0; i < TrainPassengerData_List.Count; i++)
            {
                if (diagramControll.DiagramElement.Equals(TrainPassengerData_List[i].DiagramElement))
                {
                    DestroyStationData(TrainPassengerData_List[i]);
                    i--;
                    GameManager.Instance.AddScore(1);
                    ArrangePassengerTrainPositions();
                    yield return new WaitForSeconds(0.5f / GameManager.Instance.Speed);
                }
            }

            //승객 탑승
            List<Diagram.Element> lineElements = lineManager.Elements;
            List<PassengerDiagramControll> passengerDataList = diagramControll.PassengerData_List;

            if (currentPointIndex < 0 || currentPointIndex > lineRenderer.positionCount - 1)
            {
                isStopped = false;
                yield break;
            }

            Vector3 startPoint = lineRenderer.GetPosition(currentPointIndex);
            Vector3 endPoint = movingForward ? lineRenderer.GetPosition(currentPointIndex + 1) : lineRenderer.GetPosition(currentPointIndex - 1);
            Vector3 direction = (endPoint - startPoint).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation *= Quaternion.Euler(0, -90, 0);

            for (int i = 0; i < passengerDataList.Count; i++)
            {
                if (TrainPassengerData_List.Count >= passenger_max)
                    break;
                bool isElement = false;
                PassengerDiagramControll passengerData = passengerDataList[i];
                int stationPoint = (currentPointIndex + 1) / 2;
                if (movingForward)
                {
                    for (int j = stationPoint; j < lineElements.Count; j++)
                    {
                        if (lineElements[j].Equals(passengerData.DiagramElement))
                        {
                            isElement = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = stationPoint; j >= 0; j--)
                    {
                        if (lineElements[j].Equals(passengerData.DiagramElement))
                        {
                            isElement = true;
                            break;
                        }
                    }
                }
                if (isElement && !TrainPassengerData_List.Contains(passengerData))
                {
                    yield return new WaitForSeconds(0.5f / GameManager.Instance.Speed);
                    passengerData.transform.rotation = rotation;
                    passengerData.transform.localScale = new Vector3(0.25f, 0.25f, 1);
                    passengerData.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    TrainPassengerData_List.Add(passengerData);
                    diagramControll.subtractStationData(passengerData);
                    i--;

                    ArrangePassengerTrainPositions();
                }
            }

        }
        isStopped = false;
    }
    public Vector3 GetCurrentTrainPosition()
    {
        return transform.position;
    }

    public void DestroyStationData(PassengerDiagramControll passengerData)
    {
        TrainPassengerData_List.Remove(passengerData);
        Destroy(passengerData.gameObject);
    }
    private void ArrangePassengerTrainPositions()
    {
        for (int i = 0; i < TrainPassengerData_List.Count; i++)
        {
            Vector3 localPosition;
            localPosition = new Vector3(1.7f - (0.7f * i), -0.35f, 0f);
            TrainPassengerData_List[i].transform.SetParent(transform);
            TrainPassengerData_List[i].SetPosition(localPosition);
            TrainPassengerData_List[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
