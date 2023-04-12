using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class PlayerSupport : PlayerUnit {

	[SerializeField] public float hoverspeed;

	[SerializeField] public int state; //only serialize field so it can be seen for debugging purposes

	private bool healing;

	private LineRenderer line;

	public HealBlast indicator;

	private void Start() {
		base.Start();
		base.agent.angularSpeed = 0;
		hoverspeed = 0.75f;
		state = 0;
		healing = false;

		line = GetComponent<LineRenderer>();

	}
	public override void Action1() {
		HealBlast b = Instantiate(indicator);
		b.SetParent(this);
	}

	public override void Action2() {
		Debug.Log("ACTIVATE THE SHIELDS");
	}

	public override void Action3() {

	}

	public override void Action4() {
	}

	public override void Action5()
	{

		PlayerManager.instance.AddCrystals(blueprint.cost);
		Destroy(this);

	}

	// Update is called once per frame
	void Update() {
		base.Update();

		if (state == 0) {
			line.enabled = false;

			base.agent.isStopped = true;
			rigidbody.velocity = Vector3.zero;
		}

		if (state == 1) //If this unit has been commanded to move somewhere, nothing should break it from trying to get to that location other than it reaching that location
		{
			line.enabled = false;

			if (Vector3.Distance(transform.position, base.destination) < 0.8f) {
				state = 0;
			}
		}

		if (state == 2) //this state is for moving towards an ally
		{
			line.enabled = false;

			//first check if the unit exists anymore, if not, stop coroutine
			if (PlayerManager.instance.GetUnit(target) == null) {
				state = 0;
				target = -1;
			} else {
				PlayerUnit p = PlayerManager.instance.GetUnit(target);

				//check if unit is close enought that it can start healing it

				if (Vector3.Distance(transform.position, base.destination) < 5f) {
					state = 3;
					base.SetDestination(transform.position);

				} else {
					base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
				}
			}
		}

		if (state == 3) //This state means this unit is capable of attacking it's target
		{
			//first check if the unit exists anymore, if not, exit state
			if (PlayerManager.instance.GetUnit(target) == null) {
				state = 0;
				target = -1;
			} 
			else if (Vector3.Distance(transform.position, PlayerManager.instance.GetUnit(target).transform.position) >= 5f) //if target moves too far away, start moving towards them
			  {
				state = 2;
				base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);

			} else  //Otherwise, heal will continue
			  {
				line.enabled = true;

				Vector3 startPosition = transform.position;
				Vector3 endPosition = PlayerManager.instance.GetUnit(target).transform.position;
				endPosition = new Vector3(endPosition.x, endPosition.y + 1f, endPosition.z);

				// Set the positions of the line renderer
				line.positionCount = 2;
				line.SetPosition(0, startPosition);
				line.SetPosition(1, endPosition);

                if (!healing)
                {
					healing = true;
					HealUnit();
				}
			}
		}

		Vector3 hover = new Vector3(0, 0, 0);
		hover.y = 0.2f + Mathf.Sin(Time.time * hoverspeed) * 0.20f;
		transform.position = new Vector3(transform.position.x, transform.position.y + hover.y, transform.position.z);
		transform.Rotate(0f, 10f * Time.deltaTime, 0f);

	}
	
	private void HealUnit()
    {
		if (PlayerManager.instance.GetUnit(target) != null && state == 3)
		{
			PlayerManager.instance.GetUnit(target).AddHealth(2);
			Invoke("HealUnit", 1f);
		}
        else
        {
			healing = false;
			return;
        }
	}
}
