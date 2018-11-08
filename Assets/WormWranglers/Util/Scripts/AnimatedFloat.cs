using UnityEngine;
using System.Collections;

namespace WormWranglers.Util
{
    public abstract class AnimatedFloat
    {
        // All animated floats have a current value, a current target,
        // and an optional minimum and maximum that clamp them both.

        protected float val;
        protected float lastVal;

        public virtual float value
        {
            get
            {
                float t = (Time.unscaledTime - AnimatedFloatManager.LAST_FIXED_STEP) / AnimatedFloatManager.FIXED_STEP;
                return Mathf.Lerp(lastVal, val, Mathf.Clamp01(t));
            }

            set
            {
                val = value;
                InternalClamp();
                lastVal = val;
            }
        }

        public virtual float fixedValue
        {
            get
            {
                float t = (Time.unscaledTime - Time.fixedUnscaledTime) / Time.fixedUnscaledDeltaTime;
                return Mathf.Lerp(lastVal, val, Mathf.Clamp01(t));
            }
        }

        public float target;
        public float? minimum;
        public float? maximum;

        public MonoBehaviour owner;

        // In order to make an animated float "run", the object that is using it
        // must call this Update method once per tick, providing the appropriate
        // timestep to run the animation over. This updates the value using the
        // derived animated float's behavior and then clamps the result.

        public void Tick(float scale)
        {
            lastVal = val;
            InternalUpdate(scale);
            InternalClamp();
        }

        // All animated floats must define an InternalUpdate method that in
        // some way makes the current value approximate the current target.

        protected abstract void InternalUpdate(float scale);

        // Helper method that clamps the current value and current target to the
        // minimum and maximum, if they are defined. Used after the InternalUpdate.

        protected virtual void InternalClamp()
        {
            if (minimum != null)
            {
                if (val < minimum)
                    val = (float) minimum;
                if (target < minimum)
                    target = (float) minimum;
            }

            if (maximum != null)
            {
                if (val > maximum)
                    val = (float) maximum;
                if (target > maximum)
                    target = (float) maximum;
            }
        }

        // You can retrieve the current value of an animated float implicitly.

        public static implicit operator float(AnimatedFloat f)
        {
            return f.val;
        }
    }
}