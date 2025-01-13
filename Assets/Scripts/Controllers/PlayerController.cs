using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField][Range(0f, 20f)]
    private float flyForce;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GameManager.s_this.OnStart.AddListener(delegate { ToggleGravity(true); });
        GameManager.s_this.OnEnd.AddListener(delegate { ToggleGravity(false); });
    }

    private void ToggleGravity(bool flag)
    {
        if (flag)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void FlyUp()
    {
        if (GameManager.s_this.IsPlaying)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(0, flyForce), ForceMode.Impulse);
        }
    }
}
