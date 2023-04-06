using System.Collections.Generic;

using UnityEngine;

public class EnemyManager : MonoBehaviour {
	// singleton instance
	public static EnemyManager instance;

	private Dictionary<int, EnemyUnit> units;

	[SerializeField] private GameObject crystal = null;

	private bool showHealthbar;

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

}