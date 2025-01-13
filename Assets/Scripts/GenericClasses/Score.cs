using System;
using UnityEngine;

public class Score : IComparable<Score>
{
    public int score;
    public DateTime date;

    public Score(int score)
    {
        this.score = score;
        this.date = DateTime.Now;
    }

    public static Score FromSOToScore(ScoreSO so)
    {
        Score newScore = new Score(so.score);
        newScore.date = new DateTime(so.year, so.month, so.day);

        return newScore;
    }

    public static bool operator <(Score score1,  Score score2)
    {
        return score1.score < score2.score || (score1.score == score2.score && score1.date < score2.date);
    }

    public static bool operator >(Score score1, Score score2)
    {
        return score1.score > score2.score || (score1.score == score2.score && score1.date > score2.date);
    }

    public override string ToString()
    {
        return "Score : " + score + "\t\t" + date.Day + "/" + date.Month + "/" + date.Year;
    }

    public int CompareTo(Score other)
    {
        if (this < other) return -1;
        else if (this > other) return 1;
        else return 0;
    }
}
