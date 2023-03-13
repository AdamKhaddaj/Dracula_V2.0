using System.Collections.Generic;

using UnityEngine;

public class EnemyManager : MonoBehaviour {
    // singleton instance
    public static EnemyManager instance;

    private Dictionary<int, EnemyUnit> units;

    private void Awake() {
        // singleton assign
        instance = this;

        units = new Dictionary<int, EnemyUnit>();
    }

    public void AddUnit(EnemyUnit unit) {
        units.Add(unit.GetID(), unit);
    }

    public void RemoveUnit(int id) {
        units.Remove(id);
    }

    public EnemyUnit GetUnit(int id)
    {
        //need to add handler for if id no longer exists
        return units[id];
    }
}