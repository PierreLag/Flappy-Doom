using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

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

        private string username;

        private object latestResponse;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            username = PlayerPrefs.GetString("username", "");
            GameManager.s_this.OnEnd.AddListener(delegate { StartCoroutine(UploadScore()); });
        }

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

            await Task.Delay(50);
            ResetResponse();
        }

        public void LoadUsername()
        {
            username = PlayerPrefs.GetString("username");
        }

        public string GetUsername()
        {
            return username;
        }

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

        public object GetResponse()
        {
            return latestResponse;
        }

        public void ResetResponse()
        {
            latestResponse = null;
        }
    }
}