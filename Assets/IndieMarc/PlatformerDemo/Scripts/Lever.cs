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

        public bool can_be_center; // Determines if the lever can move to the center state
        public LeverState state;  // Current state of the lever
        public bool no_return = false; // Prevent the lever from returning to its initial state
        public bool reset_on_dead = true; // Reset lever state when the player dies

        public GameObject Bridge; // The bridge GameObject to activate

        private SpriteRenderer render;
        private LeverState start_state; // The initial state of the lever
        private LeverState prev_state; // The previous state of the lever
        private float timer = 0f; // Timer for activation cooldown

        public UnityAction OnTriggerLever; // Event triggered when the lever is activated

        private static List<Lever> levers = new List<Lever>();

        private void Awake()
        {
            levers.Add(this);
            render = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            start_state = state; // Preserve the initial state
            prev_state = state;

            // Ensure the bridge is hidden at the start if the lever is not in an activating state
            if (state != LeverState.left && state != LeverState.right)
            {
                if (Bridge != null)
                    Bridge.SetActive(false);
            }

            ChangeSprite(); // Update the sprite based on the lever's initial state
        }

        void Update()
        {
            timer += Time.deltaTime;

            // Check if the state has changed and update the sprite
            if (state != prev_state)
            {
                ChangeSprite();
                prev_state = state;
            }
        }

        private void OnDestroy()
        {
            levers.Remove(this);
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            // Check if the collision is with a player character and activate the lever
            if (coll.gameObject.GetComponent<PlayerCharacterOne>() || coll.gameObject.GetComponent<PlayerCharacterTwo>())
            {
                if (state == LeverState.disabled)
                    return;

                Activate();
            }
        }

        public void Activate()
        {
            // Prevent rapid reactivation
            if (timer < 0f)
                return;

            if (!no_return || state == start_state)
            {
                timer = -0.8f; // Set cooldown timer

                // Change the state of the lever
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

                // Play lever activation sound
                GetComponent<AudioSource>().Play();

                // Invoke the lever trigger event
                OnTriggerLever?.Invoke();

                // Handle bridge activation
                if (state == LeverState.left || state == LeverState.right)
                {
                    if (Bridge != null)
                        Bridge.SetActive(true);
                }
                else
                {
                    if (Bridge != null)
                        Bridge.SetActive(false);
                }
            }
        }

        private void ChangeSprite()
        {
            // Update the sprite based on the lever's current state
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

            // Disable sprite if `no_return` is true and the lever is not in its starting state
            if (no_return && state != start_state)
            {
                render.sprite = lever_disabled;
            }
        }

        public void ResetOne()
        {
            // Reset the lever to its starting state if configured to do so on death
            if (reset_on_dead)
            {
                state = start_state;

                // Ensure the bridge is hidden if the lever resets to a non-activating state
                if (state != LeverState.left && state != LeverState.right)
                {
                    if (Bridge != null)
                        Bridge.SetActive(false);
                }

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
