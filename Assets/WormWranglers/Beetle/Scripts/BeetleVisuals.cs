using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Util;

namespace WormWranglers.Beetle
{
	public class BeetleVisuals : MonoBehaviour
	{
		[SerializeField] private Rigidbody body;
		[SerializeField] private Transform wheelFrontLeft;
		[SerializeField] private Transform wheelFrontRight;
		[SerializeField] private Transform wheelBackLeft;
		[SerializeField] private Transform wheelBackRight;

		private JiggleFloat turn = new JiggleFloat(0f, 0f, 0.4f, 0.3f);
		private JiggleFloat scale = new JiggleFloat(0f, 0f, 0.15f, 0.2f);

		private Vector3 velocityPrev;

        private KeyCode left;
        private KeyCode right;


        private void Start()
		{
			AnimatedFloatManager.Add(this, turn, true);
			AnimatedFloatManager.Add(this, scale, true);
		}

		private void Update()
		{
			// Wheel turning

			turn.target = (Input.GetKey(left) ? 1 : 0)
                        - (Input.GetKey(right) ? 1 : 0);
            Quaternion turnRot = Quaternion.Euler(0, 90 + turn * 15, 0);
			wheelFrontLeft.localRotation = wheelFrontRight.localRotation = turnRot;

			// Velocity jiggle

			Vector3 diff = velocityPrev - body.velocity;
			velocityPrev = body.velocity;

			scale.velocity += diff.y * 0.0125f;
			scale.velocity -= Mathf.Abs(diff.x) * 0.0025f;
			scale.velocity -= Mathf.Abs(diff.z) * 0.0025f;

			transform.localScale = new Vector3(1 - scale, 1 + scale, 1 - scale);
		}

        public void AssignControls(KeyCode l, KeyCode r)
        {
            left = l;
            right = r;
        }
    }
}