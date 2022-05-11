using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class ConnectionStatus : MonoBehaviour
{
    private string connectionStatusMessage = "Connection Status: ";
    public Text ConnectionStatusText;

    public void Update()
    {
        ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
    }
}
