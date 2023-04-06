using UnityEngine;

public abstract class EnemyUnit : Unit {
	[SerializeField] public EnemyUnitBlueprint blueprint = null;

	protected Vector3 destination;

	protected new Rigidbody rigidbody;

	public Animator animator;

	private void Awake() {
		// unit setup
		Setup(blueprint.health);

		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();

		agent.autoBraking = false;
		agent.acceleration = 90;
		agent.angularSpeed = 1000;

		agent.speed = 9.0f;


	}

	protected void Start() {
		// temporary unit adding
		EnemyManager.instance.AddUnit(this);
	}

}