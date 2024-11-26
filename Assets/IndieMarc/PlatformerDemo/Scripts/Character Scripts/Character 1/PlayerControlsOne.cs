using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player controls for platformer demo
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.Platformer
{
    public class PlayerControlsOne : MonoBehaviour
    {
        public int player_id;
        public KeyCode left_key = KeyCode.A;
        public KeyCode right_key = KeyCode.D;
        public KeyCode up_key = KeyCode.W;
        public KeyCode down_key = KeyCode.S;
        public KeyCode jump_key = KeyCode.Space;
        public KeyCode action_key = KeyCode.E;

        private Vector2 move = Vector2.zero;
        private bool jump_press = false;
        private bool jump_hold = false;
        private bool action_press = false;
        private bool action_hold = false;

        private static Dictionary<int, PlayerControlsOne> controls = new Dictionary<int, PlayerControlsOne>();

        void Awake()
        {
            controls[player_id] = this;
        }

        void OnDestroy()
        {
            controls.Remove(player_id);
        }

        void Update()
        {
            move = Vector2.zero;
            jump_hold = false;
            jump_press = false;
            action_hold = false;
            action_press = false;

            if (Input.GetKey(left_key))
                move += -Vector2.right;
            if (Input.GetKey(right_key))
                move += Vector2.right;
            if (Input.GetKey(up_key))
                move += Vector2.up;
            if (Input.GetKey(down_key))
                move += -Vector2.up;
            if (Input.GetKey(jump_key))
                jump_hold = true;
            if (Input.GetKeyDown(jump_key))
                jump_press = true;
            if (Input.GetKey(action_key))
                action_hold = true;
            if (Input.GetKeyDown(action_key))
                action_press = true;

            float move_length = Mathf.Min(move.magnitude, 1f);
            move = move.normalized * move_length;

            // Debugging
            Debug.Log($"[PlayerControlsOne] Player {player_id}: Move={move}, JumpPress={jump_press}, JumpHold={jump_hold}");
        }

        //------ These functions should be called from the Update function, not FixedUpdate
        public Vector2 GetMove()
        {
            return move;
        }

        public bool GetJumpDown()
        {
            return jump_press;
        }

        public bool GetJumpHold()
        {
            return jump_hold;
        }

        public bool GetActionDown()
        {
            return action_press;
        }

        public bool GetActionHold()
        {
            return action_hold;
        }

        //-----------

        public static PlayerControlsOne Get(int player_id)
        {
            if (controls.TryGetValue(player_id, out PlayerControlsOne control))
            {
                return control;
            }
            return null;
        }

        public static PlayerControlsOne[] GetAll()
        {
            PlayerControlsOne[] list = new PlayerControlsOne[controls.Count];
            controls.Values.CopyTo(list, 0);
            return list;
        }
    }
}
