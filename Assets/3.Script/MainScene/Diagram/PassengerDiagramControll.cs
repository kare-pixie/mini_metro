using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerDiagramControll : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteArray;
    public Sprite[] SpriteArray => spriteArray;
    private Diagram.Element diagramElement = Diagram.Element.circle;
    public Diagram.Element DiagramElement => diagramElement;

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }
    public void DiagramChange(Diagram.Element diagramElement)
    {
        int idx = (int)diagramElement;
        this.diagramElement = diagramElement;
        if (idx > spriteArray.Length || spriteArray[idx] == null)
            return;
        Sprite sprite = spriteArray[idx];

        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}