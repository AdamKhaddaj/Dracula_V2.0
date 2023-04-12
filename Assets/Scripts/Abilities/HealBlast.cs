using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBlast : MonoBehaviour {

	public PlayerSupport parent;

	private Color origcolor;

	private bool cancast;

	private void Start()
	{
		//putting this in cause it kinda bugs out on the frame it first spawns
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
		{
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
			{
				transform.position = hit.point;
			}
		}

		origcolor = GetComponent<Renderer>().material.color;

		cancast = false;
	}

	public void SetParent(PlayerSupport sup)
    {
		parent = sup;
	}

	void Update() {
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
				transform.position = hit.point;
			}

			if (Vector3.Distance(hit.point, parent.transform.position) > 10f)
			{
				transform.GetComponent<Renderer>().material.color = Color.red;
				cancast = false;
			}

			else
			{
				transform.GetComponent<Renderer>().material.color = origcolor;
				cancast = true;
			}

			if (Input.GetMouseButtonDown(0) && cancast)
			{
				int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
				int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
				int enemy_layer_mask = 1 << LayerMask.NameToLayer("Enemy");
				int layermask = static_layer_mask | dynamic_layer_mask | enemy_layer_mask;

				Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 3f, layermask);

				if (colliders.Length > 0)
				{
					for (int i = 0; i < colliders.Length; i++)
					{
						if (colliders[i].gameObject.GetComponent<PlayerUnit>() != null)
						{
							colliders[i].gameObject.GetComponent<Unit>().AddHealth(10);
						}
						else if (colliders[i].gameObject.GetComponent<EnemyUnit>() != null)
						{
							colliders[i].gameObject.GetComponent<Unit>().RemoveHealth(5);
						}
					}
				}
				Destroy(gameObject);
			}
		}
	}
}
