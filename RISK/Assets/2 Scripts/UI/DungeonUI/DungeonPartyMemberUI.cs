using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPartyMemberUI : MonoBehaviour
{
    [SerializeField] private Image classIcon;
    [SerializeField] private Image hpBar;
    private Player targetPlayer;

    public Player Player => targetPlayer;

    public void Initialize(Player player)  // ClassType 파라미터 제거
    {
        targetPlayer = player;
        if (player != null)
        {
            UpdateClassIcon(player.ClassType);  // Player에서 직접 ClassType 가져오기
            UpdateLocalHP();
        }
    }

    public void UpdateClassIcon(ClassType classType)
    {
        if (GameManager.Instance.characterDataDic.TryGetValue(classType, out CharacterData characterData))
        {
            classIcon.sprite = characterData.headSprite;
        }
    }

    public void UpdateHP(float healthRatio)
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = healthRatio;
        }
    }

    public void UpdateLocalHP()
    {
        if (targetPlayer != null && targetPlayer.Stats != null)
        {
            float healthRatio = targetPlayer.Stats.currentHealth / targetPlayer.Stats.maxHealth;
            UpdateHP(healthRatio);
        }
    }
}
