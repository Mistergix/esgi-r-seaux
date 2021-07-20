using System;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class GoldManager : MonoBehaviour
    {
        public float goldTimer;
        private float _gold;
        public float maxGold;
        public float incrementGold;

        private Tween _addGoldTween;

        public float Gold => _gold;

        public void LaunchAddGold(Action callback)
        {
            _addGoldTween = DOVirtual.DelayedCall(goldTimer, () =>
            {
                if(_gold >= maxGold)
                    return;
		
                if(_gold + incrementGold > maxGold){
                    _gold = maxGold;
                }
                else{
                    _gold += incrementGold;
                }

                callback();
            })
                .SetLoops(-1);
        }

        private void Awake()
        {
            _gold = 0;
        }

        public void DecreaseGold(float unitPrefabCost)
        {
            _gold = Mathf.Clamp(_gold - unitPrefabCost, 0, maxGold);
        }
    }
}