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
    /// <summary>
    /// Tests if the game starts properly and invokes the events correctly.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator IsGameStarting()
    {
        SceneManager.LoadScene(0);
        yield return null;

        List<Button> listButtons = Button.FindObjectsByType<Button>(FindObjectsSortMode.None).ToList<Button>();
        Button startButton = listButtons.Find(x => x.name == "StartButton");

        startButton.onClick.Invoke();
        yield return null;

        Assert.IsTrue(GameManager.s_this.IsPlaying);
    }

    /// <summary>
    /// Tests if the character jumps correctly once the game has started.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator IsCharacterJumping()
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

    /// <summary>
    /// Tests that the leaderboard displays correctly once the game has ended.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator IsLeaderboardDisplaying()
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

    /// <summary>
    /// Tests if the ability to read a file works correctly in script.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator IsReadingFile()
    {
        SceneManager.LoadScene(0);
        yield return null;

        LeaderboardManager leaderboardManager = GameObject.FindFirstObjectByType<LeaderboardManager>();
        Score[] scoresFromFile = leaderboardManager.ReadLeaderboard();

        yield return null;
        Assert.IsTrue(scoresFromFile.Length > 0);
    }
}
