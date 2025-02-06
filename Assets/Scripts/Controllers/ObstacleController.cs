using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;

namespace FlappyDoom
{
    public class ObstacleController : MonoBehaviour
    {
        [Serializable]
        private class TriggeredEvent : UnityEvent { }

        [SerializeField]
        private TriggeredEvent OnCollisionCustom;
        [SerializeField]
        private TriggeredEvent OnTriggerEnterCustom;
        [SerializeField]
        private bool isStatic;
        [SerializeField]
        private float movementDuration;

        // If this obstacle is not static, moves it on the X axis towards its destination. On game end, stops its movement, on new game start destroys the object.
        void Start()
        {
            if (!isStatic)
            {
                transform.DOMoveX(ObstacleSpawner.GetDestination().position.x, movementDuration).SetEase(Ease.Linear);
                GameManager.s_this.OnEnd.AddListener(delegate { DOTween.Kill(transform); });
                GameManager.s_this.OnStart.AddListener(delegate { Destroy(gameObject); });
            }
        }

        // If the player hits this obstacle's Trigger collision, invokes the action attached on the Editor.
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.TryGetComponent<PlayerController>(out PlayerController controller))
                OnTriggerEnterCustom.Invoke();
        }

        // If the player hits this obstacle's physics collision, invokes the action attached on the Editor.
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent<PlayerController>(out PlayerController controller))
                OnCollisionCustom.Invoke();
        }
    }
}