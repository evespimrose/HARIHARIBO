using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradePanelTest : MonoBehaviour
{
    public TextMeshProUGUI statsName;
    public TextMeshProUGUI curStats;
    public TextMeshProUGUI statsValue;
    public TextMeshProUGUI statsPlusChance;
    public Button statsPlusButton;

    private void Awake()
    {
        statsPlusButton.onClick.AddListener(StatsPlusButtonClick);
    }

    public void StatsNameUpdate(string statsname)
    {
        statsName.text = statsname;
    }

    public void CurStatsUpdate(string curstats)
    {
        curStats.text = curstats;
    }

    public void StatsValueUpdate(string statsvalue)
    {
        statsValue.text = statsvalue;
    }

    public void StatsPlusChanceUpdate(string statspluschance)
    {
        statsPlusChance.text = statspluschance;
    }

    public void StatsPlusButtonClick()
    {
        FirebaseManager.Instance.UpgradeCharacter(statsName.text);
    }

    
}
