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

    [UnityTest]
    public IEnumerator isCharacterJumping()
    {
        SceneManager.LoadScene(0);
        yield return null;

        List<Button> listButtons = Button.FindObjectsByType<Button>(FindObjectsSortMode.None).ToList<Button>();
        Button startButton = listButtons.Find(x => x.name == "StartButton");

        startButton.onClick.Invoke();
        yield return null;

        Button flyButton = listButtons.Find(x => x.name == "FlyUpButton");
        flyButton.onClick.Invoke();
        yield return null;

        PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();
        Assert.IsTrue(player.GetComponent<Rigidbody>().linearVelocity.magnitude > 1);
    }

    [UnityTest]
    public IEnumerator isLeaderboardDisplaying()
    {
        PlayerPrefs.SetString("username", "test");
        SceneManager.LoadScene(0);
        yield return null;

        List<Button> listButtons = Button.FindObjectsByType<Button>(FindObjectsSortMode.None).ToList<Button>();
        Button startButton = listButtons.Find(x => x.name == "StartButton");

        startButton.onClick.Invoke();
        yield return null;

        yield return new WaitUntil(() => !GameManager.s_this.IsPlaying);
        yield return new WaitForSeconds(1.2f);

        GameObject leaderboardPanel = GameObject.Find("EndScorePanel");
        Assert.IsTrue(leaderboardPanel.activeSelf);
    }

    [UnityTest]
    public IEnumerator isReadingFile()
    {
        SceneManager.LoadScene(0);
        yield return null;

        LeaderboardManager leaderboardManager = GameObject.FindFirstObjectByType<LeaderboardManager>();
        Score[] scoresFromFile = leaderboardManager.ReadLeaderboard();

        yield return null;
        Assert.IsTrue(scoresFromFile.Length > 0);
    }
}
