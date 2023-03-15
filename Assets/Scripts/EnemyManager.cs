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

    public void RemoveUnit(int id)
    {

        Destroy(units[id].gameObject);

        if (!units.Remove(id))
        {
            return;
        }
    }

    public EnemyUnit GetUnit(int id)
    {
        EnemyUnit e;
        units.TryGetValue(id, out e);
        return e;
    }
    
}