using UnityEngine;

public abstract class EnemyUnit : Unit
{
    [SerializeField] public EnemyUnitBlueprint blueprint = null;

    protected Vector3 destination;

    protected new Rigidbody rigidbody;

    private void Awake()
    {
        // unit setup
        Setup(blueprint.health);

        rigidbody = GetComponent<Rigidbody>();

    }

    protected void Start()
    {
        // temporary unit adding
        EnemyManager.instance.AddUnit(this);
    }
}