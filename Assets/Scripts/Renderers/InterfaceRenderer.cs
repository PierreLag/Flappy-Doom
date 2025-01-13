using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfaceRenderer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreDisplay;
    [SerializeField]
    private Image leaderboardPanel;
    [SerializeField]
    private ScrollRect leaderboardView;

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
}
