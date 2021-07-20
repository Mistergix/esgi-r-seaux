using System;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace DefaultNamespace
{
    public class Unit : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] private float stoppingDistance;
        public float cost;
        public bool local;
        public float minAttackDistance;
        public new Renderer renderer;
        public float damage;
        public float speed = 5;
        
        private bool _master;
        
        private Vector3 _targetPosition;
        private Vector3 _position;
        private Vector3 _lastPosition;
        private Quaternion _rotation;

        private string _attackTag;
        private string _baseAttackTag;

        private bool _canMove;
        private bool _attacking;
        
        private float  _clientSpeed;

        private bool _clientStarted;
        
        private float _lastSerializeTime;

        public void Init()
        {
            _master = PhotonNetwork.IsMasterClient;

            if (_master)
            {
               photonView.RPC("SetLocalState", RpcTarget.Others, !local);
                SetColors();
                SetTags();
            }
            else
            {
                _position = transform.position;
                _lastPosition = transform.position;

                DOVirtual.DelayedCall(1, StartClient);
            }
        }

        private void StartClient()
        {
            _clientStarted = true;
        }

        private void SetTags()
        {
            var enemyBase = FindObjectsOfType<EnemyBase>().FirstOrDefault();
            Debug.Assert(enemyBase != null, nameof(enemyBase) + " != null");
            
            if(local){
                _attackTag = _master ? enemyBase.clientUnit : enemyBase.masterClientUnit;
                _baseAttackTag = _master ? enemyBase.clientBase : enemyBase.masterClientBase;
			
                gameObject.tag = _master ? enemyBase.masterClientUnit : enemyBase.clientUnit;
            }
            else{
                _attackTag = _master ? enemyBase.masterClientUnit : enemyBase.clientUnit;
                _baseAttackTag = _master ? enemyBase.masterClientBase : enemyBase.clientBase;
			
                gameObject.tag = _master ? enemyBase.clientUnit : enemyBase.masterClientUnit;
            }
        }

        private void SetColors()
        {
            var enemyBase = FindObjectsOfType<EnemyBase>().FirstOrDefault();
            Debug.Assert(enemyBase != null, nameof(enemyBase) + " != null");
            Color color;

            if (_master)
            {
                color = local ? enemyBase.masterClientColor : enemyBase.clientColor;
            }
            else
            {
                color = local ? enemyBase.clientColor : enemyBase.masterClientColor;
            }

            renderer.material.color = color;
        }
        
        [PunRPC]
        void SetLocalState(bool local){
            this.local = local;
		
            SetColors();
            SetTags();
        }

        private void Update()
        {
            if (_master || !_clientStarted)
            {
                UpdateUnit();
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _position, _clientSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, 4f * Time.deltaTime);
            }
        }

        private void UpdateUnit()
        {
            var target = GetTargetObject(_attackTag);
            if (target == null || TooFarFromTarget(target))
            {
                target = GoToEnemyBase();
            }
            else
            {
                AttackTarget(target, go =>
                {
                    go.GetComponent<Unit>().Die();
                    Die();
                });
            }

            TurnTowards(target.transform.position);

            if (_canMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
            }
        }

        private void Die()
        {
            var enemyBase = FindObjectsOfType<EnemyBase>().FirstOrDefault();
            Debug.Assert(enemyBase != null, nameof(enemyBase) + " != null");
            var myBaseTag = local ? enemyBase.masterClientBase : enemyBase.clientBase;
            var myBase = GameObject.FindGameObjectWithTag(myBaseTag).GetComponentInParent<EnemyBase>();

            myBase.DestroyUnit(this);
        }

        private GameObject GoToEnemyBase()
        {
            var target = GetTargetObject(_baseAttackTag);
            AttackTarget(target, go =>
            {
                var enemyBase = target.GetComponentInParent<EnemyBase>();
                enemyBase.TakeDamage(damage);
            });
            return target;
        }

        private void TurnTowards(Vector3 transformPosition)
        {
            transform.LookAt(transformPosition);
        }

        private bool TooFarFromTarget(GameObject target)
        {
            return Vector3.Distance(transform.position, target.transform.position) > minAttackDistance;
        }

        private void AttackTarget(GameObject target, Action<GameObject> onCloseEnough)
        {
            _targetPosition = target.transform.position;
            _canMove = true;
            
            if(Vector3.Distance(_targetPosition, transform.position) < stoppingDistance)
            {
                onCloseEnough(target);
                _attacking = true;
                AttackAnim();
                _canMove = false;
            }
            else
            {
                _attacking = false;
                MoveAnim();
            }
        }

        private void MoveAnim()
        {
        }

        private void AttackAnim()
        {
        }

        private GameObject GetTargetObject(string targetTag){
            if (string.IsNullOrEmpty(targetTag))
            {
                return null;
            }
		
            var possibleTargets = GameObject.FindGameObjectsWithTag(targetTag);
		
            GameObject target = null;
            var smallestDistance = Mathf.Infinity;
		
            foreach (var possibleTarget in possibleTargets)
            {
                var dist = Vector3.Distance(possibleTarget.transform.position, transform.position);

                if (dist >= smallestDistance) continue;
                smallestDistance = dist;
                target = possibleTarget;
            }
		
            return target;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (_master)
                {
                    stream.SendNext(transform.position);
                    stream.SendNext(transform.rotation);
                    stream.SendNext(_attacking);
                }
            }
            else
            {
                if (!_master)
                {
                    _position = (Vector3) stream.ReceiveNext();
                    _rotation = (Quaternion) stream.ReceiveNext();
                    _attacking = (bool) stream.ReceiveNext();

                    UpdateSpeed();
                    
                    _lastSerializeTime = Time.time;
                    _lastPosition = _position;
                }
            }
        }

        private void UpdateSpeed()
        {
            if(_lastSerializeTime == 0){return;}

            var dist = Vector3.Distance(_position, _lastPosition);
            var dt = Time.time - _lastSerializeTime;
            _clientSpeed = dist / dt;
        }
    }
}