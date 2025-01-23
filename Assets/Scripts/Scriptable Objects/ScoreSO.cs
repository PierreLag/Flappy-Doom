using System;
using UnityEngine;

namespace FlappyDoom
{
    [CreateAssetMenu(fileName = "ScoreSO", menuName = "Scriptable Objects/ScoreSO")]
    [Serializable]
    public class ScoreSO : ScriptableObject
    {
        public int score;
        [Range(1, 31)]
        public int day;
        [Range(1, 12)]
        public int month;
        [Range(1900, 9999)]
        public int year;

        public void UpdateFromScore(Score score)
        {
            this.score = score.score;
            year = score.date.Year;
            month = score.date.Month;
            day = score.date.Day;
        }
    }
}