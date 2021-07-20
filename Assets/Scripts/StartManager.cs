using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviourPunCallbacks
{
    public GameObject pseudoScreen;
    public InputField pseudoInputField;
    public string loadingScene;

    private bool _isConnecting;
    string gameVersion = "1";
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _isConnecting = false;
    }

    public void LaunchGame()
    {
        var pseudo = pseudoInputField.text;
        if (pseudo == "")
        {
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = pseudo;

        TryConnect();
    }

    private void TryConnect()
    {
        _isConnecting = true;
        pseudoScreen.SetActive(false);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (_isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 2}, null);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        _isConnecting = false;
        pseudoScreen.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        // si 2ème joueur, load auto avec PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(loadingScene);
        }
    }
}
