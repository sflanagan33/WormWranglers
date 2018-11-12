using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Util;

namespace WormWranglers.Beetle
{
	public class Beetle : MonoBehaviour
	{
		[SerializeField] private Rigidbody body;

		public float turnForce;
		public float turnDrag;

		public float rightingForce;

		public float thrustForce;
		public float thrustForwardDrag;
		public float thrustLateralDrag;
		
        private LerpFloat steer = new LerpFloat(0f, 0f, 0.5f, 2, -1f, 1f);

		private void Awake()
		{
			AnimatedFloatManager.Add(this, steer, true);
		}

		private void FixedUpdate()
		{
			// Get player input (TODO: this is bad input management)

			//float h = Input.GetAxisRaw("Horizontal");
			//float v = (Input.GetAxisRaw("Gas") + 1) / 2f;
			//v -= (Input.GetAxisRaw("Reverse") + 1) / 2f;

            steer.target = (Input.GetKey(KeyCode.D) ? 1 : 0)
						 - (Input.GetKey(KeyCode.A) ? 1 : 0);
			float h = steer;
			float v = (Input.GetKey(KeyCode.Space) ? 1 : 0)
					- (Input.GetKey(KeyCode.LeftShift) ? 1 : 0);

			// ====================================================================================

			// Apply turn with horizontal input and based on the current velocity

			float turn = Vector3.Dot(body.velocity, transform.forward) * h * turnForce;
			body.AddRelativeTorque(0, turn, 0, ForceMode.Acceleration);

			// Apply torque to keep kart upright

			Quaternion r = Quaternion.FromToRotation(transform.up, Vector3.up);
			Vector3 righting = new Vector3(r.x, r.y, r.z) * rightingForce;
			body.AddTorque(righting, ForceMode.Acceleration);

			// Apply angular drag to yaw
			
			body.angularVelocity *= turnDrag;

			// ====================================================================================

			// Apply thrust with vertical input

			Vector3 thrust = transform.forward * v * thrustForce;
			body.AddForce(thrust, ForceMode.Acceleration);

			// Apply drag (separated orthogonally)
			
			Vector3 vF = Vector3.Project(body.velocity, transform.forward);
			Vector3 vL = Vector3.Project(body.velocity, transform.right);
			Vector3 vV = body.velocity - vF - vL;

			body.velocity = (vF * thrustForwardDrag) + (vL * thrustLateralDrag) + vV;
			
			// ====================================================================================
		}

		// TODO: crap code, rewrite

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("Terrain"))
				FindObjectOfType<Game>().End(Player.Worm);
		}
	}
}