using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WormWranglers.Core
{
    public class CustomizeManager : MonoBehaviour
    {
        [SerializeField] private GameObject beetlePrefab;
        [SerializeField] private Camera wormCamera;

        private List<CustomizeBeetle> beetles = new List<CustomizeBeetle>();
        private bool isDone;

        private void Awake()
        {
            // Create customizable beetles for each beetle player, give them their indices, and put them in a list

            for (int i = 0; i < Game.BEETLE_COUNT; i++)
            {
                Vector3 p = new Vector3(i * 100, 0, 0);
                GameObject g = Instantiate(beetlePrefab, p, Quaternion.identity);

                CustomizeBeetle b = g.GetComponent<CustomizeBeetle>();
                b.Initialize(i);
                beetles.Add(b);
            }

            // Arrange the cameras (for the customizable beetles / worm) on the screen

            var beetleCameras = beetles.Select(b => b.Camera).ToList();
            Game.ArrangeCameras(beetleCameras, wormCamera);

            // Play the start music in case we're coming from the results scene

            MusicManager.Play(MusicTrack.Start);
        }

        private void Update()
        {
            // If all the beetles are ready, transition to the race scene

            if (!isDone && beetles.All(b => b.IsReady))
            {
                isDone = true;

                foreach (CustomizeBeetle b in beetles)
                    b.BlockInput();

                MusicManager.Play(MusicTrack.None);
                TransitionManager.ToScene(2);
            }
        }
    }
}