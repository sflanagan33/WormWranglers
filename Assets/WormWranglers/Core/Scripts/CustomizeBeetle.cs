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
                    // flipping these
                    down    = Input.GetKeyDown(controls.up);
                    up  = Input.GetKeyDown(controls.down);
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
                        int len = data.visuals.Length;

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

            GameObject template = data.visuals[Game.BEETLE_MODEL_CHOICE[index]];

            Vector3 actualScale = template.transform.localScale;

            meshFilter.sharedMesh = template.GetComponent<MeshFilter>().sharedMesh;

            BeetlePalette palette = data.palettes[Game.BEETLE_PALETTE_CHOICE[index]];
            meshRenderer.materials[0].SetColor("_Color", palette.shadow);
            meshRenderer.materials[1].SetColor("_Color", palette.main);
            meshRenderer.materials[2].SetColor("_Color", palette.highlight);

            // Rotate and scale the model

            model.rotation = Quaternion.Euler(0, index * 120f + Time.time * 20f, 0);
            model.localScale = new Vector3(actualScale.x - modelScale, actualScale.y + modelScale, actualScale.z - modelScale);

            // delete the children
            for (int i = model.childCount - 1; i > -1; i--)
            {
                Destroy(model.GetChild(i).gameObject);
            }
            // add wheels from data
            for (int i = 1; i < template.transform.childCount; i++)
            {
                GameObject component = Instantiate(template.transform.GetChild(i).gameObject);
                component.transform.position = model.TransformPoint(component.transform.position);
                component.transform.rotation = Quaternion.Euler(component.transform.rotation.eulerAngles + model.transform.rotation.eulerAngles);
                component.transform.localScale = Vector3.Scale(component.transform.localScale, actualScale);

                component.GetComponent<MeshRenderer>().material.SetColor("_Color", palette.shadow);
                component.transform.SetParent(model.transform);
            }

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