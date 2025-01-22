using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField]
    private LeaderboardManager leaderboardManager;

    [SerializeField]
    private Image usernameInputPanel;
    [SerializeField]
    private TMP_InputField usernameInput;
    [SerializeField]
    private TextMeshProUGUI errorText;
    [SerializeField]
    private OnlineController onlineController;

    private bool isLocal = true;
    private bool isMonthOnly = false;
    private bool isAscending = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.s_this.OnEnd.AddListener(delegate { DisplayLeaderboard(); });
        GameManager.s_this.OnStart.AddListener(delegate { ClearLeaderboardView(); UpdateDisplayedScore(); });

        if (PlayerPrefs.GetString("username", "") == "")
        {
            usernameInputPanel.gameObject.SetActive(true);
        }
    }

    public void UpdateDisplayedScore()
    {
        scoreDisplay.text = "Score : " + GameManager.GetPoints();
    }

    private async void DisplayLeaderboard()
    {
        if (!leaderboardPanel.IsActive())
        {
            await Task.Delay(1000);
            isLocal = true;
            leaderboardPanel.gameObject.SetActive(true);
        }

        List<Score> scores;
        if (isLocal)
        {
            scores = leaderboardManager.ReadLeaderboard().ToList<Score>();
            await Task.Delay(100);
        }
        else
        {
            StartCoroutine(onlineController.GetScores());

            object response = null;
            int timesWaiting = 0;
            while (response == null && timesWaiting <= 5)
            {
                await Task.Delay(100);
                response = onlineController.GetResponse();
                timesWaiting++;
            }

            scores = (List<Score>)(onlineController.GetResponse());
        }

        if (isAscending)
        {
            scores.Sort((a, b) => a.CompareTo(b));
        }
        else
        {
            scores.Sort((a, b) => b.CompareTo(a));
        }

        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].date.Year > 1900 && ((isMonthOnly && scores[i].date.Month == DateTime.Now.Month) || !isMonthOnly))  // The default year the scores are initialised in, display if they're not that
            {
                GameObject newDisplayScore = InstantiateDisplayScore(scores[i].name, scores[i].date, scores[i].score);
                newDisplayScore.transform.localScale = new Vector3(1f, 1f, 1f);
                newDisplayScore.transform.DOLocalMoveY(-((RectTransform)newDisplayScore.transform).sizeDelta.y * (i * 2 + 1) / 2, 0f);
            }
        }

        ((RectTransform)leaderboardView.content).sizeDelta = new Vector2(0, ((RectTransform)scoreTemplate.transform).sizeDelta.y * leaderboardView.content.transform.childCount);
    }

    public GameObject InstantiateDisplayScore(string name, DateTime date, int score)
    {
        GameObject displayScore = Instantiate(scoreTemplate, leaderboardView.content);

        displayScore.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        displayScore.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = date.Day + "/" + date.Month + "/" + date.Year;
        displayScore.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = score.ToString();

        return displayScore;
    }

    private void ClearLeaderboardView()
    {
        Transform[] previousChildren = leaderboardView.content.GetComponentsInChildren<Transform>();
        foreach (Transform child in previousChildren)
        {
            if (child != leaderboardView.content)
                Destroy(child.gameObject);
        }
    }

    public async void SubmitUsername()
    {
        onlineController.SaveUsername(usernameInput.text);

        object response = null;
        int timesWaiting = 0;
        while (response == null && timesWaiting <= 5)
        {
            await Task.Delay(100);
            response = onlineController.GetResponse();
            timesWaiting++;
        }

        switch (response)
        {
            case "Operation successful.":
                usernameInputPanel.gameObject.SetActive(false);
                break;
            case "Error : User already exists.":
                errorText.text = "User already exists";
                break;
            default:
                errorText.text = "Could not connect to server.\nCheck your connection.";
                break;
        }
    }

    public void SwapLeaderboardArea()
    {
        if (isLocal)
        {
            ClearLeaderboardView();
            isLocal = false;
            DisplayLeaderboard();
        }
        else
        {
            ClearLeaderboardView();
            isLocal = true;
            DisplayLeaderboard();
        }
    }

    public void ShowOnlyCurrentMonth(bool isTrue)
    {
        ClearLeaderboardView();
        isMonthOnly = isTrue;
        DisplayLeaderboard();
    }

    public void SwapLeaderboardOrder()
    {
        ClearLeaderboardView();
        isAscending = !isAscending;
        DisplayLeaderboard();
    }
}
