using UnityEngine;

public class PongGoal : MonoBehaviour
{
    [Tooltip("체크하면 적 골대(득점), 체크 해제하면 내 골대(게임오버)")]
    public bool isPlayerGoal; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 공인지 확인
        if (collision.gameObject.name.Contains("Ball") || collision.GetComponent<PongBall>() != null) 
        {
            Debug.Log($"🚨 [충돌 발생] '{this.gameObject.name}' 골대에 '{collision.gameObject.name}'이 닿았습니다!");

            if (isPlayerGoal == false) 
            {
                Debug.Log($"💀 [게임 오버 판정] '{this.gameObject.name}'은(는) 플레이어 골대이므로 게임 오버 처리합니다.");
                if (PongManager.instance != null)
                {
                    PongManager.instance.OnGameOver();
                }
            }
            else 
            {
                Debug.Log($"⚽ [득점 판정] '{this.gameObject.name}'은(는) 적 골대이므로 득점 처리합니다.");
                if (PongManager.instance != null)
                {
                    PongManager.instance.OnGoal(true);
                }
            }
        }
    }
}