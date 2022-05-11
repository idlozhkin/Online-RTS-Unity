using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DogKnight : MonoBehaviourPunCallbacks
{
    private Spawner spawner;
    private Unit unit;
    private int dogKnightID = 2;
    private float hpToRecovery;
    private float maxHP;

    void Start()
    {
        unit = transform.GetComponent<Unit>();
        spawner = unit.mainBuilding.GetComponent<Spawner>();
        maxHP = unit.lvlHP[spawner.lvls[dogKnightID]];
        hpToRecovery = 0.1f * maxHP;

        StartCoroutine(Recovery(hpToRecovery));
    }

    private IEnumerator Recovery(float hp)
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            unit.AddHP(hp);
        }
    }
}
