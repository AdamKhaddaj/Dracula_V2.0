using UnityEngine;

public class PlayerSupport : PlayerUnit
{

    [SerializeField] public float hoverspeed;

    private void Start()
    {
        base.Start();
        base.agent.angularSpeed = 0;
        hoverspeed = 0.75f;
    }
    public override void Action1()
    {
        Debug.Log("HEALER ACTIVATE");

        int layer_mask = LayerMask.GetMask("Player");
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 4f, layer_mask);

        for (int i = 0; i < colliders.Length; i++)
        {
            PlayerUnit playerUnit = colliders[i].GetComponent<PlayerUnit>();

            playerUnit.AddHealth(5);
            Debug.Log("THANKS!");
        }
    }

    public override void Action2()
    {
        Debug.Log("ACTIVATE THE SHIELDS");
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

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Vector3 hover = new Vector3(0, 0, 0);
        hover.y =  0.5f + Mathf.Sin(Time.time * hoverspeed) * 0.25f;
        transform.position = new Vector3(transform.position.x, transform.position.y + hover.y, transform.position.z);
        transform.Rotate(0f, 10f * Time.deltaTime, 0f);
    }
}
