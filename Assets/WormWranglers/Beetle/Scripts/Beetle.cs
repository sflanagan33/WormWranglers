using UnityEngine;

namespace WormWranglers.Beetle
{
    public class Beetle : MonoBehaviour
    {
        public Camera Camera { get { return cam.Camera; } }

        [SerializeField] private BeetleMovement movement;
        [SerializeField] private BeetleVisuals visuals;
        [SerializeField] private BeetleCam cam;
        [SerializeField] private BeetleData data;

        public void AssignIndex(int index)
        {
            movement.AssignIndex(index);

            // replace visuals with the one stored in data
            GameObject old = transform.Find("Movement").Find("Visuals").gameObject;
            GameObject v = Instantiate(data.visuals[Core.Game.BEETLE_MODEL_CHOICE[index]]);

            // place them in the same position
            v.transform.SetPositionAndRotation(old.transform.position, old.transform.rotation);

            // destroy the old one
            Destroy(old);

            // parent it
            v.name = "Visuals";
            v.transform.SetParent(transform.Find("Movement"));

            // tweak it to fit
            visuals = v.GetComponent<BeetleVisuals>();
            visuals.SetBody(transform.Find("Movement").GetComponent<Rigidbody>());
            visuals.AssignIndex(index);
        }
    }
}