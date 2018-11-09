﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Util;

namespace WormWranglers.Beetle
{
	public class BeetleCam : MonoBehaviour
	{
		[SerializeField] private Rigidbody follow;
		[SerializeField] private Camera cam;

		private LerpFloat FOV = new LerpFloat(0f, 0f, 0.1f, 2);
		public float FOVBase;
		public float FOVExpand;

		private void Start()
		{
			AnimatedFloatManager.Add(this, FOV, true);
			FOV.value = FOV.target = FOVBase;
		}

		private void Update()
		{
			// Position

			Vector3 goalPos = follow.transform.position;
			transform.position = Vector3.Lerp(transform.position, goalPos, 0.3f);

			// Rotation
			
			Vector3 goalForward = Vector3.ProjectOnPlane(follow.transform.forward, Vector3.up);
			Quaternion goalRot = Quaternion.LookRotation(goalForward, Vector3.up);
			goalRot = Quaternion.Slerp(goalRot, follow.transform.rotation, 0.25f);

			transform.rotation = Quaternion.Slerp(transform.rotation, goalRot, 0.15f);

			// Camera FOV

			FOV.target = FOVBase + (follow.velocity.magnitude * FOVExpand);
			cam.fieldOfView = FOV;
		}
	}
}