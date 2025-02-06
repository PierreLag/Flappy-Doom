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

        // We start the music when the game starts, and stop it once the game is over.
        private void Start()
        {
            GameManager.s_this.OnEnd.AddListener(delegate { StopMusic(); });
            GameManager.s_this.OnStart.AddListener(delegate { PlayMusic(); });
        }

        /// <summary>
        /// Plays the music attached to the referenced Music Player's AudioSource.
        /// </summary>
        public void PlayMusic()
        {
            musicPlayer.Play();
        }

        /// <summary>
        /// Stops the music attached to the referenced Music Player's AudioSource.
        /// </summary>
        public void StopMusic()
        {
            musicPlayer.Stop();
        }

        /// <summary>
        /// Plays the sound effect attached to the referenced Sound Player's AudioSource.
        /// </summary>
        public static void PlayScoreUp()
        {
            instance.scoreUpPlayer.Play();
        }
    }
}