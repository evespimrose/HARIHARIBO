using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventEffects : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class EffectInfo
    {
        public string effectName;          // 이펙트 구분용 이름
        public GameObject Effect;          // 이펙트 프리팹
        public Transform StartPositionRotation;  // 이펙트 생성 위치
        public float DestroyAfter = 1.5f;       // 제거 시간
        public bool UseLocalPosition = true;     // 부모 기준 위치 사용
    }
    public EffectInfo[] Effects;

    // 스킬 상태에서 호출할 메서드
    public void PlayEffect(int effectNumber)
    {
        if (Effects == null || Effects.Length <= effectNumber)
        {
            Debug.LogError("Incorrect effect number or effect is null");
            return;
        }

        var effect = Effects[effectNumber];
        var instance = Instantiate(effect.Effect,
                                 effect.StartPositionRotation.position,
                                 effect.StartPositionRotation.rotation);

        if (effect.UseLocalPosition)
        {
            instance.transform.parent = effect.StartPositionRotation;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }

        Destroy(instance, effect.DestroyAfter);
    }
}
