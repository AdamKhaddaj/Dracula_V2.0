using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegePylon : PlayerUnit
{

	private LineRenderer line;

	public PlayerPylon playerpylon;

	private Vector3 endpoint;
	private int barrierID;

	void Start()
	{
		base.Start();
		line = GetComponent<LineRenderer>();

		int layermask = LayerMask.GetMask("Barrier");
		Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 13f, layermask);

		endpoint = colliders[0].gameObject.transform.position;
		barrierID = colliders[0].gameObject.GetComponent<EnemyUnit>().GetID();

		line.SetPosition(0, transform.position);
		line.SetPosition(1, endpoint);

		Invoke("DamageBarrier", 2);

	}

	private void DamageBarrier()
    {
		if (EnemyManager.instance.GetUnit(barrierID) != null) 
		{
			EnemyManager.instance.GetUnit(barrierID).RemoveHealth(2);
			Invoke("DamageBarrier", 3);
		}
        else
        {
			PlayerManager.instance.RemoveUnit(GetID());
		}

	}

	public override void Action1()
	{

	}

	public override void Action2()
	{

	}

	public override void Action3()
	{
	}

	public override void Action4()
	{ 
	}

	public override void Action5()
	{
	}

}
