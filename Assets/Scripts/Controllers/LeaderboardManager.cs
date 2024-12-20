using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    private static LeaderboardManager s_this;

    [SerializeField]
    private LocalLeaderboardSO localLeaderboard;

    // Start is called before the first frame update
    void Awake()
    {
        if (s_this == null)
        {
            s_this = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddNewScore(int score)
    {
        List<Score> scores = new List<Score>();

        foreach(ScoreSO scoreSo in localLeaderboard.scores)
        {
            scores.Add(Score.FromSOToScore(scoreSo));
        }

        scores.Add(new Score(score));

        scores.Sort();
        scores.RemoveAt(scores.Count - 1);

        for (int i = 0; i < scores.Count; i++)
        {
            localLeaderboard.scores[i].UpdateFromScore(scores[i]);
        }
    }
}
