using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionScreen : MonoBehaviour {

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

    private KeyCode left = KeyCode.LeftArrow;
    private KeyCode right = KeyCode.RightArrow;
    private KeyCode submit = KeyCode.UpArrow;
    private KeyCode back = KeyCode.DownArrow;
    private bool inputBlocker;

    // Flow

    private enum State { BeetleSelect, ColorSelect };
    private State state;

    // UI

    public Text message;


    private void Awake()
    {
        UpdateModel();
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

                message.text = "CHOOSE YOUR BEETLE";

                // input polling

                if (Input.GetKeyDown(left))
                {
                    modelIdx = SlideM(modelIdx, -1);
                    UpdateModel();
                }
                else if (Input.GetKeyDown(right))
                {
                    modelIdx = SlideM(modelIdx, 1);
                    UpdateModel();
                }
                else if (Input.GetKeyDown(submit))
                {
                    state = State.ColorSelect;
                }
                else if (Input.GetKeyDown(back))
                {
                    // nothing
                }
            }
            else if (state == State.ColorSelect)
            {
                // message

                message.text = "CHOOSE YOUR COLOR SCHEME";

                // input polling

                if (Input.GetKeyDown(left))
                {
                    colorIdx = SlideC(colorIdx, -1);
                    UpdateColors();
                }
                else if (Input.GetKeyDown(right))
                {
                    colorIdx = SlideC(colorIdx, 1);
                    UpdateColors();
                }
                else if (Input.GetKeyDown(submit))
                {
                    state = State.ColorSelect;
                }
                else if (Input.GetKeyDown(back))
                {
                    state = State.BeetleSelect;
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
}
