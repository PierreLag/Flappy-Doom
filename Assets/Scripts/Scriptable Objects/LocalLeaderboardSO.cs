using UnityEngine;

[CreateAssetMenu(fileName = "LocalLeaderboardSO", menuName = "Scriptable Objects/LocalLeaderboardSO")]
public class LocalLeaderboardSO : ScriptableObject
{
    public ScoreSO[] scores;

    public string ToJSON()
    {
        return JsonUtility.ToJson(scores);
    }

    public ScoreSO[] FromJSON(string json)
    {
        return JsonUtility.FromJson<ScoreSO[]>(json);
    }
}
