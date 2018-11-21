using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleSelect : MonoBehaviour {

    public List<GameObject> models;
    public Vector3 offsetFromCamera;

    public float rotationRate;

    public float bounceForce;
    public float bounceInterval;



    private GameObject thisModel;
    private int modelIdx;

    private void Awake()
    {
        UpdateModel();
    }

    private void Update()
    {
        thisModel.transform.Rotate(Vector3.up, rotationRate * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // poll for input

        // TODO : Customize these controls for individual players
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            modelIdx = Mathf.Abs(--modelIdx % models.Count);
            Debug.Log(modelIdx);
            UpdateModel();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            modelIdx = (++modelIdx % models.Count);
            UpdateModel();
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
        Destroy(thisModel);
        StopAllCoroutines();

        // create model and tweak as needed

        thisModel = Instantiate(models[modelIdx]);
        thisModel.GetComponent<Rigidbody>().useGravity = false;
        thisModel.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        StartCoroutine(Jiggle());
    }

    private IEnumerator Jiggle()
    {
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

        // Jiggle until this coroutine is destroyed

        while (true)
        {
            thisModel.GetComponent<Rigidbody>().AddForce(new Vector3(0, bounceForce, 0));
            yield return new WaitForSeconds(bounceInterval);
        }
    }
}
