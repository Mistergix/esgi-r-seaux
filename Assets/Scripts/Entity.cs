using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Entity : Photon.Pun.MonoBehaviourPunCallbacks
{
    public float health;
    public float damage;
    public float minAttackDistance;
    public float enemyBaseStopDistance;

    public Image healthBar;
    public Color masterClientColor;
    public Color clientColor;
    
    public string clientCastle;
    public string masterClientCastle;
    public string clientUnit;
    public string masterClientUnit;
    
    public string unitType;
    public GameObject selectedObject;
    
    public bool clientStarted;
    
    string attackTag;
    string castleAttackTag;
	
    float startHealth;
    float defaultStoppingDistance;
	
    float lastSerializeTime;
    Vector3 lastPosition;
    float clientSpeed;
    
    Transform enemyCastle;
    NavMeshAgent agent;
    
    Vector3 position;
    Quaternion rotation;
    
    bool master;
    
    public bool selected { get; set; }
    public bool local { get; set; }
    public Vector3 clickedPosition { get; set; }

    private void Start()
    {
	    agent = GetComponent<NavMeshAgent>();
	    startHealth = health;
	    
	    if(master){			
		    defaultStoppingDistance = agent.stoppingDistance;
		    SetUnitColor();
		    SetTags();
	    }
	    else{
		    position = transform.position;
		    lastPosition = transform.position;
			
		    //StartCoroutine(StartClient());
	    }
    }

    private void SetUnitColor(){
	    var color = Color.white;
		
	    if(master){
		    color = local ? masterClientColor : clientColor;
	    }
	    else{
		    color = local ? clientColor : masterClientColor;
	    }
		
	    healthBar.color = color;
    }

    private void SetTags(){
	    if(local){
		    attackTag = master ? clientUnit : masterClientUnit;
		    castleAttackTag = master ? clientCastle : masterClientCastle;
			
		    gameObject.tag = master ? masterClientUnit : clientUnit;
	    }
	    else{
		    attackTag = master ? masterClientUnit : clientUnit;
		    castleAttackTag = master ? masterClientCastle : clientCastle;
			
		    gameObject.tag = master ? clientUnit : masterClientUnit;
	    }
    }
}
