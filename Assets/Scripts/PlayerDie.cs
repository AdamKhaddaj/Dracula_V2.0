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

		int result = ResultChance();

		Debug.Log(result);

        if (result == 0) //ATTACK UP
        {
			int layer_mask = LayerMask.GetMask("DynamicPlayerUnits");
			Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 8f, layer_mask);

			for (int i = 0; i < colliders.Length; i++)
			{
				if(colliders[i].gameObject.GetComponent<PlayerAttack>() != null)
                {
					colliders[i].gameObject.GetComponent<PlayerAttack>().PowerUp();
				}
				if (colliders[i].gameObject.GetComponent<PlayerRanged>() != null)
				{
					colliders[i].gameObject.GetComponent<PlayerRanged>().PowerUp();
				}
			}
		}

		if (result == 1) //Damage Blast
		{
			int layer_mask = LayerMask.GetMask("Enemy");
			Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 8f, layer_mask);

			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].gameObject.GetComponent<Unit>().RemoveHealth(5);
			}
		}

		if (result == 2) //Gain Crystals
		{
			PlayerManager.instance.AddCrystals(50);
		}
		if (result == 3) //Explode!
		{
			int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
			int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
			int enemy_layer_mask = 1 << LayerMask.NameToLayer("Enemy");
			int layermask = static_layer_mask | dynamic_layer_mask | enemy_layer_mask;
			Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 8f, layermask);

			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].gameObject.GetComponent<Unit>().RemoveHealth(5);
			}
			Destroy(gameObject);
		}
		if (result == 4) //Reroll
		{
			Debug.Log("Reroll!");
		}
		if (result == 5) //Heal
		{
			int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
			int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
			int siege_layer_mask = 1 << LayerMask.NameToLayer("Sieger");
			int layermask = static_layer_mask | dynamic_layer_mask | siege_layer_mask;
			Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 8f, layermask);

			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].gameObject.GetComponent<Unit>().AddHealth(10);
			}
		}

	}

	private int ResultChance() {

		float[] sides = new float[6];

		sides[0] = Mathf.Abs(transform.up.y - 1); //atkup
		sides[1] = Mathf.Abs(transform.right.y - 1); //blast
		sides[2] = Mathf.Abs(transform.forward.y - 1); //crystl
		sides[3] = Mathf.Abs(transform.forward.y - (-1)); //deatj
		sides[4] = Mathf.Abs(transform.right.y - (-1)); //reroll
		sides[5] = Mathf.Abs(transform.up.y - (-1)); //heal

		float smallest = Mathf.Infinity;
		int smallestindex = -1;

		for(int i = 0; i < 6; i++)
        {
			if(sides[i] < smallest)
            {
				smallest = sides[i];
				smallestindex = i;
            }
        }

		return smallestindex;
		
	}

	public override void Action1() {
		
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