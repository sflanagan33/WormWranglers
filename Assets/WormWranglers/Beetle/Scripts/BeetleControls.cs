using UnityEngine;

namespace WormWranglers.Beetle
{
    public struct BeetleControls
    {
        public bool useGamepad;

        public string hor;
        public string ver;
        public KeyCode left;
        public KeyCode right;
        public KeyCode up;
        public KeyCode down;

        public BeetleControls(string hor, string ver)
        {
            this.useGamepad = true;

            this.hor = hor;
            this.ver = ver;
            this.left = KeyCode.None;
            this.right = KeyCode.None;
            this.up = KeyCode.None;
            this.down = KeyCode.None;
        }

        public BeetleControls(KeyCode left, KeyCode right, KeyCode up, KeyCode down)
        {
            this.useGamepad = false;

            this.hor = null;
            this.ver = null;
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }

        public override string ToString()
        {
            if (useGamepad)
                return "Gamepad Controls";
            return string.Format("{0} < > {1}\n{2} | {3}", left, right, up, down);
        }
    }
}