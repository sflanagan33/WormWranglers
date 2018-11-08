using UnityEngine;

namespace WormWranglers.Util
{
    public class LerpFloat : AnimatedFloat
    {
        public override float value
        {
            get
            {
                float t = (Time.time - AnimatedFloatManager.LAST_FIXED_STEP) / AnimatedFloatManager.FIXED_STEP;
                return Mathf.Lerp(lastVal, val, Mathf.Clamp01(t));
            }

            set
            {
                val = value;
                for (int i = 0; i < lerpOrder; i++)
                    lerpHelpers[i] = value;
                InternalClamp();
                lastVal = val;
            }
        }

        public float lerpFactor;

        private int lerpOrder;
        private float[] lerpHelpers;

        public LerpFloat(float value, float target,
                         float lerpFactor, int lerpOrder = 1,
                         float? minimum = null, float? maximum = null)
        {
            this.val = value;
            this.target = target;
            this.lerpFactor = lerpFactor;
            this.lerpOrder = Mathf.Max(1, lerpOrder);
            this.minimum = minimum;
            this.maximum = maximum;

            lerpHelpers = new float[this.lerpOrder];
        }

        protected override void InternalUpdate(float deltaTime)
        {
            lerpHelpers[0] = Mathf.Lerp(lerpHelpers[0], target, lerpFactor * deltaTime);

            for (int i = 1; i < lerpOrder; i++)
                lerpHelpers[i] = Mathf.Lerp(lerpHelpers[i], lerpHelpers[i - 1], lerpFactor * deltaTime);

            val = lerpHelpers[lerpOrder - 1];
        }
    }
}