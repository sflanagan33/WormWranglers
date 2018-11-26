using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Core;
using WormWranglers.Util;

namespace WormWranglers.Beetle
{
	public class BeetleVisuals : MonoBehaviour
	{
        [SerializeField] private BeetleData data;   // contains the game's beetle models and palettes
		
		[SerializeField] private Rigidbody body;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
		[SerializeField] private Transform wheelFrontLeft;
		[SerializeField] private Transform wheelFrontRight;
		[SerializeField] private Transform wheelBackLeft;
		[SerializeField] private Transform wheelBackRight;

		private JiggleFloat turn = new JiggleFloat(0f, 0f, 0.4f, 0.3f);
		private JiggleFloat scale = new JiggleFloat(0f, 0f, 0.15f, 0.2f);

		private Vector3 velocityPrev;
        private int index;

        private void Start()
		{
			AnimatedFloatManager.Add(this, turn, true);
			AnimatedFloatManager.Add(this, scale, true);
		}

		private void Update()
		{
            // Wheel turning

            BeetleControls controls = Game.BEETLE_CONTROLS[index];

            if (controls.useGamepad)
                turn.target = Input.GetAxis(controls.hor);
            
            else
                turn.target = (Input.GetKey(controls.right) ? 1 : 0)
                            - (Input.GetKey(controls.left) ? 1 : 0);
            
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

		public void AssignIndex(int index)
        {
            this.index = index;

            BeetlePalette palette = data.palettes[Game.BEETLE_PALETTE_CHOICE[index]];
            meshRenderer.materials[0].SetColor("_Color", palette.shadow);
            meshRenderer.materials[1].SetColor("_Color", palette.main);
            meshRenderer.materials[2].SetColor("_Color", palette.highlight);

            // Painting the wheels

            transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", palette.shadow);
            transform.GetChild(2).GetComponent<MeshRenderer>().material.SetColor("_Color", palette.shadow);
            transform.GetChild(3).GetComponent<MeshRenderer>().material.SetColor("_Color", palette.shadow);
            transform.GetChild(4).GetComponent<MeshRenderer>().material.SetColor("_Color", palette.shadow);
        }

        public void SetBody(Rigidbody rb) { body = rb; }

    }
}