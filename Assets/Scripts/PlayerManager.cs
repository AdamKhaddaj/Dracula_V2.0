using System.Collections.Generic;

using UnityEngine;

public class PlayerManager : MonoBehaviour {
    // singleton instance
    public static PlayerManager instance;

    [Header("Unit Prefabs")]
    [SerializeField] private GameObject attack = null;
    [SerializeField] private GameObject support = null;
    [SerializeField] private GameObject pylon = null;
    [SerializeField] private GameObject die = null;

    // resources
    private int crystals;
    private int stardust;
    private int divineBlood;

    private Dictionary<int, PlayerUnit> units;
    public List<int> selectedUnits;

    private void Awake() {
        // singleton assign
        instance = this;

        // resources
        crystals = 50;
        stardust = 0;
        divineBlood = 0;

        units = new Dictionary<int, PlayerUnit>();
        selectedUnits = new List<int>();
    }

    private void Start()
    {
        InvokeRepeating("AddCrystalsOne", 0.0f, 1.0f);
    }

    private void Update() {
        // temporary deselect all shortcut

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ClearSelectedUnits();
        }

        // temporary movement handler
        if (Input.GetMouseButtonDown(1)) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
                if (hit.transform.name == "Terrain") {
                    MoveSelectedUnits(hit.point);
                }
            }
        }

        // temporary action handling

        // custom action 1
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            for (int i = 0; i < selectedUnits.Count; i++) {
                units[selectedUnits[i]].Action1();
            }
        }

        // custom action 2
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            for (int i = 0; i < selectedUnits.Count; i++) {
                units[selectedUnits[i]].Action2();
            }
        }

        // custom action 3
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            for (int i = 0; i < selectedUnits.Count; i++) {
                units[selectedUnits[i]].Action3();
            }
        }

        // level up action
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            for (int i = 0; i < selectedUnits.Count; i++) {
                units[selectedUnits[i]].LevelUp();
            }
        }

        // harvest action
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            for (int i = 0; i < selectedUnits.Count; i++) {
                units[selectedUnits[i]].Harvest();

                // unit is deleted so move iteration back a step
                i--;
            }
        }
    }

    //RESOURCE HANDLING

    public int GetCrystals() {
        return crystals;
    }

    public void AddCrystals(int x) {
        crystals += x;
        UIManager.instance.UpdateCrystals(crystals);
    }

    public void AddCrystalsOne()
    {
        crystals ++;
        UIManager.instance.UpdateCrystals(crystals);
    }

    public void RemoveCrystals(int x) {
        crystals = Mathf.Max(crystals - x, 0);
    }

    public int GetStardust() {
        return stardust;
    }

    public void AddStardust(int x) {
        stardust += x;
        UIManager.instance.UpdateCrystals(stardust);
    }

    public void RemoveStardust(int x) {
        stardust = Mathf.Max(stardust - x, 0);
    }

    public int GetDivineBlood() {
        return divineBlood;
    }

    public void AddDivineBlood(int x) {
        divineBlood += x;
        UIManager.instance.UpdateCrystals(divineBlood);
    }

    public void RemoveDivineBlood(int x) {
        divineBlood = Mathf.Max(divineBlood - x, 0);
    }

    public void CreateWarrior(Vector3 pos)
    {
        pos.y = 0.5f;
        pos.x += 1.5f;
        GameObject g = Instantiate(attack, pos, Quaternion.identity);
    }

    //UNIT LIST HANDLING

    public void AddUnit(PlayerUnit unit) {
        units.Add(unit.GetID(), unit);
    }

    public void RemoveUnit(int id) {
        if (!units.Remove(id)) {
            return;
        }

        // if a unit dies while selected then remove from selected units
        RemoveSelectedUnit(id);
    }

    // SELECTION HANDLING

    public void AddSelectedUnit(int id) {
        units[id].Select();
        selectedUnits.Add(id);
    }

    public void RemoveSelectedUnit(int id) {
        units[id].Deselect();
        selectedUnits.Remove(id);
    }

    public void ClearSelectedUnits() {
        for (int i = 0; i < selectedUnits.Count; i++) {
            units[selectedUnits[i]].Deselect();
        }

        selectedUnits.Clear();
    }

    public void MoveSelectedUnits(Vector3 destination) {
        // set destination of all selected units
        for (int i = 0; i < selectedUnits.Count; i++) {
            units[selectedUnits[i]].SetDestination(destination);
        }
    }


    
}