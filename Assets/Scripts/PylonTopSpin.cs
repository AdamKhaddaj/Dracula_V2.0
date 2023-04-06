using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonTopSpin : MonoBehaviour {
	void Update() {
		transform.Rotate(0f, 10f * Time.deltaTime, 0f);
	}
}
