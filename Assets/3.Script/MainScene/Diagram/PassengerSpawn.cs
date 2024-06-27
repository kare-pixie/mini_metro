using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerSpawn : MonoBehaviour
{
    [SerializeField] private DiagramSpawner diagramSpawner;
    [SerializeField] private GameObject PassengerPrefabs;
    [SerializeField] private AudioClip spawnSound;
    private AudioSource audioSource;
    private List<GameObject> Station_List;
    private int totalTime;
    private void Start()
    {
        totalTime = 0;
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        Station_List = diagramSpawner.DiagramPrefabsList;
        if (Station_List.Count < 0)
            return;

        if (GameManager.Instance == null || GameManager.Instance.Speed < 1)
            return;

        if (GameManager.Instance.isGameOver)
            return;

        totalTime++;
        if (totalTime >= 500 / GameManager.Instance.Speed)
        {
            totalTime = 0;
            SpawnPassenger();
        }
    }
    public void addStationData(int idx, Diagram.Element passengerData)
    {
        GameObject passenger = Instantiate(PassengerPrefabs, Station_List[idx].transform);
        passenger.GetComponent<PassengerDiagramControll>().DiagramChange(passengerData);
        DiagramControll diagramControll = Station_List[idx].GetComponent<DiagramControll>();
        diagramControll.PassengerData_List.Add(passenger.GetComponent<PassengerDiagramControll>());

        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
    private void SpawnPassenger()
    {
        int stationNum = Random.Range(0, Station_List.Count);
        int diagramNum = Random.Range(0, 8);
        DiagramControll diagramControll = Station_List[stationNum].GetComponent<DiagramControll>();
        if (diagramControll.PassengerData_List.Count > diagramControll.Passenger_Max * 2)
            return;
        if (diagramControll.DiagramElement.Equals((Diagram.Element)diagramNum))
            return;
        switch (diagramNum)
        {
            case 0:
                if (diagramSpawner.CircleCnt > 0)
                    addStationData(stationNum, Diagram.Element.circle);
                break;
            case 1:
                if (diagramSpawner.TriangleCnt > 0)
                    addStationData(stationNum, Diagram.Element.triangle);
                break;
            case 2:
                if (diagramSpawner.SqureCnt > 0)
                    addStationData(stationNum, Diagram.Element.squre);
                break;
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                if (diagramSpawner.IsDiagramExits[diagramNum - 3])
                {
                    addStationData(stationNum, (Diagram.Element)diagramNum);
                }
                break;
        }
    }
}