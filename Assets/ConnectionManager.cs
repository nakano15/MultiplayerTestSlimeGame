using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] Transform HostButton;
    [SerializeField] Transform ClientButton;
    [SerializeField] Transform StopButton;
    UnityTransport ConnectionInfos;

    private void Awake()
    {
        ConnectionInfos = GetComponent<UnityTransport>();
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartHost();
        //NetworkManager.Singleton.LocalClient.PlayerObject = PlayerScript.PlayerCharacter.GetComponent<NetworkObject>();
        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);
        StopButton.gameObject.SetActive(true);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        //NetworkManager.Singleton.LocalClient.PlayerObject = PlayerScript.PlayerCharacter.GetComponent<NetworkObject>();
        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);
        StopButton.gameObject.SetActive(true);
    }

    public void CloseConnection()
    {
        NetworkManager.Singleton.Shutdown();
        HostButton.gameObject.SetActive(true);
        ClientButton.gameObject.SetActive(true);
        StopButton.gameObject.SetActive(false);
    }
}
