using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float health;
    private DungeonCreator dC;

    // Start is called before the first frame update
    void Start()
    {
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 0)
        {
            dC.OnDeath(gameObject);
        }
    }
}
