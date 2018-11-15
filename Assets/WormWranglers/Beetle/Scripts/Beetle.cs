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

        public float bumpAngle;
        public float bumpForceUp;
        public float bumpForceForward;

        // strangely physics is calculated BEFORE OnCollisionEnter so I need the value from last frame
        private float lastFrameVelocity;

        // input handlers
        private bool gamepad;
        private string horz;
        private string vert;
        private KeyCode left;
        private KeyCode right;
        private KeyCode accel;
        private KeyCode decel;

        // for communicating with BeetleManager to determine place
        private int index;
        private bool loser;
		
        private LerpFloat steer = new LerpFloat(0f, 0f, 0.5f, 2, -1f, 1f);

		private void Awake()
		{
            loser = false;
			AnimatedFloatManager.Add(this, steer, true);
        }

		private void FixedUpdate()
		{
            // Get player input (TODO: this is bad input management)

            float h = 0, v = 0;
            if (!gamepad)
            {
                steer.target = (Input.GetKey(right) ? 1 : 0) - (Input.GetKey(left) ? 1 : 0);
                h = steer;
                v = (Input.GetKey(accel) ? 1 : 0) - (Input.GetKey(decel) ? 1 : 0);
            }
            else
            {
                steer.target = Input.GetAxisRaw(horz);
                h = steer;
                v = Input.GetAxisRaw(vert);
            }

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

            // Store velocity
            lastFrameVelocity = body.velocity.magnitude;
		}

		// TODO: crap code, rewrite

		private void OnCollisionEnter(Collision collision)
		{
            if (collision.gameObject.CompareTag("Terrain") && !loser)
            {
                loser = true;
                FindObjectOfType<BeetleManager>().Loser(index);
            }
            if (collision.gameObject.CompareTag(gameObject.tag))
            {
                Bump(collision.gameObject);
            }
		}

        private void Bump(GameObject o)
        {
            // grab their rigidbody once
            Rigidbody enemy = o.GetComponent<Rigidbody>();
            // determine if we hit them head on
            Vector3 dir = transform.position - o.transform.position;
            float angle = Vector3.Angle(transform.forward, dir);

            // make sure we are the bumper and not the bumpee
            bool bumpCheck = lastFrameVelocity > o.GetComponent<Beetle>().lastFrameVelocity;

            if (bumpCheck &&
                Clamp0360(angle) >= 180 - bumpAngle && 
                Clamp0360(angle) <= 180 + bumpAngle)
            {
                // a head on hit, shoot them into the air
                enemy.AddForce(Vector3.up * bumpForceUp, ForceMode.Impulse);
                enemy.AddForce((body.velocity - enemy.velocity) * bumpForceForward, ForceMode.Impulse);
            }
        }

        private float Clamp0360(float eulerAngles)
        {
            float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
            if (result < 0)
            {
                result += 360f;
            }
            return result;
        }

        public void AssignIndex(int x)
        {
            index = x;
        }

        public void AssignControls(string x, string y)
        {
            gamepad = true;

            horz = x;
            vert = y;
        }

        public void AssignControls(KeyCode l, KeyCode r, KeyCode a, KeyCode d)
        {
            gamepad = false;

            left = l;
            right = r;
            accel = a;
            decel = d;
        }
	}
}