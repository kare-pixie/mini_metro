using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public bool isGameOver = false;
    public int Speed = 1;
    private int Score = 0;
    private int Day = 0;
    [SerializeField] private Text Score_Text = null;
    [SerializeField] private Text GameOverTitle_Text = null;
    [SerializeField] private Text GameOverContents_Text = null;
    [SerializeField] private GameObject overlayPanel = null;
    [SerializeField] private GameObject replayButton = null;
    [SerializeField] private GameObject menuButton = null;
    [SerializeField] private GameObject returnButton = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("게임 매니저가 이미 존재합니다.");
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(isGameOver)
        {
            if(GameOverTitle_Text != null && !GameOverTitle_Text.gameObject.activeSelf)
                GameOverTitle_Text.gameObject.SetActive(true);
            if (GameOverContents_Text != null && !GameOverContents_Text.gameObject.activeSelf)
            {
                GameOverContents_Text.gameObject.SetActive(true);
                GameOverContents_Text.text = 
                    $"본 역은 너무 혼잡해 지하철이 폐쇄조치 되었습니다.\n"+
                    $"당신의 철도에서 {Day}일 동안 {Score}명의 승객이 여행했습니다.";
            }
            if (overlayPanel != null && !overlayPanel.activeSelf)
            {
                overlayPanel.SetActive(true);
            }
            if (replayButton != null && !replayButton.activeSelf)
            {
                replayButton.SetActive(true);
            }
            if (menuButton != null && !menuButton.activeSelf)
            {
                menuButton.SetActive(true);
            }
            if (returnButton != null && returnButton.activeSelf)
            {
                returnButton.SetActive(false);
            }
        }
    }
    public void AddScore(int score)
    {
        if (!isGameOver)
        {
            Score += score;
            Score_Text.text = Score.ToString();
        }
    }
    public void AddDay(int day)
    {
        if (!isGameOver)
        {
            Day += day;
        }
    }
}
