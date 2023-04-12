using System.Collections.Generic;

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerManager : MonoBehaviour {
	// singleton instance
	public static PlayerManager instance;

	[Header("Unit Prefabs")]
	[SerializeField] private GameObject attack = null;
	[SerializeField] private GameObject ranged = null;
	[SerializeField] private GameObject support = null;
	[SerializeField] private GameObject pylon = null;
	[SerializeField] private GameObject die = null;

	// resources
	private int crystals;
	private int stardust;
	private int divineBlood;

	private Dictionary<int, PlayerUnit> units;
	public List<int> selectedUnits;

	private bool showHealthbar;

	private void Awake() {
		// singleton assign
		instance = this;

		// resources
		crystals = 50;
		stardust = 30;
		divineBlood = 0;

		units = new Dictionary<int, PlayerUnit>();
		selectedUnits = new List<int>();

		showHealthbar = false;
	}

	private void Start() {
		InvokeRepeating("AddCrystalsOne", 0.0f, 1.0f);
	}

	private void Update() {

		// movement handler
		if (Input.GetMouseButtonDown(1)) {
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {

				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
					if (hit.transform.gameObject.tag == "PalaceFloor") // a weird workaround for navmesh not being able to work with dynamic map obstacles
					{
						if (EnemyManager.instance.barrierup == false)
						{
							MoveSelectedUnits(hit.point);
						}
					}
                    else
                    {
						MoveSelectedUnits(hit.point);
					}
				} else if (hit.transform.gameObject.GetComponent<PlayerUnit>() != null) {
					TargetMoveHealers(hit.transform.gameObject.GetComponent<PlayerUnit>().GetID());
				} else if (hit.transform.gameObject.GetComponent<EnemyUnit>() != null) {
					TargetMoveOffensiveUnits(hit.transform.gameObject.GetComponent<EnemyUnit>().GetID());
				}
			}
		}

		// UI handling
		// NOTE: This method is taxing and a little ugly, we can achieve the same thing by using a state system 
		int pylonstate = 1;
		int meleestate = 1;
		int healerstate = 1;
		int rangerstate = 1;

		for (int i = 0; i < selectedUnits.Count; i++) {
			if (units[selectedUnits[i]].GetComponent<PlayerAttack>() != null) {
				pylonstate = 0;
				healerstate = 0;
				rangerstate = 0;
			}
			if (units[selectedUnits[i]].GetComponent<PlayerSupport>() != null) {
				pylonstate = 0;
				meleestate = 0;
				rangerstate = 0;
			}
			if (units[selectedUnits[i]].GetComponent<PlayerPylon>() != null) {
				meleestate = 0;
				healerstate = 0;
				rangerstate = 0;
			}
			if (units[selectedUnits[i]].GetComponent<PlayerRanged>() != null) {
				pylonstate = 0;
				healerstate = 0;
				meleestate = 0;
			}

		}

		int[] arr = { pylonstate, meleestate, healerstate, rangerstate };
		int sum = 0;

		for (int i = 0; i < arr.Length; i++) {
			sum += arr[i];
		}

		if (sum == 0 || sum == 4) {
			UIManager.instance.SetUIstate(0);
		} else {
			if (pylonstate == 1) {
				UIManager.instance.SetUIstate(1);
			}
			if (meleestate == 1) {
				UIManager.instance.SetUIstate(2);
			}
			if (healerstate == 1) {
				UIManager.instance.SetUIstate(3);
			}
			if (rangerstate == 1) {
				UIManager.instance.SetUIstate(4);
			}
		}


		//Activate Healthbar
		if (Input.GetKeyDown(KeyCode.Tab)) //this could be done faster
		{
			if (!showHealthbar) {
				foreach (KeyValuePair<int, PlayerUnit> unit in units) {
					unit.Value.transform.Find("HealthbarCanvas").gameObject.SetActive(true);
				}
				showHealthbar = true;
			} else if (showHealthbar) {
				foreach (KeyValuePair<int, PlayerUnit> unit in units) {
					unit.Value.transform.Find("HealthbarCanvas").gameObject.SetActive(false);
				}
				showHealthbar = false;
			}
		}

	}

	// custom action 1
	public void UIaction1() {
		for (int i = 0; i < selectedUnits.Count; i++) {
			units[selectedUnits[i]].Action1();
		}
	}

	// custom action 2
	public void UIaction2() {
		for (int i = 0; i < selectedUnits.Count; i++) {
			units[selectedUnits[i]].Action2();
		}
	}

	// custom action 2
	public void UIaction3() {
		for (int i = 0; i < selectedUnits.Count; i++) {
			units[selectedUnits[i]].Action3();
		}
	}

	public void UIaction4() {
		for (int i = 0; i < selectedUnits.Count; i++) {
			units[selectedUnits[i]].Action4();
		}
	}

	public void UIaction5() {
		for (int i = 0; i < selectedUnits.Count; i++) {
			units[selectedUnits[i]].Action5();
		}
	}

	//RESOURCE HANDLING

	public int GetCrystals() {
		return crystals;
	}

	public void AddCrystals(int x) {
		crystals += x;
		UIManager.instance.UpdateCrystals(crystals);
	}

	public void AddCrystalsOne() {
		crystals++;
		UIManager.instance.UpdateCrystals(crystals);
	}

	public void RemoveCrystals(int x) {
		crystals = Mathf.Max(crystals - x, 0);
		UIManager.instance.UpdateCrystals(crystals);
	}

	public int GetStardust() {
		return stardust;
	}

	public void AddStardust(int x) {
		stardust += x;
		UIManager.instance.UpdateCrystals(stardust);
	}

	public void RemoveStardust(int x) {
		stardust = Mathf.Max(stardust - x, 0);
	}

	public int GetDivineBlood() {
		return divineBlood;
	}

	public void AddDivineBlood(int x) {
		divineBlood += x;
		UIManager.instance.UpdateCrystals(divineBlood);
	}

	public void RemoveDivineBlood(int x) {
		divineBlood = Mathf.Max(divineBlood - x, 0);
	}

	//Build units
	public void CreateWarrior(Vector3 pos) {
		//check if area where it's spawning will cause no collisions:

		//radius check value will be constant for now, eventually once this function is updated to spawn any kind of unity, we'd pass through the agent.radius value
		pos.y = 0.5f;
		pos.x += 1f;
		int dynamicmask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
		int staticmask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
		int mask = dynamicmask | staticmask;

		var collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);

		while (collisioncheck.Length != 0) {
			pos.x += 1; //move it over to the right, this can also change eventually
			collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);
		}

		GameObject g = Instantiate(attack, pos, Quaternion.identity);
		int layer = LayerMask.NameToLayer("DynamicPlayerUnits");
		g.layer = layer;
	}

	public void CreateRanger(Vector3 pos) {
		//check if area where it's spawning will cause no collisions:

		//radius check value will be constant for now, eventually once this function is updated to spawn any kind of unity, we'd pass through the agent.radius value
		pos.y = 0.5f;
		pos.x += 1f;
		int dynamicmask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
		int staticmask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
		int mask = dynamicmask | staticmask;

		var collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);

		while (collisioncheck.Length != 0) {
			pos.x += 1; //move it over to the right, this can also change eventually
			collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);
		}

		GameObject g = Instantiate(ranged, pos, Quaternion.identity);
		int layer = LayerMask.NameToLayer("DynamicPlayerUnits");
		g.layer = layer;
	}

	public void CreateHealer(Vector3 pos) {
		//check if area where it's spawning will cause no collisions:

		//radius check value will be constant for now, eventually once this function is updated to spawn any kind of unity, we'd pass through the agent.radius value
		pos.y = 0.5f;
		pos.x += 1f;
		int dynamicmask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
		int staticmask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
		int mask = dynamicmask | staticmask;

		var collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);

		while (collisioncheck.Length != 0) {
			pos.x += 1; //move it over to the right, this can also change eventually
			collisioncheck = Physics.OverlapSphere(pos, 0.25f, mask);
		}

		GameObject g = Instantiate(support, pos, Quaternion.identity);
		int layer = LayerMask.NameToLayer("DynamicPlayerUnits");
		g.layer = layer;
	}

	//UNIT LIST HANDLING

	public void AddUnit(PlayerUnit unit) {
		units.Add(unit.GetID(), unit);
	}

	public void RemoveUnit(int id) {

		// if a unit dies while selected then remove from selected units
		if (selectedUnits.Contains(id)) {
			RemoveSelectedUnit(id);
		}

		Destroy(units[id].gameObject);

		if (!units.Remove(id)) {
			return;
		}

	}

	public PlayerUnit GetUnit(int id) {
		PlayerUnit p;
		units.TryGetValue(id, out p);
		return p;
	}

	public void ResetPylons() {
		foreach (PlayerUnit p in units.Values) {
			if (p is PlayerPylon) {
				((PlayerPylon)p).ResetConnection();
			}
		}

		// chain from start
		PlayerPylon.instance.Connect();
	}

	// SELECTION HANDLING

	public void AddSelectedUnit(int id) {
		units[id].Select();
		selectedUnits.Add(id);
	}

	public void RemoveSelectedUnit(int id) {
		units[id].Deselect();
		selectedUnits.Remove(id);
	}

	public void ClearSelectedUnits() {

		UIManager.instance.UIstate = 0;
		for (int i = 0; i < selectedUnits.Count; i++) {
			units[selectedUnits[i]].Deselect();
		}

		selectedUnits.Clear();
	}

	public void MoveSelectedUnits(Vector3 destination) {
		// set destination of all selected units, and set their states to 1 (which will be the state representing "commanded movement") for all dynamic units
		for (int i = 0; i < selectedUnits.Count; i++) {

			if (units[selectedUnits[i]].GetComponent<PlayerAttack>() != null) //TEMPORARY, SINCE THE STATE SYSTEM IS ONLY IMPLEMENTED IN THE MELEE CLASS FOR NOW
			{
				units[selectedUnits[i]].GetComponent<PlayerAttack>().state = 1;
				units[selectedUnits[i]].GetComponent<PlayerAttack>().target = -1;
			} else if (units[selectedUnits[i]].GetComponent<PlayerRanged>() != null) {
				units[selectedUnits[i]].GetComponent<PlayerRanged>().state = 1;
				units[selectedUnits[i]].GetComponent<PlayerRanged>().target = -1;
			} else if (units[selectedUnits[i]].GetComponent<PlayerSupport>() != null) {
				units[selectedUnits[i]].GetComponent<PlayerSupport>().state = 1;
				units[selectedUnits[i]].GetComponent<PlayerSupport>().target = -1;
			}
			units[selectedUnits[i]].SetDestination(destination);
		}

	}

	public void TargetMoveOffensiveUnits(int target) {
		for (int i = 0; i < selectedUnits.Count; i++) {

			if (units[selectedUnits[i]].GetComponent<PlayerAttack>() != null) //TEMPORARY, SINCE THE STATE SYSTEM IS ONLY IMPLEMENTED IN THE MELEE CLASS FOR NOW
			{
				units[selectedUnits[i]].GetComponent<PlayerAttack>().target = EnemyManager.instance.GetUnit(target).GetID();
				units[selectedUnits[i]].GetComponent<PlayerAttack>().state = 2;
				units[selectedUnits[i]].SetDestination(EnemyManager.instance.GetUnit(target).transform.position);
			} else if (units[selectedUnits[i]].GetComponent<PlayerRanged>() != null) {
				units[selectedUnits[i]].GetComponent<PlayerRanged>().target = EnemyManager.instance.GetUnit(target).GetID();
				units[selectedUnits[i]].GetComponent<PlayerRanged>().state = 2;
				units[selectedUnits[i]].SetDestination(EnemyManager.instance.GetUnit(target).transform.position);
			}
		}
	}

	public void TargetMoveHealers(int target) {

		for (int i = 0; i < selectedUnits.Count; i++) {
			if (units[selectedUnits[i]].GetComponent<PlayerSupport>() != null) {
				units[selectedUnits[i]].GetComponent<PlayerSupport>().target = PlayerManager.instance.GetUnit(target).GetID();
				units[selectedUnits[i]].GetComponent<PlayerSupport>().state = 2;
				units[selectedUnits[i]].SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
			}
		}
	}
}