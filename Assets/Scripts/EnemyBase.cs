using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Drawing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class EnemyBase : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject LocalEnemyBase;
    public bool gameEnded;
    public float health;
    public Transform decorParent;
    public GameObject dieFXPrefab;
    
    public string clientBase = "ClientBase";
    public string masterClientBase = "MasterClientBase";
    public string clientUnit = "ClientUnit";
    public string masterClientUnit = "MasterClientUnit";
    
    public Color clientColor;
    public Color masterClientColor;
    public Color loseColor;
    public Color victoryColor;

    public Unit unitPrefab;
    public SpawnFloor spawnFloor;

    private bool _master;

    public GoldManager goldManager;
    
    float _masterHealthStart;
    float _clientHealthStart;
    float _masterHealth;
    float _clientHealth;

    private float _battleDuration;
    private int _masterKills;
    private int _clientKills;
    private float _healthBarSpeedMaster;
    private float _healthBarSpeedClient;
    private float _lastSerializeTime;
    private float _lastClientHealth;
    private float _lastMasterHealth;

    private bool _init;
    private Ray _currentRay;

    private bool IsLocalBase => photonView.IsMine;
    private bool BelongsToMasterClient => IsLocalBase && _master || (!IsLocalBase && !_master);

    private CameraController _cameraController;
    
    private void Awake()
    {
        _cameraController = GetComponentInChildren<CameraController>();
        if (IsLocalBase)
        {
            LocalEnemyBase = gameObject;
        }
        else
        {
            _cameraController.gameObject.SetActive(false);
        }
        
        _master = PhotonNetwork.IsMasterClient;

        foreach (Transform t in transform)
        {
            var go = t.gameObject;
            if(go.TryGetComponent(out Camera _)){continue;}

            if (_master)
            {
                go.tag = IsLocalBase ? masterClientBase : clientBase;
            }
            else
            {
                go.tag = IsLocalBase ? clientBase : masterClientBase;
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (gameEnded || ! MultiplayerUI.Instance || ! _init)
        {
            return;
        }

        _battleDuration += Time.deltaTime;

        if (IsLocalBase)
        {
            UpdateBase();
        }

        if (BelongsToMasterClient)
        {
            UIUpdate();
        }
        
        Draw.ingame.Ray(_currentRay, 50, Color.red);
    }

    private void UIUpdate()
    {
        if (_master)
        {
            CollectBaseHealth(false);
        }
        else
        {
            _masterHealth -= Time.deltaTime * _healthBarSpeedMaster;
            _clientHealth -= Time.deltaTime * _healthBarSpeedClient;
        }

        MultiplayerUI.Instance.UpdateUI(_master, _masterHealth, _clientHealth, _masterHealthStart, _clientHealthStart);
    }

    
    private void UpdateBase()
    {
        if(Input.GetMouseButtonDown(0)){
            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        if (!HasEnoughGold)
        {
            return;
        }
        
        var ray = _cameraController.Camera.ScreenPointToRay(Input.mousePosition);
        _currentRay = ray;
        var hit = Physics.Raycast(ray, out var raycastHit);

        if (!hit) return;
        
        var floor = raycastHit.collider.gameObject.GetComponentInParent<SpawnFloor>();
        
        if (!floor || !CanSpawn(floor)) return;

        if (_master)
        {
            SpawnUnit(raycastHit.point, true);
        }
        else
        {
            photonView.RPC("ClientSpawn", RpcTarget.MasterClient, raycastHit.point);
        }

        goldManager.DecreaseGold(unitPrefab.cost);
        UpdateGoldUI();
    }
    
    [PunRPC]
    void ClientSpawn(Vector3 position){
        SpawnUnit(position, false);
    }

    private void SpawnUnit(Vector3 raycastHitPoint, bool masterClient)
    {
        var rotation = transform.rotation * Quaternion.Euler(0, -90, 0);
        var unitName = unitPrefab.gameObject.name;
        var unitGo = PhotonNetwork.Instantiate(unitName, raycastHitPoint, rotation, 0);
        var unit = unitGo.GetComponent<Unit>();
        if (masterClient)
        {
            unit.local = true;
        }
        
        unit.Init();
    }

    private bool CanSpawn(SpawnFloor spawn)
    {
        return this.spawnFloor == spawn;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(!_init){return;}
        if (stream.IsWriting)
        {
            if (IsLocalBase && _master)
            {
                CollectBaseHealth(false);
                stream.SendNext(_masterHealth);
                stream.SendNext(_clientHealth);
               // Debug.Log($"Stream Writing {_masterHealth}, {_clientHealth} {name}");
            }
        }
        else
        {
            if (!IsLocalBase && !_master)
            {
                var masterHealth = (float) stream.ReceiveNext();
                var clientHealth = (float) stream.ReceiveNext();
                
               // Debug.Log($"Stream Reading {masterHealth}, {clientHealth} {name}");

                if (HealthBarNotInitialized)
                {
                   // Debug.Log($"Init HealthBar {name}");
                    _masterHealthStart = masterHealth;
                    _clientHealthStart = clientHealth;

                    _masterHealth = masterHealth;
                    _clientHealth = clientHealth;
                }
                else
                {
                    if(_lastSerializeTime == 0)
                        return;

                    _healthBarSpeedMaster = EstimateSpeed(masterHealth, _lastMasterHealth);
                    _healthBarSpeedClient = EstimateSpeed(clientHealth, _lastClientHealth);
                  //  Debug.Log($"Estimate speed {_healthbarSpeedMaster}, {_healthbarSpeedClient} {name}");
                }
                
                _lastSerializeTime = Time.time;
                _lastClientHealth = clientHealth;
                _lastMasterHealth = masterHealth;
                
             //   Debug.Log($"LAST {_lastClientHealth}, {_lastMasterHealth} {name}");
            }
        }
    }

    private float EstimateSpeed(float currentHealth, float lastHealth)
    {
        var diff = lastHealth - currentHealth;
        var time = Time.time - _lastSerializeTime;
        return diff / time;
    }

    private bool HealthBarNotInitialized => _masterHealthStart == 0;

    public void Initialize()
    {
        if (_master && IsLocalBase)
        {
            CollectBaseHealth(true);
        }

        MultiplayerUI.Instance.unitCost.text = "Unit cost " + unitPrefab.cost;

        SetColors();

        if (IsLocalBase)
        {
            goldManager.LaunchAddGold(UpdateGoldUI);
			
            MultiplayerUI.Instance.boxSelectTarget.SetActive(false);
        }

        spawnFloor.name = $"SpawnFloor {name}";

        _init = true;
    }

    
    private void UpdateGoldUI()
    {
        MultiplayerUI.Instance.UpdateGoldText((int) goldManager.Gold);
    }

    private bool HasEnoughGold => goldManager.Gold >= unitPrefab.cost;

    private void SetColors()
    {
        MultiplayerUI.Instance.SetHealthBarColors(_master, masterClientColor, clientColor);
    }
    
    private void CollectBaseHealth(bool initialization)
    {
        var enemyBases = FindObjectsOfType<EnemyBase>();

        foreach (var enemyBase in enemyBases)
        {
            var totalHealth = enemyBase.TotalHealth();
			
            if(enemyBase == this){
                if (initialization)
                {
                    _masterHealthStart = totalHealth;
                }
				
                _masterHealth = totalHealth;
            }
            else{
                if (initialization)
                {
                    _clientHealthStart = totalHealth;
                }
				
                _clientHealth = totalHealth;
            }

            if (totalHealth <= 0)
            {
                EndBattle(enemyBase != this);
            }
        }
    }
    
    void EndBattle(bool masterWon){
        EndBattleLocally(_battleDuration, masterWon, _masterKills, _clientKills);
        photonView.RPC("EndBattleClient", RpcTarget.Others, _battleDuration, masterWon, _masterKills, _clientKills);
        photonView.RPC("Disconnect", RpcTarget.All);
    }
    
    [PunRPC]
    private void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    [PunRPC]
    private void EndBattleClient(float battleDuration, bool masterWon, int masterKills, int clientKills)
    {
        EndBattleLocally(battleDuration, masterWon, masterKills, clientKills);
    }

    private void EndBattleLocally(float duration, bool masterWon, int masterKills, int clientKills)
    {
        var wonBattle = masterWon && _master || !masterWon && !_master;
        var ui = MultiplayerUI.Instance;
        var color = wonBattle ? victoryColor : loseColor;
        var text = wonBattle ? "VICTORY" : "DEFEAT";
        var killedEnemies = _master ? masterKills : clientKills;
        var deadAllies = _master ? clientKills : masterKills;
        var time = FormatTime(duration);
        ui.SetVictory(color, text, killedEnemies, deadAllies, time);

        var enemyBases = FindObjectsOfType<EnemyBase>();
        foreach (var enemyBase in enemyBases)
        {
            enemyBase.gameEnded = true;
        }
    }

    private static string FormatTime(float duration)
    {
        return ((int)(duration/60) < 10 ? "0" : "") + "" + (int)(duration/60) + (duration % 60 < 10 ? ":0" : ":") + (int)(duration % 60);
    }

    private float TotalHealth()
    {
        return health;
    }

    public void DestroyUnit(Unit unit)
    {
        var photon = unit.photonView;
        if (photon == null || !photon.IsMine)
        {
            return;
        }

        PhotonNetwork.Instantiate(dieFXPrefab.name, unit.transform.position + Vector3.up * 2, unit.transform.rotation,
            0);

        DeadUnit(unit);
        PhotonNetwork.Destroy(photon);
    }

    private void DeadUnit(Unit unit)
    {
        var unitTag = unit.gameObject.tag;
        if (unitTag == clientUnit)
        {
            var localBase = LocalEnemyBase.GetComponent<EnemyBase>();
            localBase._masterKills++;
        }
        else
        {
            _clientKills++;
        }
    }

    public void TakeDamage(float damage)
    {
        if(_destroyed) {return;}
        health -= damage * Time.deltaTime;
        if (health <= 0)
        {
            _destroyed = true;
            photonView.RPC("DestroyBase", RpcTarget.All);
        }
    }

    private bool _destroyed;
    
    [PunRPC]
    void DestroyBase()
    {
        if(_destroyed) {return;}
        _destroyed = true;
        Destroy(decorParent.gameObject);
    }
}
