using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(PlayerCharacterTwo))] // Reference PlayerCharacterTwo
    public class CharacterHoldItemTwo : MonoBehaviour
    {
        public Transform hand;

        private PlayerCharacterTwo character; // Update to PlayerCharacterTwo
        private CarryItem held_item = null;
        private float take_item_timer = 0f;

        void Awake()
        {
            character = GetComponent<PlayerCharacterTwo>(); // Update to PlayerCharacterTwo
        }

        private void Start()
        {
            character.onDeath += DropItem;
        }

        void Update()
        {
            PlayerControlsTwo controls = PlayerControlsTwo.Get(character.player_id);

            take_item_timer += Time.deltaTime;
            if (held_item && controls.GetActionDown())
                held_item.UseItem();
        }

        private void LateUpdate()
        {
            if (held_item != null)
                held_item.UpdateCarryItem();
        }

        public void TakeItem(CarryItem item)
        {
            if (item == held_item || take_item_timer < 0f)
                return;

            if (held_item != null)
                DropItem();

            held_item = item;
            take_item_timer = -0.2f;
            item.Take(this);
        }

        public void DropItem()
        {
            if (held_item != null)
            {
                held_item.Drop();
                held_item = null;

                // Debugging
                Debug.Log($"[CharacterHoldItemTwo] Player {character.player_id} dropped the item.");
            }
        }

        public PlayerCharacterTwo GetCharacter()
        {
            return character;
        }

        public CarryItem GetHeldItem()
        {
            return held_item;
        }

        public Vector3 GetHandPos()
        {
            if (hand)
                return hand.transform.position;
            return transform.position;
        }

        public Vector2 GetMove()
        {
            return character.GetMove();
        }

        public Vector2 GetFacing()
        {
            return character.GetFacing();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<CarryItem>())
                TakeItem(collision.GetComponent<CarryItem>());
        }
    }
}
