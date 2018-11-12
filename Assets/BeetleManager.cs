using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BeetleManager : MonoBehaviour {

    // determine what prefab to use
    // later this can be a list, so beetles can be unique
    public GameObject beetlePrefab;

    // select number of beetles
    public int beetleCount;
    public List<KeyCode> leftSteer;
    public List<KeyCode> rightSteer;
    public List<KeyCode> accelerate;
    public List<KeyCode> decelerate;

    private void Awake()
    {
        // create beetles, place them, and assign them controls
        for (int i = 0; i < beetleCount; i++)
        {
            GameObject thisBeetle = Instantiate(beetlePrefab);
            // assign position
            Vector3 pos = Vector3.zero;
            switch (i)
            {
                case 0:
                    pos = new Vector3(-1, 0, -25);
                    break;
                case 1:
                    pos = new Vector3(1, 0, -27);
                    break;
                case 2:
                    pos = new Vector3(-1, 0, -29);
                    break;
            }
            thisBeetle.transform.position = pos;
        }
    }


    private void OnValidate()
    {
        if (beetleCount > 3)
            beetleCount = 3;
        if (beetleCount < 0)
            beetleCount = 0;
        
        List<KeyCode> l = new List<KeyCode>(beetleCount);
        List<KeyCode> r = new List<KeyCode>(beetleCount);
        List<KeyCode> a = new List<KeyCode>(beetleCount);
        List<KeyCode> d = new List<KeyCode>(beetleCount);

        while (leftSteer.Count < beetleCount)
            leftSteer.Add(new KeyCode());
        
        while (rightSteer.Count < beetleCount)
            rightSteer.Add(new KeyCode());

        while (accelerate.Count < beetleCount)
            accelerate.Add(new KeyCode());

        while (decelerate.Count < beetleCount)
            decelerate.Add(new KeyCode());

        for (int i = 0; i < beetleCount; i++)
        {
            l[i] = leftSteer[i];
            r[i] = rightSteer[i];
            a[i] = accelerate[i];
            d[i] = decelerate[i];
        }

        leftSteer = l;
        rightSteer = r;
        accelerate = a;
        decelerate = d;
    }
}


