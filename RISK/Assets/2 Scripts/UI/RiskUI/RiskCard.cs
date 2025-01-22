using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RiskCard : MonoBehaviour
{
    [Header("Card Elements")]
    [SerializeField] private Image riskIcon;
    [SerializeField] private TextMeshProUGUI riskNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI goldMultiplierText;
    [SerializeField] private TextMeshProUGUI expectedGoldText;

    [Header("Player Vote Indicators")]
    [SerializeField] private Image[] playerVoteImages;  // Player1, Player2, Player3 이미지

    [Header("Buttons")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI selectButtonText;
    [SerializeField] private TextMeshProUGUI cancelButtonText;


    public Button SelectButton => selectButton;
    public Button CancelButton => cancelButton;
    public int RiskId { get; private set; }

    public void Initialize(RiskData data, int index)
    {
        RiskId = data.riskId;

        // 기본 정보 설정
        riskNameText.text = data.riskName;
        descriptionText.text = data.description;
        goldMultiplierText.text = $"x{data.multiplier:F1}";

        // 예상 골드량 계산 (실제 계산 로직은 게임 상황에 맞게 구현 필요)
        int baseGold = 1000; // 예시 값
        int expectedGold = (int)(baseGold * data.multiplier);
        expectedGoldText.text = $"{expectedGold:N0} Gold";


        // 플레이어 투표 표시 초기화
        foreach (var playerImage in playerVoteImages)
        {
            playerImage.gameObject.SetActive(false);
        }

        // 버튼 텍스트 설정
        selectButtonText.text = "선택";
        cancelButtonText.text = "취소";
    }

    public void SetSelected(bool selected)
    {
        selectButton.gameObject.SetActive(!selected);
        cancelButton.gameObject.SetActive(selected);
    }

    // 특정 플레이어의 투표 표시 업데이트
    public void UpdatePlayerVote(int playerIndex, bool voted)
    {
        if (playerIndex >= 0 && playerIndex < playerVoteImages.Length)
        {
            playerVoteImages[playerIndex].gameObject.SetActive(voted);
        }
    }
}
