using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour {

	public void BeetleCount(string s)
    {
        Settings.beetleCount = int.Parse(s);
    }
}
