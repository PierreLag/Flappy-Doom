using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace FlappyDoom
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField]
        private LocalLeaderboardSO localLeaderboard;

        [SerializeField]
        private string filePathName = "Leaderboard.json";

        [STAThread]
        private void Awake()
        {
            try
            {
                StreamReader sr = new StreamReader(Application.persistentDataPath + Path.DirectorySeparatorChar + filePathName);
                sr.Close();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(FileNotFoundException))
                {
                    StreamWriter sw = new StreamWriter(Application.persistentDataPath + Path.DirectorySeparatorChar + filePathName);

                    sw.Write(localLeaderboard.ToJSON());

                    sw.Close();
                }
            }
        }

        private void Start()
        {
            GameManager.s_this.OnEnd.AddListener(delegate { AddNewScore(GameManager.GetPoints()); });
        }

        [STAThread]
        public void StoreLeaderboard()
        {
            try
            {
                StreamWriter sw = new StreamWriter(Application.persistentDataPath + Path.DirectorySeparatorChar + filePathName, false);
                sw.Write(localLeaderboard.ToJSON());

                sw.Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [STAThread]
        public Score[] ReadLeaderboard()
        {
            Score[] score = new Score[localLeaderboard.scores.Length];
            try
            {
                StreamReader sr = new StreamReader(Application.persistentDataPath + Path.DirectorySeparatorChar + filePathName);
                string jsonScore = sr.ReadToEnd();

                sr.Close();

                ScoreSO[] pulledScores = localLeaderboard.FromJSON(jsonScore);
                for (int i = 0; i < score.Length; i++)
                {
                    score[i] = Score.FromSOToScore(pulledScores[i]);
                    localLeaderboard.scores[i].UpdateFromScore(score[i]);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return score;
        }

        public void AddNewScore(int score)
        {
            List<Score> scores = new List<Score>();

            foreach (ScoreSO scoreSo in localLeaderboard.scores)
            {
                scores.Add(Score.FromSOToScore(scoreSo));
            }

            scores.Add(new Score(score, "You"));

            scores.Sort((a, b) => b.CompareTo(a));  //Sorts in descending order
            scores.RemoveAt(scores.Count - 1);

            for (int i = 0; i < scores.Count; i++)
            {
                localLeaderboard.scores[i].UpdateFromScore(scores[i]);
            }
            StoreLeaderboard();
        }
    }
}