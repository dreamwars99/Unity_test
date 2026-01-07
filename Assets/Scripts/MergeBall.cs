using UnityEngine;

public class MergeBall : MonoBehaviour
{
    public int level = 0; // 0단계(작음) ~ 7단계(큼)
    public bool isMerged = false; // 이미 합체된 상태인지 체크 (중복 합체 방지)

    private MergeManager manager;

    public void Init(int _level, MergeManager _manager)
    {
        level = _level;
        manager = _manager;
        isMerged = false;

        // 레벨에 따라 색상과 크기를 바꿈 (매니저한테 물어봄)
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        // 크기 설정 (레벨이 오를수록 1.2배씩 커짐)
        float scale = 0.5f + (level * 0.15f); 
        transform.localScale = Vector3.one * scale;

        // 색상 설정
        GetComponent<UnityEngine.UI.Image>().color = manager.GetLevelColor(level);
    }

    // 🔥 유니티 물리 엔진의 꽃: 충돌 감지
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMerged) return; // 난 이미 합체 처리됨.

        // 부딪힌 상대방도 MergeBall인지 확인
        MergeBall otherBall = collision.gameObject.GetComponent<MergeBall>();
        
        if (otherBall != null)
        {
            // 1. 같은 레벨인가?
            // 2. 내가 상대방보다 ID가 낮은가? (두 공 중 하나만 합체 로직을 실행하게 하려고)
            if (otherBall.level == level && 
                otherBall.GetInstanceID() < this.GetInstanceID() &&
                !otherBall.isMerged)
            {
                // 합체!
                isMerged = true;
                otherBall.isMerged = true;

                // 매니저한테 "우리 둘 합쳐서 다음 레벨 공 만들어줘!" 요청
                manager.MergeBalls(this, otherBall);
            }
        }
    }
}