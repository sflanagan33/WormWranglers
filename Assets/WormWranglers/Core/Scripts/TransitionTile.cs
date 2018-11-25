using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using WormWranglers.Util;

namespace WormWranglers.Core
{
	public class TransitionTile : MonoBehaviour
	{
		private LerpFloat s = new LerpFloat(1f, 1f, 0.25f, 2, 0f, 1f);

        private void Start()
        {
            AnimatedFloatManager.Add(this, s, false);
        }

        private void Update()
        {
            transform.localRotation = Quaternion.Euler(0, 0, s * 90);
            transform.localScale = s * 1.01f * Vector3.one;
        }

        public void Show(bool show)
        {
            s.target = show ? 1f : 0f;
        }
	}
}