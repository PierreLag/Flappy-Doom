using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyDoom
{
    public class ObstacleRemover : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
        }
    }
}