using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System;

public class InterfaceRenderer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreDisplay;
    [SerializeField]
    private Image leaderboardPanel;
    [SerializeField]
    private ScrollRect leaderboardView;
    [SerializeField]
    private GameObject scoreTemplate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.s_this.OnEnd.AddListener(delegate { DisplayLeaderboard(); });
    }

    public void UpdateDisplayedScore()
    {
        scoreDisplay.text = "Score : " + GameManager.GetPoints();
    }

    public void DisplayLeaderboard()
    {
        leaderboardPanel.gameObject.SetActive(true);
    }

    public GameObject InstantiateDisplayScore(string name, DateTime date, int score)
    {
        GameObject displayScore = Instantiate(scoreTemplate);

        displayScore.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        displayScore.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = date.Day + "/" + date.Month + "/" + date.Year;
        displayScore.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = score.ToString();

        return displayScore;
    }
}
