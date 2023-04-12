using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit : Unit {
	[SerializeField] public PlayerUnitBlueprint blueprint = null;

	private bool selected;

	protected Vector3 destination;

	protected new Rigidbody rigidbody;

	protected Animator animator;

	private Outline outline;

	private void Awake() {
		// unit setup
		Setup(blueprint.health);

		outline = GetComponent<Outline>();
		outline.enabled = false;

		// deselected by default
		Deselect();

		destination = transform.position;

		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();

		agent.autoBraking = false;
		agent.acceleration = 90;
		agent.angularSpeed = 1800;
		agent.speed = 3.0f;


	}

	protected void Start() {
		// temporary unit adding
		PlayerManager.instance.AddUnit(this);
	}

	protected void Update() {

		if ((GetComponent<PlayerPylon>() != null) || (GetComponent<PlayerDie>() != null) || (GetComponent<SiegePylon>() != null)) {
			return;
		}

		if (blueprint.movable && Vector3.Distance(transform.position, destination) > 0.3f) {
			agent.isStopped = false;
			agent.SetDestination(destination);
		} else {
			agent.isStopped = true;
			agent.SetDestination(transform.position);
		}

	}

	public void Select() {
		outline.enabled = true;
		selected = true;
	}

	public void Deselect() {
		outline.enabled = false;
		selected = false;
	}

	public void SetDestination(Vector3 destination) {
		this.destination = destination;
	}

	public abstract void Action1();
	public abstract void Action2();
	public abstract void Action3();
	public abstract void Action4();
	public abstract void Action5();

	// level up unit
	public new bool LevelUp() {
		if (PlayerManager.instance.GetCrystals() >= blueprint.levelUpCost) {
			PlayerManager.instance.RemoveCrystals(blueprint.levelUpCost);

			base.LevelUp();

			Debug.Log("LEVEL UP!");

			return true;
		}

		return false;
	}

	// harvest unit for crystals
	public void Harvest() {
		PlayerManager.instance.RemoveUnit(GetID());
		PlayerManager.instance.AddCrystals(blueprint.cost);
	}
}