using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Core;
using WormWranglers.Util;

namespace WormWranglers.Beetle
{
	public class BeetleMovement : MonoBehaviour
	{
		[SerializeField] private Rigidbody body;

        // Physics variables

		[SerializeField] private float turnForce;
		[SerializeField] private float turnDrag;

		[SerializeField] private float rightingForce;

		[SerializeField] private float thrustForce;
		[SerializeField] private float thrustForwardDrag;
		[SerializeField] private float thrustLateralDrag;

        [SerializeField] private float bumpAngle;
        [SerializeField] private float bumpForceUp;
        [SerializeField] private float bumpForceForward;
        
        private float speedPrev;

        // Identity / state variables

        private int index;
        private bool crashed;

        // ====================================================================================================================

        private RaceManager manager;
        private LerpFloat keycodeSteering = new LerpFloat(0f, 0f, 0.5f, 2, -1f, 1f);

		private void Awake()
		{
            manager = FindObjectOfType<RaceManager>();
			AnimatedFloatManager.Add(this, keycodeSteering, true);
        }

        public void Initialize(int index)
        {
            this.index = index;
        }
        
        // ====================================================================================================================

        private void FixedUpdate()
        { 
            // Read input (using the controls for this beetle's index)

            float h = 0;
            float v = 0;

            if (manager.AllowInput)
            {
                BeetleControls controls = Game.BEETLE_CONTROLS[index];

                if (controls.useGamepad)
                {
                    h = Input.GetAxisRaw(controls.hor);
                    v = Input.GetAxisRaw(controls.ver);
                }

                else
                {
                    keycodeSteering.target = (Input.GetKey(controls.right) ? 1f : 0f)
                                           - (Input.GetKey(controls.left) ? 1f : 0f);

                    h = keycodeSteering;
                    v = (Input.GetKey(controls.up) ? 1f : 0f)
                      - (Input.GetKey(controls.down) ? 1f : 0f);
                }
            }

			// ====================================================================================

			// Apply turn with horizontal input and based on the current velocity

			float turn = Vector3.Dot(body.velocity, transform.forward) * h * turnForce;
			body.AddRelativeTorque(0, turn, 0, ForceMode.Acceleration);

			// Apply torque to keep kart upright

			Quaternion r = Quaternion.FromToRotation(transform.up, Vector3.up);
			Vector3 righting = new Vector3(r.x, r.y, r.z) * rightingForce;
			body.AddTorque(righting, ForceMode.Acceleration);

			// Apply angular drag
			
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

            // Store speed

            speedPrev = body.velocity.magnitude;
		}

        // ====================================================================================================================

		private void OnCollisionEnter(Collision collision)
		{
            if (!crashed && collision.gameObject.CompareTag("Terrain"))
            {
                crashed = true;
                FindObjectOfType<RaceManager>().BeetleCrashed(index);
            }

            if (collision.gameObject.CompareTag(gameObject.tag))
            {
                Bump(collision.gameObject);
            }
		}

        private void Bump(GameObject o)
        {
            // Grab their rigidbody
            Rigidbody enemy = o.GetComponent<Rigidbody>();

            // Determine if we hit them head on
            Vector3 dir = transform.position - o.transform.position;
            float angle = Vector3.Angle(transform.forward, dir);

            // Make sure we are the bumper and not the bumpee
            bool bumpCheck = speedPrev > o.GetComponent<BeetleMovement>().speedPrev;

            if (bumpCheck
             && Clamp0360(angle) >= 180 - bumpAngle
             && Clamp0360(angle) <= 180 + bumpAngle)
            {
                enemy.AddForce(Vector3.up * bumpForceUp, ForceMode.Impulse);
                enemy.AddForce((body.velocity - enemy.velocity) * bumpForceForward, ForceMode.Impulse);
            }
        }

        private float Clamp0360(float eulerAngles)
        {
            float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
            if (result < 0)
                result += 360f;
            
            return result;
        }
	}
}