using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diagram {
    public enum Element
    {
        circle = 0,
        triangle,
        squre,
        cross,
        star,
        pentagon,
        sector,
        rhombus
    }
}

public class DiagramControll : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteArray;
    public Sprite[] SpriteArray => spriteArray;
    private Diagram.Element diagramElement;
    public Diagram.Element DiagramElement => diagramElement;
    public List<PassengerDiagramControll> PassengerData_List;
    [SerializeField] private int passenger_max = 6;
    public int Passenger_Max => passenger_max;
    private int gameoverCount;
    public int GameoverCount => gameoverCount;

    private void Awake()
    {
        gameoverCount = 0;
        diagramElement = Diagram.Element.circle;
        PassengerData_List = new List<PassengerDiagramControll>();
    }
    private void Start()
    {
        StartCoroutine(PassengerCount_co());
    }
    private void Update()
    {
        ArrangePassengerPositions();
    }
    private IEnumerator PassengerCount_co()
    {
        while(true)
        {
            if (GameManager.Instance.isGameOver)
                yield break;
            if(GameManager.Instance.Speed.Equals(0))
                yield return new WaitForSeconds(0);
            else
            {
                if (PassengerData_List.Count > passenger_max)
                {
                    gameoverCount++;
                }
                else
                {
                    if (gameoverCount > 0)
                        gameoverCount--;
                }
                yield return new WaitForSeconds(0.5f / GameManager.Instance.Speed);
            }
        }
    }
    public void subtractStationData(PassengerDiagramControll passengerData)
    {
        PassengerData_List.Remove(passengerData);
    }
    private void ArrangePassengerPositions()
    {
        for (int i = 0; i < PassengerData_List.Count; i++)
        {
            Vector3 localPosition;
            Color nowColor = nowColor = new Color(0.25f, 0.19f, 0.25f, 1f);
            if (i < passenger_max)
            {
                localPosition = new Vector3(1f + (0.5f * i), 0.5f, 0f);
            }
            else if (i == passenger_max)
            {
                localPosition = new Vector3(1f+(0.5f* passenger_max), 0.25f, 0f);
                nowColor = new Color(0.25f, 0.19f, 0.25f, 0.75f);
            }
            else
            {
                if (i > passenger_max * 2)
                {
                    localPosition = new Vector3(1f, 0.5f, 0f);
                }
                else
                {
                    localPosition = new Vector3(0.5f + (0.5f * passenger_max) - (0.5f * (i - (passenger_max + 1))), 0, 0f);
                    nowColor = new Color(0.25f, 0.19f, 0.25f, 0.75f - (0.05f * i));
                }
            }
            PassengerData_List[i].SetPosition(localPosition);
            PassengerData_List[i].GetComponent<SpriteRenderer>().color = nowColor;
        }
    }
    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }
    public void DiagramChange(Diagram.Element diagramElement)
    {
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
        int idx = (int)diagramElement;
        this.diagramElement = diagramElement;
        if (idx > spriteArray.Length || spriteArray[idx] == null)
            return;
        Sprite sprite = spriteArray[idx];

        GetComponent<SpriteRenderer>().sprite = sprite;

        if (polygonCollider == null)
            return;

        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            path.Clear();
            sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path.ToArray());
        }
    }
}
