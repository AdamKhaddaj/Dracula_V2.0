using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBlast : MonoBehaviour
{

	public GameObject parent;

	public Projectile projectile;

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

	public void SetParent(GameObject p)
	{
		parent = p;
	}

	void Update()
	{
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
		{
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
			{
				transform.position = hit.point;
			}

			if (Vector3.Distance(hit.point, parent.transform.position) > 15f)
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
				Projectile p = Instantiate(projectile, parent.transform.position, Quaternion.identity);
				p.SetTarget(parent.transform.position ,transform.position);
				Destroy(gameObject);
			}
		}
	}
}
