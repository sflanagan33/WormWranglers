using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Core;
using WormWranglers.Util;

namespace WormWranglers.Beetle
{
	public class BeetleVisuals : MonoBehaviour
	{
        [SerializeField] private BeetleData data;               // the game's beetle models and palettes
		
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
		[SerializeField] private Transform wheelFrontLeft;
		[SerializeField] private Transform wheelFrontRight;

        private int index;
		private Rigidbody body;
		private Vector3 velocityPrev;

		private JiggleFloat turn = new JiggleFloat(0f, 0f, 0.4f, 0.3f);
		private JiggleFloat scale = new JiggleFloat(0f, 0f, 0.15f, 0.2f);

        private void Start()
		{
			AnimatedFloatManager.Add(this, turn, true);
			AnimatedFloatManager.Add(this, scale, true);
		}

		public void Initialize(int index, Rigidbody body)
        {
            this.index = index;
            this.body = body;

            UpdateAppearance();
        }

		private void Update()
		{
            if (body)
            {
                // Wheel turning

                BeetleControls controls = Game.BEETLE_CONTROLS[index];

                if (controls.useGamepad)
                    turn.target = Input.GetAxis(controls.hor);
                
                else
                    turn.target = (Input.GetKey(controls.right) ? 1 : 0)
                                - (Input.GetKey(controls.left) ? 1 : 0);
                
                Quaternion turnRot = Quaternion.Euler(0, turn * 15, 0);
                wheelFrontLeft.localRotation = wheelFrontRight.localRotation = turnRot;

			    // Velocity jiggle

                Vector3 diff = velocityPrev - body.velocity;
                velocityPrev = body.velocity;

                scale.velocity += diff.y * 0.0125f;
                scale.velocity -= Mathf.Abs(diff.x) * 0.0025f;
                scale.velocity -= Mathf.Abs(diff.z) * 0.0025f;
            }

			transform.localScale = new Vector3(1 - scale, 1 + scale, 1 - scale);
		}

        public void UpdateAppearance()
        {
            meshFilter.sharedMesh = data.models[Game.BEETLE_MODEL_CHOICE[index]];

            BeetlePalette palette = data.palettes[Game.BEETLE_PALETTE_CHOICE[index]];
            meshRenderer.materials[0].SetColor("_Color", palette.main);
            meshRenderer.materials[1].SetColor("_Color", palette.shadow);
            meshRenderer.materials[2].SetColor("_Color", palette.highlight);
        }

        public void Jiggle(float velocity)
        {
            scale.velocity += velocity;
        }
    }
}