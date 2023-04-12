using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPylon : PlayerUnit {
	public static PlayerPylon instance;

	public PylonModel pylonmodel_prefab;

	private LineRenderer line;
	private bool willBeConnected;
	private bool connected;

	private void Start() {
		if (instance == null) {
			instance = this;
		}

		base.Start();
		line = GetComponent<LineRenderer>();
	}

	public void Connect() {
		// in case start isn't called in time
		line = GetComponent<LineRenderer>();

		connected = true;

		int layer_mask = LayerMask.GetMask("StaticPlayerUnits");
		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 13f, layer_mask);

		List<PlayerPylon> pylons = new List<PlayerPylon>();

		for (int i = 0; i < colliders.Length; i++) {
			PlayerPylon pylon = colliders[i].GetComponent<PlayerPylon>();

			if (pylon && !pylon.IsConnected() && !pylon.willBeConnected) {
				pylon.willBeConnected = true;
				pylons.Add(pylon);
			}
		}

		line.positionCount = pylons.Count * 2;

		int j = 0;

		for (int i = 0; i < pylons.Count; i++) {
			line.SetPosition(j++, transform.position + new Vector3(0, 2, 0));
			line.SetPosition(j++, pylons[i].transform.position + new Vector3(0, 2, 0));

			pylons[i].Connect();
		}
	}

	public void ResetConnection() {
		willBeConnected = false;
		connected = false;
	}

	public bool IsConnected() {
		return connected;
	}

	public override void Action1() {
		if (PlayerManager.instance.GetCrystals() >= 10) {
			PlayerManager.instance.RemoveCrystals(10);
			PlayerManager.instance.CreateWarrior(transform.position);
		} else {
			Debug.Log("Not Enough Crystals!");
		}
	}

	public override void Action2() {
		if (PlayerManager.instance.GetCrystals() >= 10) {
			PlayerManager.instance.RemoveCrystals(10);
			PlayerManager.instance.CreateRanger(transform.position);
		} else {
			Debug.Log("Not Enough Crystals!");
		}
	}

	public override void Action3() {
		if (PlayerManager.instance.GetCrystals() >= 10) {
			PlayerManager.instance.RemoveCrystals(10);
			PlayerManager.instance.CreateHealer(transform.position);
		} else {
			Debug.Log("Not Enough Crystals!");
		}
	}

	public override void Action4() { //pylon construction

		if (PlayerManager.instance.GetCrystals() >= 50 && PlayerManager.instance.GetStardust() >= 10) {

			PylonModel p = Instantiate(pylonmodel_prefab);
			p.SetParentPylon(this);

		} else {
			Debug.Log("Not Enough Crystals!");
		}

	}

	public override void Action5()
	{

		Harvest();

	}
}