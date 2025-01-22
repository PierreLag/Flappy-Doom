using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace FlappyDoom
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField]
        private AudioSource musicPlayer;
        [SerializeField]
        private AudioSource scoreUpPlayer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            GameManager.s_this.OnEnd.AddListener(delegate { StopMusic(); });
            GameManager.s_this.OnStart.AddListener(delegate { PlayMusic(); });
        }

        public void PlayMusic()
        {
            musicPlayer.Play();
        }

        public void StopMusic()
        {
            musicPlayer.Stop();
        }

        public static void PlayScoreUp()
        {
            instance.scoreUpPlayer.Play();
        }
    }
}