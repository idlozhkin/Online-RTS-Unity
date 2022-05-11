using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lich : MonoBehaviourPunCallbacks
{
    private Spawner spawner;
    private int lichID = 1;

    void Start()
    {
        spawner = transform.GetComponent<Unit>().mainBuilding.GetComponent<Spawner>();
    }

    void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject skeleton = PhotonNetwork.Instantiate("Skeleton", transform.position, Quaternion.identity);
            skeleton.tag = transform.tag;
            skeleton.GetComponent<Unit>().initStats(spawner.lvls[lichID]);
        }
    }
}
