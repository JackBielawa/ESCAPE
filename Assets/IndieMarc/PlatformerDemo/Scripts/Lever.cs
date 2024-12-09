using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndieMarc.Platformer
{
    public enum LeverState
    {
        left, center, right, disabled
    }

    public class Lever : MonoBehaviour
    {
        public Sprite lever_center;
        public Sprite lever_left;
        public Sprite lever_right;
        public Sprite lever_disabled;

        public bool can_be_center;
        public LeverState state;
        public bool no_return = false;
        public bool reset_on_dead = true;

        private GameObject Bridge; // We'll find this by tag instead of assigning manually
        private SpriteRenderer render;
        private LeverState start_state;
        private LeverState prev_state;
        private float timer = 0f;

        public UnityAction OnTriggerLever;

        private static List<Lever> levers = new List<Lever>();

        private void Awake()
        {
            levers.Add(this);
            render = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            // Find the Bridge by tag
            Bridge = GameObject.FindGameObjectWithTag("Bridge");

            start_state = state;
            prev_state = state;

            // Ensure the bridge state matches the lever's initial state
            UpdateBridgeState();
            ChangeSprite();
        }

        void Update()
        {
            timer += Time.deltaTime;

            // If the state has changed, update the sprite and bridge visibility
            if (state != prev_state)
            {
                ChangeSprite();
                UpdateBridgeState();
                prev_state = state;
            }
        }

        private void OnDestroy()
        {
            levers.Remove(this);
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            // Check if collided with a player and then activate the lever
            if (coll.gameObject.GetComponent<PlayerCharacterOne>() || coll.gameObject.GetComponent<PlayerCharacterTwo>())
            {
                if (state == LeverState.disabled)
                    return;

                Activate();
            }
        }

        public void Activate()
        {
            if (timer < 0f)
                return;

            if (!no_return || state == start_state)
            {
                timer = -0.8f; // cooldown

                // Cycle through states
                if (state == LeverState.left)
                {
                    state = (can_be_center) ? LeverState.center : LeverState.right;
                }
                else if (state == LeverState.center)
                {
                    state = LeverState.right;
                }
                else if (state == LeverState.right)
                {
                    state = LeverState.left;
                }

                // Play lever sound if available
                AudioSource audio = GetComponent<AudioSource>();
                if (audio != null)
                    audio.Play();

                // Trigger event
                OnTriggerLever?.Invoke();
            }
        }

        private void ChangeSprite()
        {
            // Change the lever's sprite based on its current state
            if (state == LeverState.left)
            {
                render.sprite = lever_left;
            }
            else if (state == LeverState.center)
            {
                render.sprite = lever_center;
            }
            else if (state == LeverState.right)
            {
                render.sprite = lever_right;
            }
            else if (state == LeverState.disabled)
            {
                render.sprite = lever_disabled;
            }

            // If no_return is true and we've moved from the start state, disable sprite
            if (no_return && state != start_state)
            {
                render.sprite = lever_disabled;
            }
        }

        private void UpdateBridgeState()
        {
            // If no bridge found, just skip
            if (Bridge == null)
                return;

            // Bridge should be visible only if the lever is in left or right state
            bool shouldShow = (state == LeverState.left || state == LeverState.right);

            // Activate or deactivate the bridge accordingly
            Bridge.SetActive(shouldShow);
        }

        public void ResetOne()
        {
            if (reset_on_dead)
            {
                state = start_state;
                UpdateBridgeState();
                ChangeSprite();
            }
        }

        public static void ResetAll()
        {
            foreach (Lever lever in levers)
            {
                lever.ResetOne();
            }
        }
    }
}
