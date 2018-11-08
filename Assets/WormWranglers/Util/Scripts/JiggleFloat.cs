namespace WormWranglers.Util
{
    public class JiggleFloat : AnimatedFloat
    {
        public float accelerationFactor;
        public float frictionFactor;

        public float velocity;
        public float clampBounce;

        private bool useMinimum;
        private bool useMaximum;

        public JiggleFloat(float value, float target,
                           float accelerationFactor, float frictionFactor,
                           float? minimum = null, float? maximum = null, float clampBounce = 0)
        {
            this.val = value;
            this.target = target;
            this.accelerationFactor = accelerationFactor;
            this.frictionFactor = frictionFactor;

            if (minimum.HasValue)
            {
                this.minimum = (float) minimum;
                useMinimum = true;
            }

            if (maximum.HasValue)
            {
                this.maximum = (float) maximum;
                useMaximum = true;
            }

            this.clampBounce = clampBounce;
        }

        protected override void InternalUpdate(float deltaTime)
        {
            velocity += (target - val) * accelerationFactor * deltaTime;
            velocity *= 1 - (frictionFactor * deltaTime);
            val += velocity * deltaTime;
        }

        protected override void InternalClamp()
        {
            if (useMinimum && val < minimum || useMaximum && val > maximum)
                velocity *= -clampBounce;

            base.InternalClamp();
        }
    }
}