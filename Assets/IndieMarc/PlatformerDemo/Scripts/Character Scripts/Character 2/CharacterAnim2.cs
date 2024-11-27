using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(PlayerCharacterTwo))] // Ensure this is PlayerCharacterTwo
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimTwo : MonoBehaviour
    {
        private PlayerCharacterTwo character;
        private CharacterHoldItemTwo character_item;
        private Animator animator;

        void Awake()
        {
            character = GetComponent<PlayerCharacterTwo>();
            character_item = GetComponent<CharacterHoldItemTwo>();
            animator = GetComponent<Animator>();

            character.onJump += OnJump;
            character.onCrouch += OnCrouch;
            character.onDeath += OnDeath;
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

            // Animations
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
