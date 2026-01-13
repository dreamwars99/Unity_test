using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 부딪힌 게 산성비 단어라면?
        AcidWord word = collision.GetComponent<AcidWord>();
        if (word != null)
        {
            // 매니저에게 알리고 삭제 요청
            AcidGameManager.instance.OnWordHitBottom(word);
        }
    }
}