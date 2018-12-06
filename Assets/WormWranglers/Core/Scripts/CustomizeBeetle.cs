using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using WormWranglers.Beetle;
using WormWranglers.Util;

namespace WormWranglers.Core
{
    public class CustomizeBeetle : MonoBehaviour
    {
        public bool IsReady { get { return state == State.Ready; } }
        public Camera Camera { get { return cam; } }

        [SerializeField] private BeetleData data;               // the game's beetle models and palettes
        [SerializeField] private GameObject visualsPrefab;      // the beetle visuals prefab

        [SerializeField] private Camera cam;
        [SerializeField] private Text messageText;
        [SerializeField] private Text controlsText;

        private enum State { ModelSelect, PaletteSelect, Ready }
        private State state;
        private int index;

        private BeetleVisuals visuals;

        private bool acceptInput = true;
        private float gamepadHorPrev;
        private float gamepadVerPrev;

        private JiggleFloat messageX = new JiggleFloat(0f, 0f, 0.7f, 0.4f);
        private JiggleFloat messageY = new JiggleFloat(0f, 0f, 0.7f, 0.4f);
        
        private void Start()
        {
            AnimatedFloatManager.Add(this, messageX, true);
            AnimatedFloatManager.Add(this, messageY, true);
        }

        public void Initialize(int index)
        {
            this.index = index;

            // Populate the controls text

            controlsText.text = Game.BEETLE_CONTROLS[index].ToString();

            // Spawn the visuals

            visuals = Instantiate(visualsPrefab, transform).GetComponent<BeetleVisuals>();
            visuals.Initialize(index, null);
        }

        private void Update()
        {
            // Read input

            bool left  = false;
            bool right = false;
            bool up    = false;
            bool down  = false;

            if (acceptInput)
            {
                BeetleControls controls = Game.BEETLE_CONTROLS[index];

                if (controls.useGamepad)
                {
                    float h = Input.GetAxisRaw(controls.hor);
                    float v = Input.GetAxisRaw(controls.ver);

                    left  = (h < -0.5f && h < gamepadHorPrev);
                    right = (h > +0.5f && h > gamepadHorPrev);
                    up    = (v < -0.5f && v < gamepadVerPrev);
                    down  = (v > +0.5f && v > gamepadVerPrev);

                    gamepadHorPrev = h;
                    gamepadVerPrev = v;
                }

                else
                {
                    left  = Input.GetKeyDown(controls.left);
                    right = Input.GetKeyDown(controls.right);
                    up    = Input.GetKeyDown(controls.up);
                    down  = Input.GetKeyDown(controls.down);
                }
            }
            
			// ====================================================================================

            // Allow customization

            switch (state)
            {
                case State.ModelSelect:

                    messageText.text = "CHOOSE A MODEL";

                    if (down)
                        state = State.PaletteSelect;
                    else
                    {
                        int cur = Game.BEETLE_MODEL_CHOICE[index];
                        int add = (right ? 1 : 0) - (left ? 1 : 0);
                        int len = data.models.Length;

                        Game.BEETLE_MODEL_CHOICE[index] = (cur + add + len) % len;

                        if (add != 0)
                        {
                            visuals.UpdateAppearance();
                            visuals.Jiggle(Mathf.Abs(add) * -0.05f);
                            
                            MusicManager.Play(Sound.Tick);
                            messageX.velocity += add * 10f;
                        }
                    }

                    break;

                case State.PaletteSelect:

                    messageText.text = "CHOOSE A PALETTE";

                    if (down)
                        state = State.Ready;
                    else if (up)
                        state = State.ModelSelect;
                    else
                    {
                        int cur = Game.BEETLE_PALETTE_CHOICE[index];
                        int add = (right ? 1 : 0) - (left ? 1 : 0);
                        int len = data.palettes.Length;
                        
                        Game.BEETLE_PALETTE_CHOICE[index] = (cur + add + len) % len;
                        
                        if (add != 0)
                        {
                            visuals.UpdateAppearance();
                            visuals.Jiggle(Mathf.Abs(add) * -0.05f);

                            MusicManager.Play(Sound.Tick);
                            messageX.velocity += add * 10f;
                        }
                    }
                    
                    break;

                case State.Ready:

                    messageText.text = "READY!";

                    if (up)
                        state = State.PaletteSelect;
                    break;
            }

            if (up != down)
            {
                MusicManager.Play(Sound.Select);
                messageY.velocity += (up ? 5f : 0f) - (down ? 5f : 0f);
            }

            // ====================================================================================

            visuals.transform.rotation = Quaternion.Euler(0, index * 120f + Time.time * 20f, 0);
            messageText.rectTransform.anchoredPosition = new Vector2(messageX, messageY - 25);
        }

        public void BlockInput()
        {
            acceptInput = false;
        }
    }
}