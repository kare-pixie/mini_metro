using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public List<LineData> lines = new List<LineData>();
    private LineRenderer line;
    public List<TrainController> trains = new List<TrainController>(); // 열차 리스트
    [SerializeField] private float width = 0.1f;
    private List<Diagram.Element> elements;
    public List<Diagram.Element> Elements => elements;

    public void SaveLine(Vector3 start, Vector3 intermediatePosition, Vector3 end)
    {
        LineData newLine = new LineData { startPosition = start, interPosition = intermediatePosition, endPosition = end };
        lines.Add(newLine);
        UpdateElements();
    }
    public void RemoveLine()
    {
        if (lines.Count > 0)
        {
            lines.RemoveAt(lines.Count - 1);
            elements.RemoveAt(elements.Count - 1);
        }
    }

    public LineData GetLastLine()
    {
        if (lines.Count > 0)
        {
            return lines[lines.Count - 1];
        }
        return null;
    }
    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = line.endWidth = width;
        line.numCornerVertices = 10;
        line.numCapVertices = 10;

        // Width curve 설정
        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(0.0f, width);
        widthCurve.AddKey(0.5f, width);
        widthCurve.AddKey(1.0f, width);
        line.widthCurve = widthCurve;

        elements = new List<Diagram.Element>();
    }

    private void Update()
    {
        UpdateLineRenderer();
    }
    private void UpdateLineRenderer()
    {
        if (lines.Count == 0)
        {
            line.positionCount = 0;
            return;
        }

        line.positionCount = lines.Count * 2 + 1;
        int positionIndex = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            LineData currentLine = lines[i];

            if (i == 0)
            {
                line.SetPosition(positionIndex++, currentLine.startPosition);
            }

            line.SetPosition(positionIndex++, currentLine.interPosition);
            line.SetPosition(positionIndex++, currentLine.endPosition);
        }
    }

    private void UpdateElements()
    {
        List<Diagram.Element> seenElements = new List<Diagram.Element>();

        int i = 0;
        foreach (var lineData in lines)
        {
            if(i==0)
                AddElementFromPosition(lineData.startPosition, seenElements);
            //AddElementFromPosition(lineData.interPosition, seenElements);
            AddElementFromPosition(lineData.endPosition, seenElements);
            i++;
        }
        elements.Clear();
        elements = seenElements;
    }

    private void AddElementFromPosition(Vector3 position, List<Diagram.Element> seenElements)
    {
        GameObject stationObject = GetObjectAtPosition(position);
        if (stationObject != null && stationObject.CompareTag("Station"))
        {
            Diagram.Element element = stationObject.GetComponent<DiagramControll>().DiagramElement;
            seenElements.Add(element);
        }
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
    public int GetLineCount()
    {
        return line.positionCount;
    }
    public bool IsTrainOnLine(LineData line)
    {
        foreach (TrainController train in trains)
        {
            if (train.lineRenderer == null)
                continue;

            Vector3 trainPosition = train.GetCurrentTrainPosition();
            if (IsPointOnLineSegment(trainPosition, line.startPosition, line.endPosition) ||
                IsPointOnLineSegment(trainPosition, line.startPosition, line.interPosition) ||
                IsPointOnLineSegment(trainPosition, line.interPosition, line.endPosition))
            {
                return true;
            }
        }
        return false;
    }
    private bool IsPointOnLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        float lineLength = Vector3.Distance(lineStart, lineEnd);
        float d1 = Vector3.Distance(point, lineStart);
        float d2 = Vector3.Distance(point, lineEnd);
        float buffer = 0.01f; // 부동소수점 오차를 고려한 버퍼

        // 두 거리의 합이 선의 길이와 같으면 점이 선 위에 있음
        return Mathf.Abs((d1 + d2) - lineLength) < buffer;
    }
}
