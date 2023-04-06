using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour {
	// NOTE: some segments of this code borrows from https://github.com/pickles976/RTS_selection/blob/master/global_selection.cs

	bool dragging;
	bool UIclicking;

	//for the rectangle that appears during click and dragon
	MeshCollider selectionBox;
	Mesh selectionMesh;

	//point you click vs point you let go
	Vector3 p1, p2;

	//corners of selection rectangle
	Vector2[] corners;

	//vertices of box we make to send out raycasts and actually do selections
	Vector3[] boxvertices;

	void Start() {
		dragging = false;
		UIclicking = false;
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButtonDown(0)) //single click
		{
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //Don't do selection handling stuff if a UI button is being clicked
			{
				UIclicking = true;
				return;
			}
			p1 = Input.mousePosition;
		}
		if (Input.GetMouseButton(0)) //check for dragging
		{
			if (UIclicking) {
				return;
			}
			if ((p1 - Input.mousePosition).magnitude > 30) {
				dragging = true;
			}
		}

		if (Input.GetMouseButtonUp(0)) //on mouse release
		{
			if (UIclicking) {
				UIclicking = false;
				return;
			}

			if (!dragging) {
				//Only check for dynamic and static player units
				int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
				int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
				int layermask = static_layer_mask | dynamic_layer_mask;

				RaycastHit hit;
				var ray = Camera.main.ScreenPointToRay(p1);

				if (Physics.Raycast(ray, out hit, 1000f, layermask)) {
					//No matter what, if it's a pylon, it should deselect everything and select that pylon (for now only one pylon can be selected at a time)
					if (hit.transform.gameObject.GetComponent<PlayerPylon>() != null) {
						PlayerManager.instance.ClearSelectedUnits();
						PlayerManager.instance.AddSelectedUnit(hit.transform.gameObject.GetComponent<PlayerUnit>().GetID());
					} else if (Input.GetKey(KeyCode.LeftControl)) //for ctrl clicking
					  {
						PlayerManager.instance.AddSelectedUnit(hit.transform.gameObject.GetComponent<PlayerUnit>().GetID());
					} else //deselect everything other than the thing you single clicked
					  {
						PlayerManager.instance.ClearSelectedUnits();
						PlayerManager.instance.AddSelectedUnit(hit.transform.gameObject.GetComponent<PlayerUnit>().GetID());
					}
				} else {
					if (!Input.GetKey(KeyCode.LeftControl)) {
						PlayerManager.instance.ClearSelectedUnits();
					}
				}
			} else {
				boxvertices = new Vector3[4];
				p2 = Input.mousePosition;
				corners = setCornerOrder(p1, p2);

				for (int i = 0; i < corners.Length; i++) {
					//Bug note: when doing drag select, if you start the drag or end the drag while pointing at the skybox, the raycast stuff doesn't work properly. 

					//int layer_mask = LayerMask.GetMask("Terrain"); 

					RaycastHit hit;
					var ray = Camera.main.ScreenPointToRay(corners[i]);

					if (Physics.Raycast(ray, out hit, 1000f)) {
						boxvertices[i] = new Vector3(hit.point.x, 0, hit.point.z);
					}

				}

				//make the mesh
				selectionMesh = createSelectionMesh(boxvertices);

				selectionBox = gameObject.AddComponent<MeshCollider>();
				selectionBox.sharedMesh = selectionMesh;
				selectionBox.convex = true;
				selectionBox.isTrigger = true;

				if (!Input.GetKey(KeyCode.LeftControl)) {
					PlayerManager.instance.ClearSelectedUnits();
				}

				Destroy(selectionBox, 0.03f);

			}
			dragging = false;
		}

	}

	private void OnGUI() {
		if (dragging) {
			var rect = BoxDrawer.getRect(p1, Input.mousePosition);
			BoxDrawer.makeRect(rect, new Color(0.7f, 0.7f, 0.7f, 0.20f));
			BoxDrawer.makeRectBorders(rect, 1.5f, new Color(0.7f, 0.7f, 0.7f));
		}
	}

	Vector2[] setCornerOrder(Vector2 p1, Vector2 p2) {
		// We always want the corner order to be the same regardless of drag direction. Since bottom left is (0,0), we can take the min vector for
		// bottom left corner, and the max vector for top right corner

		Vector2 bottomLeft = Vector2.Min(p1, p2);
		Vector2 topRight = Vector2.Max(p1, p2);
		Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);
		Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);

		Vector2[] rectCorners = { topLeft, topRight, bottomLeft, bottomRight };

		return rectCorners;
	}

	Mesh createSelectionMesh(Vector3[] corners) {
		Vector3[] vertices = new Vector3[8];
		int[] triangles = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

		for (int i = 0; i < 4; i++) {
			vertices[i] = corners[i];
		}

		for (int j = 4; j < 8; j++) {
			vertices[j] = corners[j - 4] + Vector3.up * 100.0f;
		}

		Mesh selectionMesh = new Mesh();
		selectionMesh.vertices = vertices;
		selectionMesh.triangles = triangles;

		return selectionMesh;

	}

	private void OnTriggerEnter(Collider c) {
		if (LayerMask.NameToLayer("DynamicPlayerUnits") == c.gameObject.layer) //Click dragging should only ever select moveable units
		{
			PlayerManager.instance.AddSelectedUnit(c.gameObject.GetComponent<PlayerUnit>().GetID());
		}
	}

}