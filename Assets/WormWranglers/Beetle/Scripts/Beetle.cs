using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		private void FixedUpdate()
		{
			// Get player input

			float h = Input.GetAxisRaw("Horizontal");
			float v = (Input.GetAxisRaw("Gas") + 1) / 2f;
			v -= (Input.GetAxisRaw("Reverse") + 1) / 2f;

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
	}
}