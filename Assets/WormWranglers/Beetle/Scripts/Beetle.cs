using UnityEngine;

namespace WormWranglers.Beetle
{
    public class Beetle : MonoBehaviour
    {
        public Camera Camera { get { return cam.Camera; } }

        [SerializeField] private GameObject visualsPrefab;

        [SerializeField] private BeetleMovement movement;
        [SerializeField] private BeetleCam cam;

        public void Initialize(int index)
        {
            movement.Initialize(index);

            // Spawn the visuals
            
            var visuals = Instantiate(visualsPrefab, movement.transform).GetComponent<BeetleVisuals>();
            visuals.Initialize(index, GetComponentInChildren<Rigidbody>());
        }
    }
}