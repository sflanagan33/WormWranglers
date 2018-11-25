using UnityEngine;

namespace WormWranglers.Beetle
{
    public class Beetle : MonoBehaviour
    {
        public Camera Camera { get { return cam.Camera; } }

        [SerializeField] private BeetleMovement movement;
        [SerializeField] private BeetleVisuals visuals;
        [SerializeField] private BeetleCam cam;

        public void AssignIndex(int index)
        {
            movement.AssignIndex(index);
            visuals.AssignIndex(index);
        }
    }
}