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

        [SerializeField] private BeetleData data;   // contains the game's beetle models and palettes

        [SerializeField] private Camera cam;
        [SerializeField] private Transform model;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Text messageText;
        [SerializeField] private Text controlsText;

        private enum State { ModelSelect, PaletteSelect, Ready }
        private State state;
        private int index;
        private bool acceptInput = true;

        private JiggleFloat modelScale = new JiggleFloat(0f, 0f, 0.25f, 0.2f, -0.9f, 0.9f);
        private JiggleFloat messageX = new JiggleFloat(0f, 0f, 0.7f, 0.4f);
        private JiggleFloat messageY = new JiggleFloat(0f, 0f, 0.7f, 0.4f);

        private void Start()
        {
            AnimatedFloatManager.Add(this, modelScale, true);
            AnimatedFloatManager.Add(this, messageX, true);
            AnimatedFloatManager.Add(this, messageY, true);

            // Populate the controls text

            controlsText.text = Game.BEETLE_CONTROLS[index].ToString();
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

                if (controls.useGamepad) // TODO: still needs to act like getkeydown
                {
                    left  = Input.GetAxis(controls.hor) < -0.5f;
                    right = Input.GetAxis(controls.hor) > +0.5f;
                    up    = Input.GetAxis(controls.ver) < -0.5f;
                    down  = Input.GetAxis(controls.ver) > +0.5f;
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

                        messageX.velocity += add * 10f;
                        modelScale.velocity -= Mathf.Abs(add) * 0.1f;
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
                        
                        messageX.velocity += add * 10f;
                        modelScale.velocity -= Mathf.Abs(add) * 0.1f;
                    }
                    
                    break;

                case State.Ready:

                    messageText.text = "READY!";

                    if (up)
                        state = State.PaletteSelect;
                    break;
            }

            messageY.velocity += (up ? 5f : 0f) - (down ? 5f : 0f);

			// ====================================================================================

            // Update the model's appearance based on the current choices

            meshFilter.sharedMesh = data.models[Game.BEETLE_MODEL_CHOICE[index]];
            BeetlePalette palette = data.palettes[Game.BEETLE_PALETTE_CHOICE[index]];
            meshRenderer.materials[0].SetColor("_Color", palette.shadow);
            meshRenderer.materials[1].SetColor("_Color", palette.main);
            meshRenderer.materials[2].SetColor("_Color", palette.highlight);

            // Rotate and scale the model

            model.rotation = Quaternion.Euler(0, index * 120f + Time.time * 20f, 0);
            model.localScale = new Vector3(1 - modelScale, 1 + modelScale, 1 - modelScale);

            // Move the message text

            messageText.rectTransform.anchoredPosition = new Vector2(messageX, messageY - 40);
        }

        public void AssignIndex(int index)
        {
            this.index = index;
        }

        public void BlockInput()
        {
            acceptInput = false;
        }
    }
}