using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bridge : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;
        private Rigidbody2D rigidBody;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();

            rigidBody.bodyType = RigidbodyType2D.Static;
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            spriteRenderer.enabled = active;
            boxCollider.enabled = active;
            rigidBody.simulated = active;
        }
    }
}