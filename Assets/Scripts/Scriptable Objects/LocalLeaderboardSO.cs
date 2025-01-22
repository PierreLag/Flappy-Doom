using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FlappyDoom
{
    [CreateAssetMenu(fileName = "LocalLeaderboardSO", menuName = "Scriptable Objects/LocalLeaderboardSO")]
    public class LocalLeaderboardSO : ScriptableObject
    {
        public ScoreSO[] scores;

        public string ToJSON()
        {
            return JArray.FromObject(scores).ToString();
        }

        public ScoreSO[] FromJSON(string json)
        {
            JArray jscores = JArray.Parse(json);
            ScoreSO[] scores = jscores.ToObject<ScoreSO[]>();

            return scores;
        }
    }
}