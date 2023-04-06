using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBlast : MonoBehaviour {
	// Update is called once per frame
	void Update() {
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
				transform.position = hit.point;

				if (Input.GetMouseButtonDown(0)) {
					int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
					int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
					int enemy_layer_mask = 1 << LayerMask.NameToLayer("Enemy");
					int layermask = static_layer_mask | dynamic_layer_mask | enemy_layer_mask;

					Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 3f, layermask);

					if (colliders.Length > 0) {
						for (int i = 0; i < colliders.Length; i++) {
							if (colliders[i].gameObject.GetComponent<PlayerUnit>() != null) {
								colliders[i].gameObject.GetComponent<Unit>().AddHealth(10);
							} else if (colliders[i].gameObject.GetComponent<EnemyUnit>() != null) {
								colliders[i].gameObject.GetComponent<Unit>().RemoveHealth(5);
							}
						}
					}
					Destroy(gameObject);
				}
			}
		}
	}
}
