using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : PlayerUnit {
	bool rolling, testselect;
	GameObject indicator;
	Vector3 indicatorDir;

	public float flick_y, flick_force, spin_force;

	private void Start() {
		base.Start();
		indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		indicator.transform.position = gameObject.transform.position;
		indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		indicator.GetComponent<Collider>().enabled = false;
		indicator.SetActive(false);
		rolling = false;
		testselect = false;

		flick_force = 4.5f;
		flick_y = 8;
		spin_force = 12;

	}

	private void Roll(Vector3 dir) {

		Vector3 flickdir = dir * 4;
		flickdir.y += flick_y;

		rigidbody.AddForce(flickdir * flick_force, ForceMode.Impulse);

		Vector3 spin = Random.insideUnitSphere;
		//spin.y = 50;

		rigidbody.AddTorque(spin * spin_force, ForceMode.Impulse);

		Invoke("SetRolling", 1);

	}

	private void SetRolling() {
		rolling = true;
	}

	private void OnMouseOver() //TEST CODE
	{
		if (Input.GetMouseButtonUp(0)) {
			indicator.SetActive(true);
			indicator.transform.position = gameObject.transform.position;
			testselect = true;
		}
	}

	private void Update() {
		if (testselect) {
			//update the position of the indictor such that it follows the mouse along a fixed orbit around the dice, and then flicks in that direction if mouse down
			//First, get direction of mouse relative to the dice

			int layer_mask = LayerMask.GetMask("Terrain");
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);


			if (Physics.Raycast(ray, out hit, 1000f, layer_mask)) {
				// get direction vector

				indicatorDir = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z) - gameObject.transform.position;
			}

			Vector3 pos;

			indicatorDir.Normalize();
			indicatorDir *= 1.8f;

			pos.y = gameObject.transform.position.y;
			pos.x = gameObject.transform.position.x + indicatorDir.x;
			pos.z = gameObject.transform.position.z + indicatorDir.z;

			indicator.transform.position = pos;



			if (Input.GetMouseButtonDown(0)) {
				Roll(indicatorDir);
				testselect = false;
				indicator.SetActive(false);
			}

		}

		if (rolling) {
			if (rigidbody.velocity.magnitude < 0.001) {
				rolling = false;
				FindResult();
			}
		}
	}

	private void FindResult() {
		int result = 0;

		Vector3 rotation = gameObject.transform.eulerAngles;

		rotation = new Vector3(Mathf.RoundToInt(rotation.x), Mathf.RoundToInt(rotation.y), Mathf.RoundToInt(rotation.z));

		//By checking rotation values along x, y, and z axis at increments of 90 degrees, we can tell what side of the dice is facing up
		//This code borrows from the following: https://levidsmith.com/games/yatzy-dice-game/#dice_value

		//float attackside_chance = ResultChance(1);
		//float crystalside_chance = ResultChance(2);

		if (rotation.x == 180 && rotation.y == 270 ||
			rotation.x == 0 && rotation.z == 90) {
			result = 1;
			Debug.Log("DAMANGE BLAST");
		} else if (rotation.x == 270) {
			result = 2;
			Debug.Log("CRYSTALS GAIN");

		} else if (rotation.x == 180 && rotation.z == 0 ||
			rotation.x == 0 && rotation.z == 180) {
			result = 3;
			Debug.Log("HEALING BLAST");

		} else if (rotation.x == 180 && rotation.z == 180 ||
			rotation.x == 0 && rotation.z == 0) {
			result = 4;
			Debug.Log("ATTACK UP");

		} else if (rotation.x == 90) {
			result = 5;
			Debug.Log("DEATH EXPLODE");

		} else if (rotation.x == 0 && rotation.z == 270 ||
			rotation.x == 180 && rotation.z == 90) {
			result = 6;
			Debug.Log("REROLL");

		}
	}

	private float ResultChance(int dieside) {
		if (dieside == 1) {
			return Mathf.Abs(1 - gameObject.transform.eulerAngles.y);
		}
		if (dieside == 2) {
			return Mathf.Abs(1 - gameObject.transform.eulerAngles.y);
		}
		if (dieside == 3) {
			return Mathf.Abs(1 - gameObject.transform.eulerAngles.y);
		}
		if (dieside == 4) {
			return Mathf.Abs(1 - gameObject.transform.eulerAngles.y);
		}
		if (dieside == 5) {
			return Mathf.Abs(1 - gameObject.transform.eulerAngles.y);
		}
		if (dieside == 6) {
			return Mathf.Abs(1 - gameObject.transform.eulerAngles.y);
		}
		return 0;
	}

	public override void Action1() {
		// move action
	}

	public override void Action2() {

	}

	public override void Action3() {

	}

	public override void Action4() {
	}

	public override void Action5() {
	}

}