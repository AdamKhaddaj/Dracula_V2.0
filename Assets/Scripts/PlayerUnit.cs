using UnityEngine;

public abstract class PlayerUnit : Unit
{
    [SerializeField] public PlayerUnitBlueprint blueprint = null;

    private bool selected;

    protected Vector3 destination;

    protected new Rigidbody rigidbody;

    private void Awake()
    {
        // unit setup
        Setup(blueprint.health);

        // deselected by default
        Deselect();

        destination = transform.position;

        rigidbody = GetComponent<Rigidbody>();

    }

    protected void Start()
    {
        // temporary unit adding
        PlayerManager.instance.AddUnit(this);
    }

    protected void Update()
    {
        // movement handling
        // only consider x and z positions

        if (blueprint.movable && Vector3.Distance(transform.position, destination) > 1f)
        {
            Vector3 direction = destination - transform.position;
            direction.y = 0;
            direction.Normalize();

            rigidbody.velocity = direction * blueprint.moveSpeed;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    private void OnMouseOver()
    {
        /*
        // temporary selection handling 
        if (Input.GetMouseButtonDown(0))
        {
            if (selected)
            {
                PlayerManager.instance.RemoveSelectedUnit(GetID());
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    PlayerManager.instance.ClearSelectedUnits();
                }

                PlayerManager.instance.AddSelectedUnit(GetID());
            }
        }
        */
    }

    public void Select()
    {
        selected = true;
        GetComponent<Renderer>().material.color = blueprint.color * Color.white * GetLevel();
    }

    public void Deselect()
    {
        selected = false;
        GetComponent<Renderer>().material.color = blueprint.color * Color.gray * GetLevel();
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    public abstract void Action1();
    public abstract void Action2();
    public abstract void Action3();

    // level up unit
    public new bool LevelUp()
    {
        if (PlayerManager.instance.GetCrystals() >= blueprint.levelUpCost)
        {
            PlayerManager.instance.RemoveCrystals(blueprint.levelUpCost);

            base.LevelUp();

            Debug.Log("LEVEL UP!");

            return true;
        }

        return false;
    }

    // harvest unit for crystals
    public void Harvest()
    {
        PlayerManager.instance.RemoveUnit(GetID());
        PlayerManager.instance.AddCrystals(blueprint.cost);

        Destroy(gameObject);
    }
}