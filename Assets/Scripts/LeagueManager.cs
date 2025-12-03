using UnityEngine;
using System.Collections.Generic; // 리스트(List) 쓸 때 필요
using UnityEngine.UI;

public class LeagueManager : MonoBehaviour
{
    // 1. 재료 준비: 붕어빵 틀(Prefab)과 담을 그릇(Content)
    public GameObject rankItemPrefab; 
    public Transform contentArea;

    void Start()
    {
        // 게임 시작하자마자 15명 생성!
        CreateRankingList();
    }

    void CreateRankingList()
    {
        // 1. 실제 같은 이름 리스트 (파이썬 리스트랑 같음)
        string[] names = new string[] {
            "박태현", "Wars Dream", "Srishti", "Ace", "Umme",
            "Silvia", "Niccolo", "Maria", "John", "David",
            "Elena", "Hassan", "Yuki", "Kim", "Lee"
        };

        // 2. 점수 기준점 (1등 점수)
        int currentScore = 1500;

        // 15명 생성 반복문
        for (int i = 0; i < names.Length; i++)
        {
            // --- [생성] ---
            GameObject newItem = Instantiate(rankItemPrefab, contentArea);
            
            // --- [데이터 계산] ---
            int rank = i + 1;
            string name = names[i];
            
            // 점수는 1등보다 조금씩 작아지게 랜덤으로 깎음 (리얼함 추가)
            currentScore -= Random.Range(50, 150); 
            string score = currentScore.ToString();
            string totalXP = (currentScore * 3).ToString(); // 총 XP는 대충 점수의 3배라고 치자

            // --- [데이터 주입] ---
            RankItem script = newItem.GetComponent<RankItem>();
            script.SetData(rank.ToString(), name, score, totalXP);

            // --- [✨ 하이라이트 로직] ---
            // 만약 7등("Niccolo")이 '나'라고 가정한다면?
            if (rank == 7)
            {
                // 배경색을 연두색으로 변경! (듀오링고 스타일)
                // 주의: Item_Rank 프리팹 최상위에 Image 컴포넌트가 있어야 작동함
                Image bgImage = newItem.GetComponent<Image>();
                if (bgImage != null)
                {
                    bgImage.color = new Color(0.85f, 1f, 0.85f); // 연한 초록색
                }
            }
        }
    }
}