using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventEffects : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class EffectInfo
    {
        public string effectName;          // ����Ʈ ���п� �̸�
        public GameObject Effect;          // ����Ʈ ������
        public Transform StartPositionRotation;  // ����Ʈ ���� ��ġ
        public float DestroyAfter = 1.5f;       // ���� �ð�
        public bool UseLocalPosition = true;     // �θ� ���� ��ġ ���
    }
    public EffectInfo[] Effects;

    // ��ų ���¿��� ȣ���� �޼���
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
