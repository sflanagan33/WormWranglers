using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Util;

namespace WormWranglers.Core
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Transform cam;
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform title;

        private bool acceptInput = false;

        private JiggleFloat sx = new JiggleFloat(0f, 0f, 0.100f, 0.225f, 0f);
        private JiggleFloat sy = new JiggleFloat(0f, 0f, 0.075f, 0.200f, 0f);

        private IEnumerator Start()
        {
            AnimatedFloatManager.Add(this, sx, false);
            AnimatedFloatManager.Add(this, sy, false);
            yield return new WaitForSeconds(1.25f);

            sx.target = sy.target = 1f;
            MusicManager.Play(MusicTrack.Start);
            yield return new WaitForSeconds(1f);

            acceptInput = true;
        }

        private void Update()
        {
            // Make camera spin

            cam.rotation = Quaternion.Euler(0, Time.time * 5f, 0);

            // Make container scale
            
            container.localScale = new Vector3(sx, sy, 1f);

            // Make title wobble

            float y = 140f + Mathf.Sin(Time.time * 2f) * 2f;
            float z = Mathf.Sin(Time.time);
            title.anchoredPosition = new Vector2(0, y);
            title.localRotation = Quaternion.Euler(0, 0, z);
        }
        
        public void Play(int count)
        {
            if (acceptInput)
            {
                acceptInput = false;
                sx.target = sy.target = 0;

                Game.Initialize(count);
                TransitionManager.ToScene(1);
            }
        }
    }
}