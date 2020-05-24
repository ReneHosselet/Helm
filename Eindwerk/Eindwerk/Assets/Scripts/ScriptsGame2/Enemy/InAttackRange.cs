using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAttackRange : MonoBehaviour
{
    private Enemy enemyObj;

    // Start is called before the first frame update
    void Start()
    {
        enemyObj = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyObj.Attack();
        }
    }
}
