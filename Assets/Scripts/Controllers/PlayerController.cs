using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyDoom
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 20f)]
        private float flyForce;

        [SerializeField]
        private Texture2D aliveTex;
        [SerializeField]
        private Texture2D deadTex;

        private Rigidbody rb;
        private Vector3 initialPosition;
        private MeshRenderer meshRenderer;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            initialPosition = transform.position;
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        private void Start()
        {
            GameManager.s_this.OnStart.AddListener(delegate { transform.position = initialPosition; ToggleGravity(true); meshRenderer.material.mainTexture = aliveTex; });
            GameManager.s_this.OnEnd.AddListener(delegate { ToggleGravity(false); PlayDeathSound(); meshRenderer.material.mainTexture = deadTex; });
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

        private void PlayDeathSound()
        {
            GetComponent<AudioSource>().Play();
        }
    }
}