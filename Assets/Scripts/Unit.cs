using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour {
	private static int nextID;

	[SerializeField] private int id;
	private int level;
	[SerializeField] private int health;

	public NavMeshAgent agent;

	private Material hurtmat;
	private Material origmat;

	protected void Setup(int health) {
		id = nextID++;
		level = 1;
		this.health = health;
		hurtmat = Resources.Load<Material>("Damage");
		origmat = transform.GetChild(0).GetComponent<Renderer>().material;
	}

	public int GetID() {
		return id;
	}

	public int GetLevel() {
		return level;
	}

	protected void LevelUp() {
		level++;
	}

	public int GetHealth() {
		return health;
	}

	public void AddHealth(int x) {
		health += x;
	}

	public void RemoveHealth(int x) {

		health = Mathf.Max(health - x, 0);

		transform.GetChild(0).GetComponent<Renderer>().material = hurtmat;
		Invoke("ResetColour", 0.1f);

		if (health == 0) {
			if (GetComponent<PlayerUnit>() != null) {
				PlayerManager.instance.RemoveUnit(id);
			} else {
				EnemyManager.instance.RemoveUnit(id);
			}
		}
	}

	void ResetColour() {
		transform.GetChild(0).GetComponent<Renderer>().material = origmat;
	}
}