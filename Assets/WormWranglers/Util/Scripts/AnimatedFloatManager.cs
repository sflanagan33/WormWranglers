using UnityEngine;
using System.Collections.Generic;

namespace WormWranglers.Util
{
    public class AnimatedFloatManager : MonoBehaviour
    {
        public static readonly float FIXED_STEP = 1 / 60f;
        public static float LAST_FIXED_STEP;

        public static void Add(MonoBehaviour owner, AnimatedFloat f, bool useScaledTime)
        {
            f.owner = owner;

            if (useScaledTime)
                afm.scaledFloats.Add(f);
            else
                afm.unscaledFloats.Add(f);
        }

        private static AnimatedFloatManager afm
        {
            get
            {
                if (a == null)
                {
                    GameObject g = new GameObject("AnimatedFloatManager");
                    a = g.AddComponent<AnimatedFloatManager>();
                    DontDestroyOnLoad(g);
                }

                return a;
            }
        }

        private static AnimatedFloatManager a;

        // ====================================================================================================================

        private List<AnimatedFloat> scaledFloats = new List<AnimatedFloat>();
        private List<AnimatedFloat> unscaledFloats = new List<AnimatedFloat>();

        private float fixedTime;

        private void Update()
        {
            // Clear floats that no longer exist from the list

            scaledFloats.RemoveAll(f => f.owner == null || f == null);
            unscaledFloats.RemoveAll(f => f.owner == null || f == null);

            // Tick floats

            while (fixedTime < Time.unscaledTime)
            {
                fixedTime += FIXED_STEP;

                foreach (AnimatedFloat f in scaledFloats)
                    f.Tick(Time.timeScale);
                foreach (AnimatedFloat f in unscaledFloats)
                    f.Tick(1);
            }

            LAST_FIXED_STEP = fixedTime;
        }
    }
}