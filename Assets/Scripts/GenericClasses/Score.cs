using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Score : IComparable<Score>
{
    public string name;
    public int score;
    public DateTime date;

    public Score(int score, string name)
    {
        this.score = score;
        this.date = DateTime.Now;
        this.name = name;
    }

    [JsonConstructor]
    public Score(int score, string name, DateTime date)
    {
        this.score = score;
        this.date = date;
        this.name = name;
    }

    public static Score FromSOToScore(ScoreSO so)
    {
        Score newScore = new Score(so.score, "You", new DateTime(so.year, so.month, so.day));

        return newScore;
    }

    public static List<Score> FromJSON(string json)
    {
        JArray jscores = JArray.Parse(json);
        List<Score> scores = jscores.ToObject<List<Score>>();

        return scores;
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
