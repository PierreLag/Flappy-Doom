using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager s_this;

    private int points;
    private bool isPlaying;
    public bool IsPlaying { get { return isPlaying; } }

    private UnityEvent onStart = new();
    public UnityEvent OnStart { get { return onStart; } set { onStart = value; } }
    private UnityEvent onEnd = new();
    public UnityEvent OnEnd { get { return onEnd; } set { onEnd = value; } }

    [SerializeField]
    private InterfaceRenderer interfaceRenderer;

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
        isPlaying = false;
    }

    public static void AddPoint()
    {
        s_this.points++;
        s_this.interfaceRenderer.UpdateDisplayedScore();
    }

    public static int GetPoints()
    {
        return s_this.points;
    }

    public static void EndGame()
    {
        s_this.isPlaying = false;
        s_this.OnEnd?.Invoke();
    }

    public static void StartGame()
    {
        s_this.points = 0;
        s_this.isPlaying = true;
        s_this.onStart?.Invoke();
    }

    public static void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }
}
