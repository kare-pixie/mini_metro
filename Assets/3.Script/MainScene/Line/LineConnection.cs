using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnection : MonoBehaviour
{
    Camera cam;
    private LineRenderer line;
    private Vector3 previousPosition;
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField] private float width = 0.1f;
    private GameObject clickedObject; // 클릭한 오브젝트를 저장할 변수
    private Vector3 savedPosition; // 저장된 좌표를 저장할 변수
    private bool isSaved = false; // 좌표가 저장되었는지 여부를 나타내는 변수
    private TrainController trainController;

    private LineManager lineManager;
    private void Start()
    {
        cam = Camera.main;
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = line.endWidth = width;
        previousPosition = transform.position;

        lineManager = FindObjectOfType<LineManager>();
        trainController = FindObjectOfType<TrainController>();
    }

    private GameObject GetObjectUnderMouse()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        // 마우스 위치에서 가장 가까운 Collider 찾기
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePosition);
        if (colliders.Length > 0)
        {
            // 가장 가까운 Collider를 가진 오브젝트 반환
            return colliders[0].gameObject;
        }
        // Collider가 없는 경우 null 반환
        return null;
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭 시작 시, 클릭한 오브젝트 저장
            clickedObject = GetObjectUnderMouse();
            if (clickedObject != null && clickedObject.CompareTag("Station"))
            {
                if (lineManager.lines.Count > 0)
                {
                    LineData lastLine = lineManager.GetLastLine();
                    if (clickedObject.transform.position.Equals(lastLine.endPosition) || clickedObject.transform.position.Equals(lastLine.startPosition))
                    {
                        isSaved = true;
                        savedPosition = clickedObject.transform.position;
                        line.positionCount = 1;
                        line.SetPosition(0, savedPosition);
                        previousPosition = savedPosition;
                    }
                }
                else
                {
                    isSaved = true;
                    savedPosition = clickedObject.transform.position;
                    line.positionCount = 1;
                    line.SetPosition(0, savedPosition);
                    previousPosition = savedPosition;
                }
            }
        }

        if (Input.GetMouseButton(0) && isSaved)
        {
            Vector3 currentPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0f;

            if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
            {
                // 중간 점 계산
                Vector3 intermediatePosition = new Vector3((savedPosition.x + currentPosition.x) / 2, savedPosition.y, 0f);
                line.positionCount = 3;
                line.SetPosition(0, savedPosition);
                line.SetPosition(1, intermediatePosition);
                line.SetPosition(2, currentPosition);
                previousPosition = currentPosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(isSaved)
            {
                clickedObject = GetObjectUnderMouse();
                if (clickedObject == null || !clickedObject.CompareTag("Station"))
                {
                    // 스테이션에서 끝나지 않으면 선 초기화
                    isSaved = false;
                    line.positionCount = 0;
                    previousPosition = transform.position;

                    LineData lastLine = lineManager.GetLastLine();
                    if (lastLine != null && lastLine.startPosition.Equals(savedPosition) && 
                        (!lineManager.IsTrainOnLine(lastLine) || (lineManager.GetLineCount() == 3) && trainController.TrainPassengerData_List.Count.Equals(0)))
                        lineManager.RemoveLine();
                }
                else
                {
                    Vector3 finalPosition = clickedObject.transform.position;
                    Vector3 intermediatePosition = new Vector3((savedPosition.x + finalPosition.x) / 2, savedPosition.y, 0f);
                    if (savedPosition.Equals(finalPosition))
                    {
                        return;
                    }
                    isSaved = false;
                    // 선 마무리
                    line.positionCount = 3;
                    line.SetPosition(0, savedPosition);
                    line.SetPosition(1, intermediatePosition);
                    line.SetPosition(2, finalPosition);

                    if (lineManager.lines.Count != 0)
                    {
                        bool isDuplicate = false;
                        LineData lastLine = lineManager.GetLastLine();

                        for (int i = 0; i < lineManager.lines.Count; i++)
                        {
                            LineData currentLine = lineManager.lines[i];
                            if ((currentLine.startPosition.Equals(savedPosition) && currentLine.endPosition.Equals(finalPosition)) ||
                                (currentLine.startPosition.Equals(finalPosition) && currentLine.endPosition.Equals(savedPosition)))
                            {
                                isDuplicate = true;
                                break;
                            }
                        }

                        if (!isDuplicate && lastLine != null)
                        {
                            isDuplicate = lastLine.startPosition.Equals(savedPosition) || (lastLine.startPosition.Equals(savedPosition) && lastLine.endPosition.Equals(finalPosition)) ||
                                          (lastLine.startPosition.Equals(finalPosition) && lastLine.endPosition.Equals(savedPosition));
                        }

                        if (isDuplicate)
                        {
                            line.positionCount = 0;
                        }
                        else
                        {
                            lineManager.SaveLine(savedPosition, intermediatePosition, finalPosition);
                        }
                    }
                    else
                    {
                        lineManager.SaveLine(savedPosition, intermediatePosition, finalPosition);
                    }
                }
            }
        }
    }
}
