using UnityEngine;
using TMPro;

public class AcidWord : MonoBehaviour
{
    public float fallSpeed = 200f;
    private TextMeshProUGUI textComponent;
    public string myWord;

    private bool isDead = false; // 중복 사망 방지용 플래그

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void SetWord(string word, float speed)
    {
        myWord = word;
        textComponent.text = myWord;
        fallSpeed = speed;
        isDead = false;
    }

    void Update()
    {
        if (isDead) return; // 이미 죽은 상태면 로직 중단

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // [수정 핵심] 화면 밖으로 나가면 무조건 Miss 처리!
        // DeadZone 콜라이더가 없어도 작동하는 안전장치
        if (transform.localPosition.y < -1000) 
        {
            isDead = true;
            // 매니저에게 "나 바닥에 닿았어!" 라고 보고
            AcidGameManager.instance.OnWordHitBottom(this);
        }
    }
}