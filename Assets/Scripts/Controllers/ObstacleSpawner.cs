using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyDoom
{
    public class ObstacleSpawner : MonoBehaviour
    {
        private static ObstacleSpawner s_this;

        [SerializeField]
        private GameObject obstaclePrefab;
        [SerializeField]
        private float lowestRandomPosition;
        [SerializeField]
        private float highestRandomPosition;
        [SerializeField]
        private float spawnInterval;
        [SerializeField]
        private Transform destination;

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

        // Starts the spawning of obstacles on game start, stops it on game end.
        private void Start()
        {
            GameManager.s_this.OnStart.AddListener(delegate { StartSpawning(); });
            GameManager.s_this.OnEnd.AddListener(delegate { CancelInvoke("SpawnObstacle"); });
        }

        /// <summary>
        /// Starts spawning obstacles with a frequency determined by spawnInterval.
        /// </summary>
        private void StartSpawning()
        {
            InvokeRepeating("SpawnObstacle", 1f, spawnInterval);
        }

        /// <summary>
        /// Returns the transform of the obstacles' destination.
        /// </summary>
        /// <returns>A Transform with the position of the obstacles' destination.</returns>
        public static Transform GetDestination()
        {
            return s_this.destination;
        }

        /// <summary>
        /// Instantiates a new obstacle, with a Y coordinate ranging from this spawner's lowestRandomPosition to its highestRandomPosition.
        /// </summary>
        private void SpawnObstacle()
        {
            Instantiate(obstaclePrefab, new Vector3(transform.position.x, Random.Range(lowestRandomPosition, highestRandomPosition), transform.position.z), obstaclePrefab.transform.rotation, transform);
        }
    }
}