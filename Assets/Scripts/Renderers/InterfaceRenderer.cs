using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FlappyDoom
{
    public class InterfaceRenderer : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreDisplay;
        [SerializeField]
        private Image leaderboardPanel;
        [SerializeField]
        private ScrollRect leaderboardView;
        [SerializeField]
        private GameObject scoreTemplate;
        [SerializeField]
        private LeaderboardManager leaderboardManager;

        [SerializeField]
        private Image usernameInputPanel;
        [SerializeField]
        private TMP_InputField usernameInput;
        [SerializeField]
        private TextMeshProUGUI errorText;
        [SerializeField]
        private OnlineController onlineController;

        private bool isLocal = true;
        private bool isMonthOnly = false;
        private bool isAscending = false;

        // Assigns the display of the leaderboard on game end, and its reinitialisation once a new game starts.
        // If it's the first time booting the game, asks for the player's username to store it on both the database and locally.
        void Start()
        {
            GameManager.s_this.OnEnd.AddListener(delegate { DisplayLeaderboard(); });
            GameManager.s_this.OnStart.AddListener(delegate { ClearLeaderboardView(); UpdateDisplayedScore(); });

            if (PlayerPrefs.GetString("username", "") == "")
            {
                usernameInputPanel.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Updates the score displayed on the top right of the screen to show the player's score in the current game.
        /// </summary>
        public void UpdateDisplayedScore()
        {
            scoreDisplay.text = "Score : " + GameManager.GetPoints();
        }

        /// <summary>
        /// Displays the leaderboard. If it wasn't shown before this call, waits for a second for the animation and SFX to play.
        /// <para></para>
        /// Its displayed values depend on the values of isLocal, isAscending and isMonthOnly.
        /// <para></para>
        /// isLocal : whether it shows the scores stored on the device's local NoSQL database or the ones stored on the distant SQL database.
        /// <br/>
        /// isAscending : shows the scores in either ascending or descending order.
        /// <br/>
        /// isMonthOnly : if on, it will only show the scores for this month. Otherwise, show all scores.
        /// </summary>
        private async void DisplayLeaderboard()
        {
            if (!leaderboardPanel.IsActive())
            {
                await Task.Delay(1000);
                isLocal = true;
                leaderboardPanel.gameObject.SetActive(true);
            }

            List<Score> scores;
            if (isLocal)
            {
                scores = leaderboardManager.ReadLeaderboard().ToList<Score>();
                await Task.Delay(100);
            }
            else
            {
                StartCoroutine(onlineController.GetScores());

                object response = null;
                int timesWaiting = 0;
                while (response == null && timesWaiting <= 5)
                {
                    await Task.Delay(100);
                    response = onlineController.GetResponse();
                    timesWaiting++;
                }

                scores = (List<Score>)(onlineController.GetResponse());
            }

            if (isAscending)
            {
                scores.Sort((a, b) => a.CompareTo(b));
            }
            else
            {
                scores.Sort((a, b) => b.CompareTo(a));
            }

            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i].date.Year > 1900 && ((isMonthOnly && scores[i].date.Month == DateTime.Now.Month) || !isMonthOnly))  // The default year the scores are initialised in, display if they're not that
                {
                    GameObject newDisplayScore = InstantiateDisplayScore(scores[i].name, scores[i].date, scores[i].score);
                    newDisplayScore.transform.localScale = new Vector3(1f, 1f, 1f);
                    newDisplayScore.transform.DOLocalMoveY(-((RectTransform)newDisplayScore.transform).sizeDelta.y * (i * 2 + 1) / 2, 0f);
                }
            }
            // We adapt the content size of the Leaderboard ScrollRect in order to allow the smooth and responsive scrolling through the data.
            ((RectTransform)leaderboardView.content).sizeDelta = new Vector2(0, ((RectTransform)scoreTemplate.transform).sizeDelta.y * leaderboardView.content.transform.childCount);
        }

        /// <summary>
        /// Takes the template stored in scoreTemplate and applies the attributes score's value, who scored it, and when.
        /// </summary>
        /// <param name="name">The name of the player who made the score (the default for locally stored scores is "You").</param>
        /// <param name="date">The date at which the score was made.</param>
        /// <param name="score">The value of the score.</param>
        /// <returns>The instantiated GameObject of the template with its value attributed.</returns>
        public GameObject InstantiateDisplayScore(string name, DateTime date, int score)
        {
            GameObject displayScore = Instantiate(scoreTemplate, leaderboardView.content);

            displayScore.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
            displayScore.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = date.Day + "/" + date.Month + "/" + date.Year;
            displayScore.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = score.ToString();

            return displayScore;
        }

        /// <summary>
        /// Destroys the leaderboard ScrollRect's content's children.
        /// </summary>
        private void ClearLeaderboardView()
        {
            Transform[] previousChildren = leaderboardView.content.GetComponentsInChildren<Transform>();
            foreach (Transform child in previousChildren)
            {
                if (child != leaderboardView.content)
                    Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Takes the name inserted into the input field usernameInput, and passes it onto the OnlineController to be sent to the database.
        /// Fails if a user with the same name already exists.
        /// </summary>
        public async void SubmitUsername()
        {
            onlineController.SaveUsername(usernameInput.text);

            object response = null;
            int timesWaiting = 0;
            while (response == null && timesWaiting <= 15)
            {
                await Task.Delay(100);
                response = onlineController.GetResponse();
                timesWaiting++;
            }

            switch (response)
            {
                case "Operation successful.":
                    usernameInputPanel.gameObject.SetActive(false);
                    break;
                case "Error : User already exists.":
                    errorText.text = "User already exists";
                    break;
                case "Error : Couldn't connect":
                    errorText.text = "Could not connect to server.\nCheck your connection.";
                    break;
                default:
                    errorText.text = "Connection timed out.\nCheck your connection.";
                    break;
            }
        }

        /// <summary>
        /// Swaps between the local and the distant leaderboard database.
        /// </summary>
        public void SwapLeaderboardArea()
        {
            if (isLocal)
            {
                ClearLeaderboardView();
                isLocal = false;
                DisplayLeaderboard();
            }
            else
            {
                ClearLeaderboardView();
                isLocal = true;
                DisplayLeaderboard();
            }
        }

        /// <summary>
        /// Changes the value of the isMonthOnly bool, affecting which values to display on the leaderboard.
        /// </summary>
        /// <param name="isTrue">True if only scores of this month should be shown, false otherwise.</param>
        public void ShowOnlyCurrentMonth(bool isTrue)
        {
            ClearLeaderboardView();
            isMonthOnly = isTrue;
            DisplayLeaderboard();
        }

        /// <summary>
        /// Toggles the display order of the scores on the leaderboard.
        /// </summary>
        public void SwapLeaderboardOrder()
        {
            ClearLeaderboardView();
            isAscending = !isAscending;
            DisplayLeaderboard();
        }
    }
}