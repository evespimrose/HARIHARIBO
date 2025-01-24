using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPartyMemberUI : MonoBehaviour
{
    [SerializeField] private Image classIcon;
    [SerializeField] private Image hpBar;
    private Player targetPlayer;

    public void Initialize(Player player, ClassType classType)
    {
        targetPlayer = player;
        // GameManager에서 직업 아이콘 가져오기
        if (GameManager.Instance.characterDataDic.TryGetValue(classType, out CharacterData characterData))
        {
            classIcon.sprite = characterData.headSprite;  // 또는 실제 사용하는 아이콘 변수명
        }
    }

    public void UpdateHP()
    {
        if (targetPlayer != null && targetPlayer.Stats != null)
        {
            float healthRatio = targetPlayer.Stats.currentHealth / targetPlayer.Stats.maxHealth;
            hpBar.fillAmount = healthRatio;
        }
    }
}
