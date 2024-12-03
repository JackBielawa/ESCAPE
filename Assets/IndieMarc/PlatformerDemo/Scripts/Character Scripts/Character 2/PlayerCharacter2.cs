using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerCharacterTwo : MonoBehaviour
    {
        public int player_id = 2;

        [Header("Stats")]
        public float max_hp = 100f;

        [Header("Status")]
        public bool invulnerable = false;

        [Header("Movement")]
        public float move_accel = 1f;
        public float move_deccel = 1f;
        public float move_max = 1f;

        [Header("Jump")]
        public bool can_jump = true;
        public bool double_jump = true;
        public float jump_strength = 1f;
        public float jump_time_min = 0.1f;
        public float jump_time_max = 0.5f;
        public float jump_gravity = 1f;
        public float jump_fall_gravity = 2f;
        public float jump_move_percent = 0.75f;
        public LayerMask ground_layer;
        public float ground_raycast_dist = 0.1f;

        [Header("Crouch")]
        public bool can_crouch = true;
        public float crouch_coll_percent = 0.5f;

        [Header("Fall Below Level")]
        public bool reset_when_fall = true;
        public float fall_pos_y = -5f;
        public float fall_damage_percent = 0.25f;

        [Header("Transformation")]
        public GameObject rectanglePrefab; // Assign your Rectangle prefab here

        public UnityAction onDeath;
        public UnityAction onHit;
        public UnityAction onJump;
        public UnityAction onLand;
        public UnityAction onCrouch;

        private Rigidbody2D rigid;
        private BoxCollider2D box_coll;
        private Vector2 coll_start_size;
        private Vector2 coll_start_offset;
        private Vector3 start_scale;
        private Vector3 last_ground_pos;
        private Vector3 average_ground_pos;

        private Vector2 move;
        private Vector2 move_input;
        private bool jump_press;
        private bool jump_hold;
        private bool action_press;
        private bool action_hold;

        private float hp;
        private bool is_dead = false;
        private bool was_grounded = false;
        private bool is_grounded = false;
        private bool is_crouch = false;
        private bool is_jumping = false;
        private bool is_double_jump = false;
        private bool disable_controls = false;
        private float grounded_timer = 0f;
        private float jump_timer = 0f;
        private float hit_timer = 0f;

        // New variables for power-up and transformation
        private bool hasPowerUp = false;
        private bool isRectangleForm = false;
        private GameObject rectangleInstance;
        private Vector2 originalColliderSize;
        private Vector2 originalColliderOffset;
        private Vector2 rectangleColliderSize;
        private Vector2 rectangleColliderOffset;

        private static Dictionary<int, PlayerCharacterTwo> character_list = new Dictionary<int, PlayerCharacterTwo>();

        void Awake()
        {
            character_list[player_id] = this;
            rigid = GetComponent<Rigidbody2D>();
            box_coll = GetComponent<BoxCollider2D>();
            coll_start_size = box_coll.size;
            coll_start_offset = box_coll.offset;
            start_scale = transform.localScale;
            average_ground_pos = transform.position;
            last_ground_pos = transform.position;
            hp = max_hp;

            // Store original collider size and offset
            originalColliderSize = box_coll.size;
            originalColliderOffset = box_coll.offset;

            // Debugging
            Debug.Log($"[PlayerCharacterTwo] Awake: Player ID {player_id}, Start Position {transform.position}");
        }

        void OnDestroy()
        {
            character_list.Remove(player_id);
            Debug.Log($"[PlayerCharacterTwo] OnDestroy: Player ID {player_id} removed.");
        }

        void Start()
        {
            // Additional startup debugging if needed
            Debug.Log($"[PlayerCharacterTwo] Start: Player ID {player_id}");
        }

        // Handle physics
        void FixedUpdate()
        {
            if (is_dead)
                return;

            // Movement velocity
            float desiredSpeed = Mathf.Abs(move_input.x) > 0.1f ? move_input.x * move_max : 0f;
            float acceleration = Mathf.Abs(move_input.x) > 0.1f ? move_accel : move_deccel;
            acceleration = !is_grounded ? jump_move_percent * acceleration : acceleration;
            move.x = Mathf.MoveTowards(move.x, desiredSpeed, acceleration * Time.fixedDeltaTime);

            // Debugging
            Debug.Log($"[FixedUpdate] Player {player_id}: move_input.x: {move_input.x}, desiredSpeed: {desiredSpeed}, acceleration: {acceleration}, move.x: {move.x}");

            UpdateFacing();
            UpdateJump();
            UpdateCrouch();

            // Move
            rigid.velocity = move;

            // Debugging
            Debug.Log($"[FixedUpdate] Player {player_id}: Rigidbody velocity: {rigid.velocity}");
        }

        // Handle render and controls
        void Update()
        {
            if (is_dead)
                return;

            hit_timer += Time.deltaTime;
            grounded_timer += Time.deltaTime;

            // Controls
            PlayerControlsTwo controls = PlayerControlsTwo.Get(player_id);
            move_input = !disable_controls ? controls.GetMove() : Vector2.zero;
            jump_press = !disable_controls ? controls.GetJumpDown() : false;
            jump_hold = !disable_controls ? controls.GetJumpHold() : false;
            action_press = !disable_controls ? controls.GetActionDown() : false;
            action_hold = !disable_controls ? controls.GetActionHold() : false;

            // Debugging
            Debug.Log($"[Update] Player {player_id}: move_input: {move_input}, jump_press: {jump_press}, jump_hold: {jump_hold}");

            if (jump_press || move_input.y > 0.5f)
                Jump();

            // Handle transformation
            if (hasPowerUp && action_press)
            {
                ToggleForm();
            }

            // Reset when fall
            if (transform.position.y < fall_pos_y - GetSize().y)
            {
                // For debugging, you can comment out the damage and death to prevent character from dying
                // TakeDamage(max_hp * fall_damage_percent);

                if (reset_when_fall)
                    Teleport(last_ground_pos);

                // Debugging
                Debug.Log($"[Update] Player {player_id} fell below fall_pos_y: {fall_pos_y}, Teleporting to last ground position: {last_ground_pos}");
            }
        }

        private void UpdateFacing()
        {
            if (Mathf.Abs(move.x) > 0.01f)
            {
                float side = (move.x < 0f) ? -1f : 1f;
                transform.localScale = new Vector3(start_scale.x * side, start_scale.y, start_scale.z);

                // Debugging
                Debug.Log($"[UpdateFacing] Player {player_id}: Facing direction changed. New localScale.x: {transform.localScale.x}");
            }
        }

        private void UpdateJump()
        {
            // Update the previous grounded state
            was_grounded = is_grounded;

            // Detect if the character is grounded
            is_grounded = DetectGrounded();

            jump_timer += Time.fixedDeltaTime;

            // Debugging
            Debug.Log($"[UpdateJump] Player {player_id}: was_grounded: {was_grounded}, is_grounded: {is_grounded}, is_jumping: {is_jumping}, jump_timer: {jump_timer}");

            // Handle jump timing
            if (is_jumping && !jump_hold && jump_timer > jump_time_min)
                is_jumping = false;
            if (is_jumping && jump_timer > jump_time_max)
                is_jumping = false;

            // Apply gravity and vertical movement
            if (!is_grounded)
            {
                // Character is in the air
                float gravity = !is_jumping ? jump_fall_gravity : jump_gravity; // Adjust gravity based on jumping state
                move.y = Mathf.MoveTowards(move.y, -move_max * 2f, gravity * Time.fixedDeltaTime);

                // Debugging
                Debug.Log($"[UpdateJump] Player {player_id}: In air. Applying gravity: {gravity}, move.y: {move.y}");
            }
            else if (!is_jumping)
            {
                // Character is on the ground and not jumping
                move.y = 0f;

                // Debugging
                Debug.Log($"[UpdateJump] Player {player_id}: Grounded. Resetting vertical movement.");
            }

            if (!is_grounded)
                grounded_timer = 0f;

            // Update grounded position
            if (!was_grounded && is_grounded)
                average_ground_pos = transform.position;
            if (is_grounded)
                average_ground_pos = Vector3.Lerp(transform.position, average_ground_pos, 1f * Time.deltaTime);

            // Save last landed position
            if (is_grounded && grounded_timer > 1f)
                last_ground_pos = average_ground_pos;

            // Handle landing event
            if (!was_grounded && is_grounded)
            {
                onLand?.Invoke();

                // Debugging
                Debug.Log($"[UpdateJump] Player {player_id}: Landed on ground.");
            }
        }

        private void UpdateCrouch()
        {
            if (!can_crouch)
                return;

            // Crouch
            bool was_crouch = is_crouch;
            if (move_input.y < -0.1f && is_grounded)
            {
                is_crouch = true;
                move = Vector2.zero;
                box_coll.size = new Vector2(coll_start_size.x, coll_start_size.y * crouch_coll_percent);
                box_coll.offset = new Vector2(coll_start_offset.x, coll_start_offset.y - coll_start_size.y * (1f - crouch_coll_percent) / 2f);

                if (!was_crouch && is_crouch)
                {
                    onCrouch?.Invoke();

                    // Debugging
                    Debug.Log($"[UpdateCrouch] Player {player_id}: Started crouching.");
                }
            }
            else
            {
                is_crouch = false;
                box_coll.size = coll_start_size;
                box_coll.offset = coll_start_offset;

                if (was_crouch && !is_crouch)
                {
                    // Debugging
                    Debug.Log($"[UpdateCrouch] Player {player_id}: Stopped crouching.");
                }
            }
        }

        public void Jump(bool force_jump = false)
        {
            if (can_jump && (!is_crouch || force_jump))
            {
                if (is_grounded || force_jump || (!is_double_jump && double_jump))
                {
                    is_double_jump = !is_grounded;
                    move.y = jump_strength;
                    jump_timer = 0f;
                    is_jumping = true;
                    onJump?.Invoke();

                    // Debugging
                    Debug.Log($"[Jump] Player {player_id}: Jump initiated. is_double_jump: {is_double_jump}, move.y: {move.y}");
                }
            }
        }

        private bool DetectGrounded()
        {
            Vector2 position = rigid.position + box_coll.offset + Vector2.down * (box_coll.size.y / 2f);
            float rayLength = ground_raycast_dist;

            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, rayLength, ground_layer);
            Debug.DrawRay(position, Vector2.down * rayLength, Color.red);

            if (hit.collider != null && hit.collider != box_coll && !hit.collider.isTrigger)
            {
                Debug.Log($"[DetectGrounded] Player {player_id}: Hit detected with {hit.collider.name} at point {hit.point}");
                return true;
            }
            else
            {
                Debug.Log($"[DetectGrounded] Player {player_id}: No ground detected.");
                return false;
            }
        }

        public void Teleport(Vector3 pos)
        {
            transform.position = pos;
            move = Vector2.zero;
            is_jumping = false;

            // Debugging
            Debug.Log($"[Teleport] Player {player_id}: Teleported to position {pos}");
        }

        public void HealDamage(float heal)
        {
            if (!is_dead)
            {
                hp += heal;
                hp = Mathf.Min(hp, max_hp);

                // Debugging
                Debug.Log($"[HealDamage] Player {player_id}: Healed {heal} HP. Current HP: {hp}");
            }
        }

        public void TakeDamage(float damage)
        {
            if (!is_dead && !invulnerable && hit_timer > 0f)
            {
                hp -= damage;
                hit_timer = -1f;

                // Debugging
                Debug.Log($"[TakeDamage] Player {player_id}: Took {damage} damage. Current HP: {hp}");

                if (hp <= 0f)
                {
                    Kill();
                }
                else
                {
                    onHit?.Invoke();
                }
            }
        }

        public void Kill()
        {
            if (!is_dead)
            {
                is_dead = true;
                rigid.velocity = Vector2.zero;
                move = Vector2.zero;
                move_input = Vector2.zero;

                onDeath?.Invoke();

                // Debugging
                Debug.Log($"[Kill] Player {player_id}: Character has died.");

                // For debugging, respawn the character after a delay
                StartCoroutine(RespawnCharacter());
            }
        }

        private IEnumerator RespawnCharacter()
        {
            yield return new WaitForSeconds(2f); // Wait for 2 seconds
            is_dead = false;
            hp = max_hp;
            Teleport(last_ground_pos);
            EnableControls();

            // Debugging
            Debug.Log($"[RespawnCharacter] Player {player_id}: Character has respawned.");
        }

        public void DisableControls() { disable_controls = true; }
        public void EnableControls() { disable_controls = false; }

        public Vector2 GetMove()
        {
            return move;
        }

        public Vector2 GetFacing()
        {
            return Vector2.right * Mathf.Sign(transform.localScale.x);
        }

        public bool IsJumping()
        {
            return is_jumping;
        }

        public bool IsGrounded()
        {
            return is_grounded;
        }

        public bool IsCrouching()
        {
            return is_crouch;
        }

        public float GetHP()
        {
            return hp;
        }

        public bool IsDead()
        {
            return is_dead;
        }

        public Vector2 GetSize()
        {
            if (box_coll != null)
                return new Vector2(Mathf.Abs(transform.localScale.x) * box_coll.size.x, Mathf.Abs(transform.localScale.y) * box_coll.size.y);
            return new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (is_dead)
                return;

            // Debugging
            Debug.Log($"[OnCollisionEnter2D] Player {player_id}: Collided with {collision.collider.name} on layer {LayerMask.LayerToName(collision.collider.gameObject.layer)}");
        }

        // New method to collect the power-up
        public void CollectPowerUp()
        {
            hasPowerUp = true;
            Debug.Log($"[CollectPowerUp] Player {player_id}: Power-up collected.");
        }

        // New method to toggle between forms
        private void ToggleForm()
        {
            isRectangleForm = !isRectangleForm;

            if (isRectangleForm)
            {
                // Switch to rectangle form
                if (rectangleInstance == null)
                {
                    rectangleInstance = Instantiate(rectanglePrefab, transform);
                    rectangleInstance.transform.localPosition = Vector3.zero;

                    // Get rectangle collider size and offset
                    BoxCollider2D rectCollider = rectangleInstance.GetComponent<BoxCollider2D>();
                    if (rectCollider != null)
                    {
                        rectangleColliderSize = rectCollider.size;
                        rectangleColliderOffset = rectCollider.offset;
                    }
                    else
                    {
                        // Set default values if collider not found
                        rectangleColliderSize = originalColliderSize;
                        rectangleColliderOffset = originalColliderOffset;
                    }

                    // Disable rectangle's own collider to avoid conflicts
                    if (rectCollider != null)
                        rectCollider.enabled = false;
                }
                rectangleInstance.SetActive(true);

                // Disable player's visual components
                GetComponent<SpriteRenderer>().enabled = false;

                // Adjust collider size
                box_coll.size = rectangleColliderSize;
                box_coll.offset = rectangleColliderOffset;

                Debug.Log($"[ToggleForm] Player {player_id}: Transformed into rectangle.");
            }
            else
            {
                // Switch to normal form
                if (rectangleInstance != null)
                {
                    rectangleInstance.SetActive(false);
                }
                // Enable player's visual components
                GetComponent<SpriteRenderer>().enabled = true;

                // Restore collider size
                box_coll.size = originalColliderSize;
                box_coll.offset = originalColliderOffset;

                Debug.Log($"[ToggleForm] Player {player_id}: Reverted to normal form.");
            }
        }

        public static PlayerCharacterTwo GetNearest(Vector3 pos, float range = 99999f, bool alive_only = false)
        {
            PlayerCharacterTwo nearest = null;
            float min_dist = range;
            foreach (PlayerCharacterTwo character in GetAll())
            {
                if (!alive_only || !character.IsDead())
                {
                    float dist = (pos - character.transform.position).magnitude;
                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        nearest = character;
                    }
                }
            }
            return nearest;
        }

        public static PlayerCharacterTwo Get(int player_id)
        {
            if (character_list.TryGetValue(player_id, out PlayerCharacterTwo character))
            {
                return character;
            }
            return null;
        }

        public static PlayerCharacterTwo[] GetAll()
        {
            PlayerCharacterTwo[] list = new PlayerCharacterTwo[character_list.Count];
            character_list.Values.CopyTo(list, 0);
            return list;
        }
    }
}
