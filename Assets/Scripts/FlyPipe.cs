using UnityEngine;

public class FlyPipe : MonoBehaviour
{
    // 개별 속도 변수는 삭제함. 매니저 속도를 따름.
    private bool hasPassed = false;
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.Find("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        // [신규] 매니저한테 "지금 속도 몇이에요?" 물어봐서 이동
        float currentSpeed = FlyManager.instance.currentPipeSpeed;
        
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        // 화면 밖 삭제
        if (transform.localPosition.x < -700f) Destroy(gameObject);

        // 충돌 및 점수 로직
        if (player != null)
        {
            float distSide = transform.localPosition.x - player.localPosition.x;
            float distHeight = Mathf.Abs(transform.localPosition.y - player.localPosition.y);

            // 1. 충돌 (사망)
            // 가로 80 이내, 세로 250 이상 차이나면 박은 것
            if (Mathf.Abs(distSide) < 80f && distHeight > 250f)
            {
                FlyManager.instance.GameOver();
            }

            // 2. 점수 (통과)
            if (distSide < -50f && !hasPassed && !FlyManager.instance.isGameOver)
            {
                hasPassed = true;
                FlyManager.instance.AddScore();
            }
        }
    }
}