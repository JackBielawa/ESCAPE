using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Lever : MonoBehaviour
    {
        public Sprite leverLeft;
        public Sprite leverRight;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;
        private LeverState state = LeverState.Left;
        private Bridge bridgeToActivate;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            spriteRenderer.sprite = leverLeft;

            // Find the bridge by tag
            GameObject bridgeObj = GameObject.FindGameObjectWithTag("Bridge");
            if (bridgeObj != null)
            {
                bridgeToActivate = bridgeObj.GetComponent<Bridge>();
                if (bridgeToActivate == null)
                {
                    Debug.LogError("Bridge object found but missing Bridge component!");
                }
            }
            else
            {
                Debug.LogError("No object with Bridge tag found!");
            }
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("PlayerTwo"))
            {
                ToggleLever();
            }
        }

        private void ToggleLever()
        {
            state = state == LeverState.Left ? LeverState.Right : LeverState.Left;
            spriteRenderer.sprite = state == LeverState.Left ? leverLeft : leverRight;

            if (bridgeToActivate != null)
            {
                bridgeToActivate.SetActive(state == LeverState.Right);
            }
        }
    }
}