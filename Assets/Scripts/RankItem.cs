using UnityEngine;
using TMPro; // 글자(TextMeshPro) 쓸 때 필수!
using UnityEngine.UI; // 이미지(Image) 쓸 때 필수!

public class RankItem : MonoBehaviour
{
    // 1. Inspector에서 우리가 끌어다 놓을 빈칸들
    public TextMeshProUGUI textRank;    // 등수 (1, 2, 3...)
    public Image imgAvatar;             // 프로필 사진
    public TextMeshProUGUI textName;    // 이름
    public Image imgFlag;               // 국기
    public TextMeshProUGUI textScore;   // 점수 (130)
    public TextMeshProUGUI textTotalXP; // 총 경험치 (1167 XP)

    // 2. 외부에서 데이터를 받아서 화면을 갱신하는 함수
    public void SetData(string rank, string name, string score, string totalXP)
    {
        textRank.text = rank;
        textName.text = name;
        textScore.text = score;
        textTotalXP.text = totalXP + " XP";
        
        // (이미지는 나중에 데이터 들어오면 처리할 예정)
        // [추가] 3등까지만 메달 보여주고, 4등부터는 메달 숨기기
        // 파이썬: if int(rank) > 3: imgRank.SetActive(False)
        int rankNum = int.Parse(rank);
        
        // 1_Rank 이미지(노란 메달)를 껐다 켰다 하는 것
        // 주의: textRank(숫자)의 부모가 메달 이미지라면, 부모를 끄면 숫자도 같이 꺼짐.
        // 이 부분은 네 Hierarchy 구조에 따라 다를 수 있어. 
        // 일단 메달 이미지가 있다면 -> imgRankObject.SetActive(rankNum <= 3); 같은 식인데
        // 지금은 간단하게 텍스트 색깔만 바꿔보자.
        
        if (rankNum > 3)
        {
             // 4등부터는 등수 글자색을 회색으로
             textRank.color = Color.gray;
             // (만약 메달 이미지를 변수로 연결해뒀다면 여기서 끄면 됨)
        }
        else
        {
             // 1,2,3등은 금색 느낌
             textRank.color = new Color(1f, 0.8f, 0f); 
        }
    }

}