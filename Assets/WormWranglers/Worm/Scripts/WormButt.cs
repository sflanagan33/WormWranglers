using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Worm
{
    public class WormButt : MonoBehaviour
    {
        [SerializeField] private Worm worm;

        private void LateUpdate()
        {
            transform.position = worm.GetButtPosition();
            transform.rotation = worm.GetButtRotation();
        }
    }
}
