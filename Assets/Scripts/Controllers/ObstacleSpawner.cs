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

        private void Start()
        {
            GameManager.s_this.OnStart.AddListener(delegate { StartSpawning(); });
            GameManager.s_this.OnEnd.AddListener(delegate { CancelInvoke("SpawnObstacle"); });
        }

        // Start is called before the first frame update
        private void StartSpawning()
        {
            InvokeRepeating("SpawnObstacle", 1f, spawnInterval);
        }

        public static Transform GetDestination()
        {
            return s_this.destination;
        }

        private void SpawnObstacle()
        {
            Instantiate(obstaclePrefab, new Vector3(transform.position.x, Random.Range(lowestRandomPosition, highestRandomPosition), transform.position.z), obstaclePrefab.transform.rotation, transform);
        }
    }
}