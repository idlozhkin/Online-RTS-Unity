using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Chest : MonoBehaviourPunCallbacks
{
    private Spawner spawner;
    private int chestID = 0;
    private int[] goldBonus = { 0, 1, 2, 3, 4 };

    void Start()
    {
        spawner = transform.GetComponent<Unit>().mainBuilding.GetComponent<Spawner>();
    }

    void OnDestroy()
    {
        spawner.AddGold(goldBonus[spawner.lvls[chestID]]);
    }
}
