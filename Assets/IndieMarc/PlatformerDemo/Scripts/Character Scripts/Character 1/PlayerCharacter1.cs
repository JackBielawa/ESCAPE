using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Platformer character movement
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerCharacterOne : MonoBehaviour
    {
        public int player_id;

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

        [Header("Shooting")]
        public bool can_shoot = false; // Indicates if the player can shoot
        public GameObject fireballPrefab; // Assign the fireball prefab in the Inspector
        public Transform firePoint; // The point from where the fireball will be instantiated

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
        public bool is_dead = false;
        private bool was_grounded = false;
        private bool is_grounded = false;
        private bool is_crouch = false;
        private bool is_jumping = false;
        private bool is_double_jump = false;
        private bool disable_controls = false;
        private float grounded_timer = 0f;
        private float jump_timer = 0f;
        private float hit_timer = 0f;

        private bool facingRight = true; // Indicates if the player is facing right

        private static Dictionary<int, PlayerCharacterOne> character_list = new Dictionary<int, PlayerCharacterOne>();

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
        }

        void Start()
        {

            gameObject.SetActive(true);

            // If firePoint is not assigned, create it
            if (firePoint == null)
            {
                GameObject firePointObject = new GameObject("FirePoint");
                firePointObject.transform.parent = transform;
                firePoint = firePointObject.transform;

                // Set initial position based on facing direction
                firePoint.localPosition = new Vector3(0.5f, 0, 0);
            }

            is_dead = false;
        }

        void OnDestroy()
        {
            character_list.Remove(player_id);
        }

        void FixedUpdate()
        {
            if (is_dead)
                return;

            // Movement velocity
            float desiredSpeed = Mathf.Abs(move_input.x) > 0.1f ? move_input.x * move_max : 0f;
            float acceleration = Mathf.Abs(move_input.x) > 0.1f ? move_accel : move_deccel;
            acceleration = !is_grounded ? jump_move_percent * acceleration : acceleration;
            move.x = Mathf.MoveTowards(move.x, desiredSpeed, acceleration * Time.fixedDeltaTime);

            UpdateFacing();
            UpdateJump();
            UpdateCrouch();

            // Apply movement
            rigid.velocity = move;
        }

        void Update()
        {
            if (is_dead)
                return;

            hit_timer += Time.deltaTime;
            grounded_timer += Time.deltaTime;

            // Controls
            PlayerControlsOne controls = PlayerControlsOne.Get(player_id);
            move_input = !disable_controls ? controls.GetMove() : Vector2.zero;
            jump_press = !disable_controls ? controls.GetJumpDown() : false;
            jump_hold = !disable_controls ? controls.GetJumpHold() : false;
            action_press = !disable_controls ? controls.GetActionDown() : false;
            action_hold = !disable_controls ? controls.GetActionHold() : false;

            // Debugging
            Debug.Log($"[Update] can_shoot: {can_shoot}, action_press: {action_press}");

            if (jump_press)
                Jump();

            if (can_shoot && action_press)
            {
                Debug.Log("[Update] Attempting to shoot fireball.");
                ShootFireball();
            }

            // Reset when falling below a certain point
            if (transform.position.y < fall_pos_y - GetSize().y)
            {
                if (reset_when_fall)
                    Teleport(last_ground_pos);
            }
        }

        public void EnableShooting()
        {
            can_shoot = true;
            Debug.Log("[EnableShooting] Shooting enabled.");
        }

        public void DisableShooting()
        {
            can_shoot = false;
            Debug.Log("[DisableShooting] Shooting disabled.");
        }

        private void ShootFireball()
        {
            if (fireballPrefab != null && firePoint != null)
            {
                // Determine the spawn position based on facing direction
                Vector3 spawnPosition = transform.position + new Vector3(facingRight ? 0.5f : -0.5f, 0, 0);

                // Determine the rotation based on facing direction
                Quaternion spawnRotation = facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

                // Instantiate the fireball with the correct position and rotation
                GameObject fireball = Instantiate(fireballPrefab, spawnPosition, spawnRotation);

                Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
                if (fireballRb != null)
                {
                    // Set the velocity based on the facing direction
                    Vector2 direction = facingRight ? Vector2.right : Vector2.left;
                    Fireball fireballScript = fireball.GetComponent<Fireball>();
                    float speed = fireballScript != null ? fireballScript.speed : 5f;
                    fireballRb.velocity = direction * speed;
                }
            }
        }




        private void UpdateFacing()
        {
            if (Mathf.Abs(move.x) > 0.01f)
            {
                // Determine the facing direction
                facingRight = move.x > 0f;
                float side = facingRight ? 1f : -1f;

                // Flip the player scale
                transform.localScale = new Vector3(start_scale.x * side, start_scale.y, start_scale.z);

                // Adjust the FirePoint's position
                if (firePoint != null)
                {
                    firePoint.localPosition = new Vector3(facingRight ? 0.5f : -0.5f, firePoint.localPosition.y, firePoint.localPosition.z);
                    firePoint.localRotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
                }
            }
        

    }

    private void UpdateJump()
        {
            was_grounded = is_grounded;

            // Detect if the character is grounded
            is_grounded = DetectGrounded();

            jump_timer += Time.fixedDeltaTime;

            // Handle jump timing
            if (is_jumping && !jump_hold && jump_timer > jump_time_min)
                is_jumping = false;
            if (is_jumping && jump_timer > jump_time_max)
                is_jumping = false;

            // Apply gravity and vertical movement
            if (!is_grounded)
            {
                // Character is in the air
                float gravity = !is_jumping ? jump_fall_gravity : jump_gravity;
                move.y = Mathf.MoveTowards(move.y, -move_max * 2f, gravity * Time.fixedDeltaTime);
            }
            else if (!is_jumping)
            {
                // Character is on the ground and not jumping
                move.y = 0f;
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
            }
        }

        private void UpdateCrouch()
        {
            if (!can_crouch)
                return;

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
                }
            }
            else
            {
                is_crouch = false;
                box_coll.size = coll_start_size;
                box_coll.offset = coll_start_offset;
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
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Teleport(Vector3 pos)
        {
            transform.position = pos;
            move = Vector2.zero;
            is_jumping = false;
        }

        public void HealDamage(float heal)
        {
            if (!is_dead)
            {
                hp += heal;
                hp = Mathf.Min(hp, max_hp);
            }
        }

        public void TakeDamage(float damage)
        {
            if (!is_dead && !invulnerable && hit_timer > 0f)
            {
                hp -= damage;
                hit_timer = -1f;

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

                // Respawn the character after a delay
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

            if (collision.gameObject.CompareTag("LavaSquare"))
            {
                is_dead = true;
                Debug.Log("Player1 hit the lavaSquare and is now dead.");
            }
            
            if (collision.gameObject.CompareTag("Dragon"))
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                    UpdateDragonCount();
                    Debug.Log("Player1 has been deactivated.");
                }
                else
                {
                    Debug.Log("Player1 could not be found.");
                }

            }
        }

        void UpdateDragonCount()
        {
            GameState gameState = FindObjectOfType<GameState>();
            if (gameState != null)
            {
                gameState.dragonCount++;
            }
            else
            {
                Debug.LogError("GameState not found in the scene!");
            }
        }

        public static PlayerCharacterOne GetNearest(Vector3 pos, float range = 99999f, bool alive_only = false)
        {
            PlayerCharacterOne nearest = null;
            float min_dist = range;
            foreach (PlayerCharacterOne character in GetAll())
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

        public static PlayerCharacterOne Get(int player_id)
        {
            if (character_list.TryGetValue(player_id, out PlayerCharacterOne character))
            {
                return character;
            }
            return null;
        }

        public static PlayerCharacterOne[] GetAll()
        {
            PlayerCharacterOne[] list = new PlayerCharacterOne[character_list.Count];
            character_list.Values.CopyTo(list, 0);
            return list;
        }
    }
}
