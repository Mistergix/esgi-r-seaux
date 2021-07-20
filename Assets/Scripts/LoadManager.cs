using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviourPunCallbacks
{
    public string menuScene;
    public string gameScene;
    public Transform castleSpawn;

    [SerializeField] private GameObject enemyBasePrefab;

    private bool _master;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(menuScene);
            return;
        }

        _master = PhotonNetwork.IsMasterClient;
        
        if (EnemyBase.LocalEnemyBase != null) return;
        
        InstantiateCastle();
    }

    private void InstantiateCastle()
    {
        var pos = castleSpawn.position;
        var angle = 0;
        if (!_master)
        {
            angle = 180;
            pos.z *= -1;
        }

        var rot = Quaternion.Euler(Vector3.up * angle);
        var go = PhotonNetwork.Instantiate(enemyBasePrefab.name, pos, rot);
        var castleName = _master ? "MASTER" : "CLIENT";
        go.name = $"Castle {castleName}";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (_master)
        {
            PhotonNetwork.LoadLevel(gameScene);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(menuScene);
    }
    
    public void LeaveRoom(){
        Debug.Log("LEAVE ROOM");
        PhotonNetwork.LeaveRoom();
    }
}
