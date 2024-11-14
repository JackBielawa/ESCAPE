using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.Platformer
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("Camera Target")]
        public GameObject playerOne;
        public GameObject playerTwo;
        public Vector3 target_offset;
        public float camera_speed = 5f;

        [Header("Level Limits")]
        public float level_bottom;
        public float level_left;
        public float level_right;

        private Camera cam;
        private Vector3 cur_pos;
        private GameObject lock_target = null;

        private Vector3 shake_vector = Vector3.zero;
        private float shake_timer = 0f;
        private float shake_intensity = 1f;

        private static FollowCamera _instance;

        void Awake()
        {
            _instance = this;
            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            if (playerOne == null || playerTwo == null)
                return;

            // Calculate the midpoint between the two players
            Vector3 midpoint = (playerOne.transform.position + playerTwo.transform.position) / 2 + target_offset;

            // Set level limits
            float fh = GetFrustrumHeight() / 2f;
            float fw = GetFrustrumWidth() / 2f;
            midpoint.x = Mathf.Clamp(midpoint.x, level_left + fw, level_right - fw);
            midpoint.y = Mathf.Max(level_bottom + fh, midpoint.y);

            // Move the camera smoothly towards the midpoint
            Vector3 diff = midpoint - transform.position;
            if (diff.magnitude > 0.1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, midpoint, ref cur_pos, 1f / camera_speed, Mathf.Infinity, Time.deltaTime);
            }

            // Adjust camera zoom based on the distance between the players
            float playerDistance = Vector3.Distance(playerOne.transform.position, playerTwo.transform.position);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, Mathf.Max(5f, playerDistance / 2f), Time.deltaTime * camera_speed);

            // Shake effect
            if (shake_timer > 0f)
            {
                shake_timer -= Time.deltaTime;
                shake_vector = new Vector3(Mathf.Cos(shake_timer * Mathf.PI * 8f) * 0.02f, Mathf.Sin(shake_timer * Mathf.PI * 7f) * 0.02f, 0f);
                transform.position += shake_vector * shake_intensity;
            }
        }

        public float GetFrustrumHeight()
        {
            if (cam.orthographic)
                return 2f * cam.orthographicSize;
            else
                return 2.0f * Mathf.Abs(transform.position.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        public float GetFrustrumWidth()
        {
            return GetFrustrumHeight() * cam.aspect;
        }

        public void LockCameraOn(GameObject ltarget)
        {
            lock_target = ltarget;
        }

        public void UnlockCamera()
        {
            lock_target = null;
        }

        public void Shake(float intensity = 2f, float duration = 0.5f)
        {
            shake_intensity = intensity;
            shake_timer = duration;
        }

        public static FollowCamera Get()
        {
            return _instance;
        }

        public static Camera GetCamera()
        {
            if (_instance)
                return _instance.cam;
            return null;
        }
    }
}
