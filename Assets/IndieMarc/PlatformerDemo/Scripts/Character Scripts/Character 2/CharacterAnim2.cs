using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(PlayerCharacterTwo))] // Ensure this is PlayerCharacterTwo, not PlayerCharacter
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimTwo : MonoBehaviour
    {
        private PlayerCharacterTwo character2; // Update to PlayerCharacterTwo
        private Animator animator2;
        private CharacterHoldItem character_item2;

        void Awake()
        {
            character2 = GetComponent<PlayerCharacterTwo>(); // Ensure this gets PlayerCharacterTwo
            animator2 = GetComponent<Animator>();

            character2.onJump += OnJump;
            character2.onCrouch += OnCrouch;
        }

        void Update()
        {
            // Animations
            animator2.SetBool("Jumping", character2.IsJumping());
            animator2.SetBool("InAir", !character2.IsGrounded());
            animator2.SetBool("Crouching", character2.IsCrouching());
            animator2.SetFloat("Speed", character2.GetMove().magnitude);
            if (character_item2 != null)
                animator2.SetBool("Hold", character_item2.GetHeldItem() != null);
        }

        void OnCrouch()
        {
            animator2.SetTrigger("Crouch");
        }

        void OnJump()
        {
            animator2.SetTrigger("Jump");
        }
    }
}
