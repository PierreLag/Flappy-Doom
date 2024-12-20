using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager s_this;

    private int points;

    private void Awake()
    {
        if (s_this != null)
        {
            Destroy(gameObject);
        }
        else
        {
            s_this = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        points = 0;
    }

    public static void AddPoint()
    {
        s_this.points++;
        Debug.Log("New score : " + s_this.points);
    }

    public static int GetPoints()
    {
        return s_this.points;
    }

    public static void EndGame()
    {
        Time.timeScale = 0;
    }
}
