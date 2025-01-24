using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonBossUI : MonoBehaviour
{
    public TextMeshProUGUI bossName;
    public Image bossHpBar;

    public BossMonster bossMonster;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() =>  bossHpBar != null);
        bossName.text = bossMonster.bossName;
    }

    // Update is called once per frame
    void Update()
    {
        if (bossMonster == null) return;
        bossHpBar.fillAmount = bossMonster.curHp / bossMonster.maxHp;
    }
}
