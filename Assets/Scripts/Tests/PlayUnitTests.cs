using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Build.Content;
using FlappyDoom;

public class PlayUnitTests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator isGameStarting()
    {
        SceneManager.LoadScene(0);
        yield return null;

        List<Button> listButtons = Button.FindObjectsByType<Button>(FindObjectsSortMode.None).ToList<Button>();
        Button startButton = listButtons.Find(x => x.name == "StartButton");

        startButton.onClick.Invoke();
        yield return null;

        Assert.IsTrue(GameManager.s_this.IsPlaying);
    }
}
