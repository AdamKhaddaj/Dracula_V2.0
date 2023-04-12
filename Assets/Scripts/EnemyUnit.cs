using UnityEngine;
using System.Collections;

public abstract class EnemyUnit : Unit {
	[SerializeField] public EnemyUnitBlueprint blueprint = null;

	protected Vector3 destination;

	protected new Rigidbody rigidbody;

	public Animator animator;

	protected void Awake() {
		// unit setup
		Setup(blueprint.health);

		destination = transform.position;

		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();

		agent.autoBraking = false;
		agent.acceleration = 90;
		agent.angularSpeed = 1000;
		agent.speed = 2.5f;

	}

	protected void Start() {
		// temporary unit adding
		EnemyManager.instance.AddUnit(this);

	}

    protected void Update()
    {

		if (GetComponent<EnemyPylon>() != null || GetComponent<EnemyBarrier>() != null || GetComponent<DraculaHitbox>() != null)
		{
			return;
		}

		if (blueprint.movable && Vector3.Distance(transform.position, destination) > 0.3f)
		{
			agent.isStopped = false;
			agent.SetDestination(destination);
		}
		else
		{
			agent.isStopped = true;
			agent.SetDestination(transform.position);
		}
	}

    public void SetDestination(Vector3 destination)
	{
		this.destination = destination;
	}

	public void TempSpeedChange()
    {
		StartCoroutine(SpeedChangeCoroutine());
    }

	public IEnumerator SpeedChangeCoroutine()
    {
		agent.speed = 1f;
		yield return new WaitForSeconds(6f);
		agent.speed = 2.5f;
    }

}