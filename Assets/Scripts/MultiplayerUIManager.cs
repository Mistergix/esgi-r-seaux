using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerUIManager : MonoBehaviourPunCallbacks
{
    public string menuScene;
    public GameObject vsScreen;

    public Text localPseudo;
    public Text enemyPseudo;

    private bool _master;
    
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(menuScene);
            return;
        }

        PhotonNetwork.SerializationRate = 3;
        PhotonNetwork.SendRate = 30;
        _master = PhotonNetwork.IsMasterClient;
        
        localPseudo.text = PhotonNetwork.LocalPlayer.NickName;
        enemyPseudo.text = PhotonNetwork.LocalPlayer.GetNext().NickName;

        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (_master)
            {
                photonView.RPC("LaunchGame", RpcTarget.All);
            }
        });
    }

    private bool _leftRoom;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(_leftRoom) {return;}
        base.OnPlayerLeftRoom(otherPlayer);
        try
        {
            PhotonNetwork.LeaveRoom();
            _leftRoom = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
        
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (!GameEnded())
        {
            LoadMenu();
            _leftRoom = true;
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    private static bool GameEnded()
    {
        var enemyBase = EnemyBase.LocalEnemyBase.GetComponent<EnemyBase>();
        return enemyBase.gameEnded;
    }

    [PunRPC]
    private void LaunchGame()
    {
        var bases = FindObjectsOfType<EnemyBase>().ToList();
        foreach (var enemyBase in bases)
        {
            enemyBase.Initialize();
        }

        DOVirtual.DelayedCall(3, () =>
        {
            vsScreen.SetActive(false);
        });
    }
}
