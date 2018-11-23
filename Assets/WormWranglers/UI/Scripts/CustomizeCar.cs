using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WormWranglers.Beetle;

public class CustomizeCar : MonoBehaviour {

    [HideInInspector]
    public int index;

    // All info to choose from

    public List<GameObject> models;
    public List<Material> baseColors; // colors are weird and I'm gonna make -1 store the prefab's default
    public List<Material> highlightColors;
    public List<Material> shadowColors;

    // Visual tweaks

    public Vector3 offsetFromCamera;
    public float rotationRate;
    public float bounceForce;
    public float bounceInterval;

    // Backing data

    private GameObject thisModel;
    private int modelIdx;
    private int colorIdx;
    private Material originalBase;
    private Material originalHighlight;
    private Material originalShadow;

    // Controls to use

    private bool gamepad;
    private string horz;
    private string vert;
    private KeyCode left = KeyCode.LeftArrow;
    private KeyCode right = KeyCode.RightArrow;
    private KeyCode submit = KeyCode.UpArrow;
    private KeyCode back = KeyCode.DownArrow;
    private bool inputBlocker;

    // Flow

    private enum State { BeetleSelect, ColorSelect, Done };
    private State state;

    // UI

    public Canvas canvas;


    private void Awake()
    {
        UpdateModel();
    }

    private void Start()
    {
        CreateCanvas();
    }

    private void Update()
    {
        thisModel.transform.Rotate(Vector3.up, rotationRate * Time.deltaTime);

        // poll for input, whose actions depend on state

        if (!inputBlocker)
        {
            if (state == State.BeetleSelect)
            {
                // message

                canvas.transform.Find("Message").GetComponent<Text>().text = "CHOOSE YOUR BEETLE";

                // input polling

                // TODO: Make gamepad axis control act like getkeydown where it only fires once

                if ((!gamepad && Input.GetKeyDown(left)) || 
                    (gamepad && Input.GetAxisRaw(horz) == -1))
                {
                    modelIdx = SlideM(modelIdx, -1);
                    UpdateModel();
                }
                else if ((!gamepad && Input.GetKeyDown(right)) ||
                    (gamepad && Input.GetAxisRaw(horz) == 1))
                {
                    modelIdx = SlideM(modelIdx, 1);
                    UpdateModel();
                }
                else if ((!gamepad && Input.GetKeyDown(submit)) ||
                    (gamepad && Input.GetAxisRaw(vert) == 1))
                {
                    state = State.ColorSelect;
                }
                else if ((!gamepad && Input.GetKeyDown(back)) ||
                    (gamepad && Input.GetAxisRaw(vert) == -1))
                {
                    // nothing
                }
            }
            else if (state == State.ColorSelect)
            {
                // message

                canvas.transform.Find("Message").GetComponent<Text>().text = "CHOOSE YOUR COLOR SCHEME";

                // input polling

                if ((!gamepad && Input.GetKeyDown(left)) ||
                    (gamepad && Input.GetAxisRaw(horz) == -1))
                {
                    colorIdx = SlideC(colorIdx, -1);
                    UpdateColors();
                }
                else if ((!gamepad && Input.GetKeyDown(right)) ||
                    (gamepad && Input.GetAxisRaw(horz) == 1))
                {
                    colorIdx = SlideC(colorIdx, 1);
                    UpdateColors();
                }
                else if ((!gamepad && Input.GetKeyDown(submit)) ||
                    (gamepad && Input.GetAxisRaw(vert) == 1))
                {
                    // send info to the manager

                    FinalizeSelection();
                    state = State.Done;
                }
                else if ((!gamepad && Input.GetKeyDown(back)) ||
                    (gamepad && Input.GetAxisRaw(vert) == -1))
                {
                    state = State.BeetleSelect;
                }
            }
            else if (state == State.Done)
            {
                // message

                canvas.transform.Find("Message").GetComponent<Text>().text = "WAITING FOR OTHER PLAYERS";

                // input polling

                if ((!gamepad && Input.GetKeyDown(back)) ||
                    (gamepad && Input.GetAxisRaw(vert) == -1))
                {
                    // undo finalization

                    FindObjectOfType<CustomizationManager>().RemoveBeetleSelection(index);
                    state = State.ColorSelect;
                }
            }
        }
    }

    private void LateUpdate()
    {
        // allows application of jiggle physics without the model actually moving

        thisModel.transform.position = transform.position + offsetFromCamera;
    }



    private void UpdateModel()
    {
        // get rid of any remnants of the old model

        if (thisModel != null)
        {
            thisModel.transform.Find("Visuals").Find("Trail").gameObject.SetActive(true);
            Destroy(thisModel);
            StopAllCoroutines();
        }

        // create model and tweak as needed

        thisModel = Instantiate(models[modelIdx]);
        thisModel.GetComponent<Rigidbody>().useGravity = false;
        thisModel.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // reset its colors

        colorIdx = -1;
        Material[] originals = thisModel.transform.Find("Visuals").GetComponent<Renderer>().materials;
        originalBase = originals[0];
        originalHighlight = originals[1];
        originalShadow = originals[2];

        // disable the trail, because it's annoying

        thisModel.transform.Find("Visuals").Find("Trail").gameObject.SetActive(false);

        StartCoroutine(Jiggle());
    }

    private void UpdateColors()
    {
        // unjiggle for visual effect

        StopAllCoroutines();

        // change the model's colors stored in "Visuals"

        if (colorIdx == -1)
        {
            thisModel.transform.Find("Visuals").GetComponent<Renderer>().materials =
                new Material[] { originalBase, originalHighlight, originalShadow };
            thisModel.transform.Find("Visuals").GetChild(1).GetComponent<Renderer>().material = originalShadow;
            thisModel.transform.Find("Visuals").GetChild(2).GetComponent<Renderer>().material = originalShadow;
            thisModel.transform.Find("Visuals").GetChild(3).GetComponent<Renderer>().material = originalShadow;
            thisModel.transform.Find("Visuals").GetChild(4).GetComponent<Renderer>().material = originalShadow;
        }
        else
        {
            thisModel.transform.Find("Visuals").GetComponent<Renderer>().materials =
                new Material[] { baseColors[colorIdx], highlightColors[colorIdx], shadowColors[colorIdx] };
            thisModel.transform.Find("Visuals").GetChild(1).GetComponent<Renderer>().material = shadowColors[colorIdx];
            thisModel.transform.Find("Visuals").GetChild(2).GetComponent<Renderer>().material = shadowColors[colorIdx];
            thisModel.transform.Find("Visuals").GetChild(3).GetComponent<Renderer>().material = shadowColors[colorIdx];
            thisModel.transform.Find("Visuals").GetChild(4).GetComponent<Renderer>().material = shadowColors[colorIdx];
        }

        StartCoroutine(Jiggle());
    }

    private void CreateCanvas()
    {
        canvas = Instantiate<Canvas>(canvas);
        canvas.transform.SetParent(transform, false);
        canvas.worldCamera = GetComponent<Camera>();

        // assign controls
        if (gamepad)
            canvas.transform.Find("Controls").GetComponent<Text>().text = "Gamepad Controls";
        else
            canvas.transform.Find("Controls").GetComponent<Text>().text = string.Format("{0} < > {1}\n{2} | {3}", left, right, submit, back);
    }

    private void FinalizeSelection()
    {
        // assign controls to the beetle and its visuals

        if (gamepad)
        {
            thisModel.GetComponent<Beetle>().AssignControls(horz, vert);
            thisModel.transform.Find("Visuals").GetComponent<BeetleVisuals>().AssignControls(horz);
        }
        else
        {
            thisModel.GetComponent<Beetle>().AssignControls(left, right, submit, back);
            thisModel.transform.Find("Visuals").GetComponent<BeetleVisuals>().AssignControls(left, right);
        }

        // fix everything I changed for visual effect

        thisModel.transform.Find("Visuals").Find("Trail").gameObject.SetActive(true);
        thisModel.GetComponent<Rigidbody>().useGravity = true;
        thisModel.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        thisModel.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // send to the manager

        FindObjectOfType<CustomizationManager>().AssignFinalBeetle(thisModel, index);
    }

    private IEnumerator Jiggle()
    {
        // block inputs while it's tiny

        inputBlocker = true;

        // start infinitely small, and grow to normal size

        Vector3 initialScale = thisModel.transform.localScale;
        thisModel.transform.localScale /= 100f;
        Vector3 velRef = Vector3.zero;
        while (Mathf.Abs((thisModel.transform.localScale - initialScale).magnitude) > 0.01f)
        {
            thisModel.transform.localScale = Vector3.SmoothDamp(
                thisModel.transform.localScale,
                initialScale,
                ref velRef,
                bounceInterval / 5f);
            yield return null;
        }

        // ensure it gets back to size

        thisModel.transform.localScale = initialScale;
        inputBlocker = false;

        // Jiggle until this coroutine is destroyed

        while (true)
        {
            thisModel.GetComponent<Rigidbody>().AddForce(new Vector3(0, bounceForce, 0));
            yield return new WaitForSeconds(bounceInterval);
        }
    }

    private int SlideM(int idx, int x)
    {
        idx += x;
        if (idx == -1)
            idx = models.Count - 1;
        if (idx == models.Count)
            idx = 0;
        return idx;
    }

    private int SlideC(int idx, int x)
    {
        idx += x;
        
        // allows idx to be -1 so the original color can be preserved

        if (idx == -2)
            idx = baseColors.Count - 1;
        if (idx == baseColors.Count)
            idx = -1;
        return idx;
    }

    public void AssignControls(string x, string y)
    {
        gamepad = true;

        horz = x;
        vert = y;
    }

    public void AssignControls(KeyCode l, KeyCode r, KeyCode a, KeyCode d)
    {
        gamepad = false;

        left = l;
        right = r;
        submit = a;
        back = d;
    }
}
