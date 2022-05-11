using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventSystemController : MonoBehaviourPunCallbacks
{
    public GameObject panel0;
    public GameObject panel1;
    public GameObject gold1;
    public GameObject gold2;
    void Start()
    {
        panel0.SetActive(false);
        panel1.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            gold2.SetActive(false);
        }
        else
        {
            gold1.SetActive(false);
        }
    }

    public void ChooseBase(int id)
    {
        if (id == 0 && PhotonNetwork.IsMasterClient)
        {
            panel0.SetActive(true);

        }
        if (id == 1 && !PhotonNetwork.IsMasterClient)
        {
            panel1.SetActive(true);
        }
    }

    public void UnChoose()
    {
        panel0.SetActive(false);
        panel1.SetActive(false);
    }


    public void BackToLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }
}
