using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystals : MonoBehaviour {

	Rigidbody rb;

	private void Start() {
		rb = GetComponent<Rigidbody>();
	}

	void Update() {
		int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
		int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
		int layermask = static_layer_mask | dynamic_layer_mask;

		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 3f, layermask);

		if (colliders.Length > 0) {
			for (int i = 0; i < colliders.Length; i++) {
				if (Vector3.Distance(colliders[i].gameObject.transform.position, transform.position) < 0.4f) {
					PlayerManager.instance.AddCrystals(5);
					Destroy(gameObject);
				}
			}

			//if its not collected, have it move towards a unit till its collected
			Vector3 movedir = colliders[0].gameObject.transform.position - transform.position;
			movedir = movedir.normalized * 3f;
			rb.velocity = movedir;

		}
	}
}
