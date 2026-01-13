using UnityEngine;

public class PongPaddle : MonoBehaviour
{
    public bool isPlayer = true; 
    public float moveSpeed = 10f; // AI 속도
    public float playerKeyboardSpeed = 20f; // 키보드 이동 속도
    public Transform ball; 
    
    // [수정] 화면 끝(540)까지 가도록 범위 확장 (기존 480 -> 525)
    // 패들 너비가 크다면 이 값을 인스펙터에서 조금 줄여야 벽을 안 뚫습니다.
    public float xLimit = 540f; 

    // 마지막 마우스 위치 저장용
    private Vector3 lastMousePos;

    void Start()
    {
        lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        if (isPlayer)
        {
            MovePlayer();
        }
        else
        {
            MoveAI();
        }
    }

    void MovePlayer()
    {
        float targetX = transform.localPosition.x;
        float h = Input.GetAxisRaw("Horizontal"); // 키보드 입력 (-1, 0, 1)

        // 1. 키보드 입력이 있을 때 (우선순위 1등)
        if (h != 0)
        {
            // 키보드로 이동
            targetX += h * playerKeyboardSpeed * Time.deltaTime * 60f;
            lastMousePos = Input.mousePosition; 
        }
        // 2. 마우스 입력이 있을 때 (키보드 안 누를 때)
        else 
        {
            // 마우스가 조금이라도 움직였는지 확인
            if (Vector3.Distance(Input.mousePosition, lastMousePos) > 0.1f)
            {
                float mouseRatio = Input.mousePosition.x / Screen.width;
                targetX = (mouseRatio * 1080f) - 540f;
                lastMousePos = Input.mousePosition;
            }
        }

        // 3. 이동 제한 (Clamp)
        targetX = Mathf.Clamp(targetX, -xLimit, xLimit);

        transform.localPosition = new Vector3(targetX, transform.localPosition.y, 0);
    }

    void MoveAI()
    {
        if (ball == null) return;

        float targetX = Mathf.Lerp(transform.localPosition.x, ball.localPosition.x, moveSpeed * Time.deltaTime);
        targetX = Mathf.Clamp(targetX, -xLimit, xLimit);

        transform.localPosition = new Vector3(targetX, transform.localPosition.y, 0);
    }
}