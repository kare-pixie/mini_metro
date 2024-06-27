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
    private GameObject clickedObject; // Ŭ���� ������Ʈ�� ������ ����
    private Vector3 savedPosition; // ����� ��ǥ�� ������ ����
    private bool isSaved = false; // ��ǥ�� ����Ǿ����� ���θ� ��Ÿ���� ����
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
        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        // ���콺 ��ġ���� ���� ����� Collider ã��
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePosition);
        if (colliders.Length > 0)
        {
            // ���� ����� Collider�� ���� ������Ʈ ��ȯ
            return colliders[0].gameObject;
        }
        // Collider�� ���� ��� null ��ȯ
        return null;
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 Ŭ�� ���� ��, Ŭ���� ������Ʈ ����
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
                // �߰� �� ���
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
                    // �����̼ǿ��� ������ ������ �� �ʱ�ȭ
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
                    // �� ������
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
