using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wizard : MonoBehaviourPunCallbacks
{
    private GameObject target;

    public void FireBall()
    {
        target = transform.GetComponent<Unit>().target;

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject fireball = PhotonNetwork.Instantiate("FireBall", transform.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().target = target;
        }
    }
}
