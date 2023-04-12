using System.Collections.Generic;

using UnityEngine;

public class EnemyManager : MonoBehaviour {
	// singleton instance

	public static EnemyManager instance;

	private Dictionary<int, EnemyUnit> units;

	[SerializeField] private GameObject crystal = null;

	private bool showHealthbar;

	[SerializeField] private EnemyGuard guard = null;

	public bool barrierup = true;

	private void Awake() {
		// singleton assign
		instance = this;

		units = new Dictionary<int, EnemyUnit>();
		showHealthbar = false;
	}

	private void Update() {
		//Activate Healthbar
		if (Input.GetKeyDown(KeyCode.Tab)) //this could be done faster
		{
			if (!showHealthbar) {
				foreach (KeyValuePair<int, EnemyUnit> unit in units) {
					unit.Value.transform.Find("HealthbarCanvas").gameObject.SetActive(true);
				}
				showHealthbar = true;
			} else if (showHealthbar) {
				foreach (KeyValuePair<int, EnemyUnit> unit in units) {
					unit.Value.transform.Find("HealthbarCanvas").gameObject.SetActive(false);
				}
				showHealthbar = false;
			}
		}
	}

	public void AddUnit(EnemyUnit unit) {
		units.Add(unit.GetID(), unit);
	}

	public void RemoveUnit(int id) {

		if(GetUnit(id).gameObject.layer == LayerMask.NameToLayer("Barrier")){ //A very strange fix for navmesh not being able to accomodate for dynamic map obstacles
			barrierup = false;
        }

		if (GetUnit(id).gameObject.name == "BossUnit")
		{
			GameObject dracula = GameObject.Find("Dracula Boss");
			Destroy(dracula);
			for (int i = 0; i < 50; i++)
            {
				Instantiate(crystal, dracula.transform.position, Quaternion.identity);
			}
			return;
		}

		Instantiate(crystal, GetUnit(id).transform.position, Quaternion.identity);
		Destroy(units[id].gameObject);

		if (!units.Remove(id)) {
			return;
		}
	}

	public EnemyUnit GetUnit(int id) {
		EnemyUnit e;
		units.TryGetValue(id, out e);
		return e;
	}

	public void CreateGuard(Vector3 pos, int type)
	{
		//check if area where it's spawning will cause no collisions:

		//radius check value will be constant for now, eventually once this function is updated to spawn any kind of unity, we'd pass through the agent.radius value
		pos.z -= 1f;
		int dynamicmask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
		int staticmask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
		int mask = dynamicmask | staticmask;

		var collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);

		while (collisioncheck.Length != 0)
		{
			pos.x += 1; //move it over to the right, this can also change eventually
			collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);
		}

		EnemyGuard g = Instantiate(guard, pos, Quaternion.identity);
		g.SetType(type);
		int layer = LayerMask.NameToLayer("Enemy");
		g.gameObject.layer = layer;
	}

}