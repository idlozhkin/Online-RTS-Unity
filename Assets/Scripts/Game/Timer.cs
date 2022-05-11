using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Timer : MonoBehaviourPunCallbacks
{
    public Text timerText;
    public float time;
    //private PhotonView photonView;
    private float startTime;
    private bool allReady = false;
    [SerializeField] private GameInfo gameInfo;

    void Start()
    {
        // photonView = GetComponent<PhotonView>();
        photonView.RPC("SendData", RpcTarget.OthersBuffered, true);
    }
    void Update()
    {
        if (!allReady || gameInfo.endGame) return;

        time = Time.time - startTime;

        string minutes = ((int)time / 60).ToString();
        string seconds = (time % 60).ToString("f2");

        timerText.text = minutes + ":" + seconds;
    }

    [PunRPC]
    private void SendData(bool ready)
    {
        if (PhotonNetwork.IsConnectedAndReady && ready)
        {
            StartTimer();
            allReady = true;
        }
    }

    void StartTimer()
    {
        startTime = Time.time;
    }

    // public void OnPhotonPlayerConnected()
    // {
    //     // photonView.RPC("SendData", RpcTarget.OthersBuffered, true);
    //     // Debug.Log(PhotonNetwork.PlayerList.Length);
    // }

}
