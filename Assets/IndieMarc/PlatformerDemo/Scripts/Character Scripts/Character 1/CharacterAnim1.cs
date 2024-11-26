using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(PlayerCharacterOne))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnim : MonoBehaviour
    {
        private PlayerCharacterOne character;
        private CharacterHoldItem character_item;
        private Animator animator;

        void Awake()
        {
            character = GetComponent<PlayerCharacterOne>();
            character_item = GetComponent<CharacterHoldItem>();
            animator = GetComponent<Animator>();

            character.onJump += OnJump;
            character.onCrouch += OnCrouch;
            character.onDeath += OnDeath; // Added death handling
        }

        void Update()
        {
            if (character.IsDead())
            {
                animator.SetBool("Dead", true);
                return;
            }
            else
            {
                animator.SetBool("Dead", false);
            }

            // Anims
            animator.SetBool("Jumping", character.IsJumping());
            animator.SetBool("InAir", !character.IsGrounded());
            animator.SetBool("Crouching", character.IsCrouching());
            animator.SetFloat("Speed", Mathf.Abs(character.GetMove().x));

            if (character_item != null)
                animator.SetBool("Hold", character_item.GetHeldItem() != null);
        }

        void OnCrouch()
        {
            animator.SetTrigger("Crouch");
        }

        void OnJump()
        {
            animator.SetTrigger("Jump");
        }

        void OnDeath()
        {
            animator.SetTrigger("Death");
        }
    }
}
