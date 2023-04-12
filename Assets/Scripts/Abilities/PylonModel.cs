using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonModel : MonoBehaviour {

	public GameObject prefab;
	public PlayerPylon parentpylon;

	public SiegePylon sieger;

	private Material hurtmat;
	private Material origmat;
	private Material siegemat;

	bool canbuild, siegePylon;

	void Start() {
		//putting this in cause it kinda bugs out on the frame it first spawns
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
				transform.position = hit.point;
			}
		}

		hurtmat = Resources.Load<Material>("Damage");
		siegemat = Resources.Load<Material>("Sieger");
		origmat = transform.GetChild(0).GetComponent<Renderer>().material;

		canbuild = false;
		siegePylon = false;
	}

	public void SetParentPylon(PlayerPylon p) {
		parentpylon = p;
	}

	// Update is called once per frame
	void Update() {

		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
				transform.position = hit.point;
			}

			if (Vector3.Distance(hit.point, parentpylon.transform.position) > 12f) {
				transform.GetChild(0).GetComponent<Renderer>().material = hurtmat;
				transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material = hurtmat;
				canbuild = false;
				siegePylon = false;
			} else {

				//check if it can siege the barrier
				int barrier_layer = LayerMask.GetMask("Barrier");
				Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 13f, barrier_layer);

				if (colliders.Length > 0) //it can siege
				{
					transform.GetChild(0).GetComponent<Renderer>().material = siegemat;
					transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material = siegemat;
					siegePylon = true;
					canbuild = true;
				}
                else
                {
					transform.GetChild(0).GetComponent<Renderer>().material = origmat;
					transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material = origmat;
					canbuild = true;
					siegePylon = false;
				}
			}
		}

		if (Input.GetMouseButtonUp(0) && canbuild) {

            if (siegePylon)
            {
				Instantiate(sieger, transform.position, transform.rotation);
				Destroy(gameObject);
			}

            else
            {
				Instantiate(prefab, transform.position, transform.rotation);
				PlayerManager.instance.ResetPylons();
				Destroy(gameObject);
			}
		}

	}
}
