using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;

namespace FlappyDoom
{
    public class OnlineController : MonoBehaviour
    {
        [SerializeField]
        private string apiUrlBase;
        [SerializeField]
        private string apiUploadUsername;
        [SerializeField]
        private string apiUploadScore;
        [SerializeField]
        private string apiDownloadScores;
        [SerializeField]
        private string apiTestConnection;

        private string username;

        private object latestResponse;

        // Loads the username from the stored PlayerPrefs (empty by default), then on game end, uploads the score online.
        void Start()
        {
            username = PlayerPrefs.GetString("username", "");
            GameManager.s_this.OnEnd.AddListener(delegate { StartCoroutine(UploadScore()); });
            StartCoroutine(TestConnectionServer());
        }

        /// <summary>
        /// Tries to store the username on the remote database. On failure, does not store it locally. Response stays active once obtained for a minimum of 150ms.
        /// </summary>
        /// <param name="username">The username to store both remotely and locally.</param>
        public async void SaveUsername(string username)
        {
            StartCoroutine(UploadUsername(username));

            object response = null;
            int timesWaiting = 0;
            while (response == null && timesWaiting <= 5)
            {
                await Task.Delay(100);
                response = latestResponse;
                timesWaiting++;
            }

            switch (response)
            {
                case "Operation successful.":
                    this.username = username;
                    PlayerPrefs.SetString("username", username);
                    break;
                default:
                    break;
            }

            await Task.Delay(150);
            ResetResponse();
        }

        /// <summary>
        /// Loads the stored username from the PlayerPrefs.
        /// </summary>
        public void LoadUsername()
        {
            username = PlayerPrefs.GetString("username");
        }

        /// <summary>
        /// Returns the username stored currently in the object.
        /// </summary>
        /// <returns>The string with the player's username.</returns>
        public string GetUsername()
        {
            return username;
        }

        /// <summary>
        /// Tries to upload the username onto the distant database. The variable latestResponse may have different contents depending on the upload status :
        /// <para></para>
        /// "Error : Couldn't connect" : the web request failed due to a connection error, most likely due to poor or inexistant internet.
        /// <br/>
        /// "Error : User already exists." : the web request was successful, but a user with the same username already exists in the database.
        /// <br/>
        /// "Operation successful." : the web request was successful, and the username was correctly added onto the database.
        /// </summary>
        /// <param name="username">The username to upload onto the database.</param>
        /// <returns></returns>
        private IEnumerator UploadUsername(string username)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

            formData.Add(new MultipartFormDataSection("username", username));

            UnityWebRequest apiRequest = UnityWebRequest.Post(apiUrlBase + apiUploadUsername, formData);
            apiRequest.SendWebRequest();

            while (!apiRequest.isDone)
            {
                yield return null;
            }

            string response = "";
            switch (apiRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    response = "Error : Couldn't connect";
                    break;
                case UnityWebRequest.Result.Success:
                    response = apiRequest.downloadHandler.text;
                    break;
                default:
                    break;
            }

            latestResponse = response;
        }

        /// <summary>
        /// Uploads the score on the remote database. Keeps only the highest score on the database per user.
        /// </summary>
        /// <returns></returns>
        public IEnumerator UploadScore()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

            formData.Add(new MultipartFormDataSection("username", username));
            formData.Add(new MultipartFormDataSection("score", GameManager.GetPoints().ToString()));
            formData.Add(new MultipartFormDataSection("date", DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day));

            UnityWebRequest apiRequest = UnityWebRequest.Post(apiUrlBase + apiUploadScore, formData);
            apiRequest.SendWebRequest();

            while (!apiRequest.isDone)
            {
                yield return null;
            }

            string response = "";
            switch (apiRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    response = "Error : Couldn't connect";
                    break;
                case UnityWebRequest.Result.Success:
                    response = apiRequest.downloadHandler.text;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Tries to obtain the list of scores from the remote database. Once done, puts the resulting List&lt;<see cref="Score"/>&gt; on the variable latestResponse.
        /// <para>If unable to connect, latestResponse instead has the string "Error : Couldn't connect".</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetScores()
        {
            UnityWebRequest apiRequest = UnityWebRequest.Get(apiUrlBase + apiDownloadScores);
            apiRequest.SendWebRequest();

            while (!apiRequest.isDone)
            {
                yield return null;
            }

            string response = "";
            List<Score> scores = new List<Score>();
            switch (apiRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    response = "Error : Couldn't connect";
                    break;
                case UnityWebRequest.Result.Success:
                    response = apiRequest.downloadHandler.text;
                    scores = Score.FromJSON(response);
                    break;
                default:
                    break;
            }

            latestResponse = scores;
        }

        /// <summary>
        /// Returns the variable latestResponse, containing the latest results from an online query.
        /// </summary>
        /// <returns>The object latestResponse. May be a string, may be a List of Score, may be null.</returns>
        public object GetResponse()
        {
            return latestResponse;
        }

        /// <summary>
        /// Resets latestResponse to be null.
        /// </summary>
        public void ResetResponse()
        {
            latestResponse = null;
        }

        public IEnumerator TestConnectionServer()
        {
            UnityWebRequest apiRequest = UnityWebRequest.Get(apiUrlBase + apiTestConnection);
            apiRequest.SendWebRequest();

            while (!apiRequest.isDone)
            {
                yield return null;
            }

            string response = "";
            List<MockClass> charList = new List<MockClass>();
            switch (apiRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    response = "Error : Couldn't connect";
                    break;
                case UnityWebRequest.Result.Success:
                    response = apiRequest.downloadHandler.text;
                    Debug.Log(response);
                    charList = MockClass.FromJSONToCharList(response);
                    break;
                default:
                    break;
            }

            string result = "";
            foreach (MockClass c in charList)
            {
                result += c.a;
            }
            Debug.Log("Clé encryptage : " + result);
            latestResponse = result;
        }

        [Serializable]
        private class MockClass
        {
            public string a;

            public static List<MockClass> FromJSONToCharList(string json)
            {
                JArray jchars = JArray.Parse(json);
                List<MockClass> chars = jchars.ToObject<List<MockClass>>();

                return chars;
            }
        }
    }
}