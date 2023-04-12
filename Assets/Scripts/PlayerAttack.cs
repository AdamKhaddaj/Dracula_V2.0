using UnityEngine;
using System.Collections;
using System;

public class PlayerAttack : PlayerUnit {

	[SerializeField] public int state; //only serialize field so it can be seen for debugging purposes
	private bool attacking, poweredup;

	private void Start() {
		base.Start();

		//For now, 0 = idle, 1 = moving, 2 = moving towards enemy, 3 = attacking
		//Target will be the ID of the enemy unit it is targetting for either attacking or moving towards, or -1 if it is not targetting anything
		state = 0;
		attacking = false;
		poweredup = false;

		//Animation stuff
		animator.SetBool("isRunning", false);
		animator.SetBool("isAttacking", false);
		animator.SetInteger("attackType", 0);
	}

	private void Update() {
		base.Update();
		if (state == 0) //The following code makes the unit automatically target an enemy unit only if it's within a certain radius and if the player unit idle
		{

			base.agent.isStopped = true;
			rigidbody.velocity = Vector3.zero;

			int layer_mask = LayerMask.GetMask("Enemy");
			Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 3f, layer_mask);

			if (colliders.Length > 0) //There is an enemy in aggro
			{
				//A target needs to be picked if there are multiple enemies. Pick the nearest enemy
				float closest = Mathf.Infinity;
				int closest_target = -1;

				for (int i = 0; i < colliders.Length; i++) {
					EnemyUnit enemyUnit = colliders[i].GetComponent<EnemyUnit>();
					if (Vector3.Distance(transform.position, enemyUnit.gameObject.transform.position) < closest) {
						closest_target = enemyUnit.GetID();
						closest = Vector3.Distance(transform.position, enemyUnit.gameObject.transform.position);
					}
				}

				target = closest_target;
				state = 2;
				base.SetDestination(EnemyManager.instance.GetUnit(target).transform.position);
			}
		}

		if (state == 1) //If this unit has been commanded to move somewhere, nothing should break it from trying to get to that location other than it reaching that location
		{
			if (Vector3.Distance(transform.position, base.destination) < 0.3f) {
				state = 0;
			}
		}

		if (state == 2) {
			//first check if the unit exists anymore, if not, stop coroutine
			if (EnemyManager.instance.GetUnit(target) == null) {
				state = 0;
				target = -1;
			} else {
				EnemyUnit e = EnemyManager.instance.GetUnit(target);

				//Check if either a) the target is close enough that this unit can attack it, or b) this unit should move towrads it

				if (Vector3.Distance(transform.position, base.destination) < 1f) {
					state = 3;
					base.SetDestination(transform.position);
				} else {
					base.SetDestination(EnemyManager.instance.GetUnit(target).transform.position);
				}
			}
		}

		if (state == 3) //This state means this unit is capable of attacking it's target
		{
			//first check if the unit exists anymore, if not, exit state
			if (EnemyManager.instance.GetUnit(target) == null) {
				state = 0;
				target = -1;
			} else if (Vector3.Distance(transform.position, EnemyManager.instance.GetUnit(target).transform.position) >= 1f) //if target moves too far away, start moving towards them
			  {
				state = 2;
				base.SetDestination(EnemyManager.instance.GetUnit(target).transform.position);
			} else //Otherwise, attack animation will continue playing, and trigger the deal damage function

			  {
				transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation((EnemyManager.instance.GetUnit(target).transform.position - transform.position), Vector3.up).eulerAngles.y, 0f);
			}

		}

		//Animation handling
		if (state == 0) {
			animator.SetBool("isRunning", false);
			animator.SetBool("isAttacking", false);
		} else if (state == 1 || state == 2) {
			animator.SetBool("isRunning", true);
			animator.SetBool("isAttacking", false);
		} else if (state == 3) {
			animator.SetBool("isRunning", false);
			animator.SetBool("isAttacking", true);
			int attackType = UnityEngine.Random.Range(0, 2);
			animator.SetInteger("attackType", attackType);
		}

	}

	private void DealDamage() {
		if (EnemyManager.instance.GetUnit(target) == null) {
			return;
		}
		//Actually deal the damage
		EnemyUnit e = EnemyManager.instance.GetUnit(target);
		e.RemoveHealth(5);
	}

	public void PowerUp()
	{
		StartCoroutine(PowerUpCoroutine());
	}

	public IEnumerator PowerUpCoroutine()
	{
		poweredup = true;
		agent.speed = 3.75f;
		gameObject.transform.localScale *= 2;
		animator.speed = 1.5f;
		yield return new WaitForSeconds(6f);
		agent.speed = 2.5f;
		gameObject.transform.localScale *= 0.5f;
		animator.speed = 1;
		poweredup = false;
	}

	//None of the following are implemented yet
	public override void Action1() { //Freeze all enemies around
		int layer_mask = LayerMask.GetMask("Enemy");
		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 8f, layer_mask);

		for (int i = 0; i < colliders.Length; i++) {
			EnemyUnit enemyUnit = colliders[i].GetComponent<EnemyUnit>();
			enemyUnit.TempSpeedChange();
		}
	}
	public override void Action2() { //Go supercharge
        if (!poweredup)
        {
			PowerUp();
        }
	}

	public override void Action3() {

	}

	public override void Action4() { }
	public override void Action5() {

		Harvest();
	
	}
}