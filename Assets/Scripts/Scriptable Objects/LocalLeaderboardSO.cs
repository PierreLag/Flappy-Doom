using UnityEngine;

[CreateAssetMenu(fileName = "LocalLeaderboardSO", menuName = "Scriptable Objects/LocalLeaderboardSO")]
public class LocalLeaderboardSO : ScriptableObject
{
    public ScoreSO[] scores;
}
