using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RiskData
{
    public string riskName;         // 리스크 이름
    public string description;      // 리스크 설명
    public float multiplier;        // 골드 배율
    public bool isSelected;         // 선택 여부
}
