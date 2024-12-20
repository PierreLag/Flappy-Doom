using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;

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

    // Start is called before the first frame update
    void Start()
    {
        if (!isStatic)
            transform.DOMoveX(ObstacleSpawner.GetDestination().position.x, movementDuration).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController controller))
            OnTriggerEnterCustom.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerController>(out PlayerController controller))
            OnCollisionCustom.Invoke();
    }
}
