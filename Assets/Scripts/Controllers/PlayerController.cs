using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField][Range(0f, 20f)]
    private float flyForce;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlyUp()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(0, flyForce), ForceMode2D.Impulse);
    }
}
