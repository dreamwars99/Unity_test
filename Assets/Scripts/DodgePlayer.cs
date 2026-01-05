using UnityEngine;

public class DodgePlayer : MonoBehaviour
{
    public float speed = 1000f; 

    void Update()
    {
        // 1. 키보드 입력 (기본)
        // -1: 왼쪽, 0: 정지, 1: 오른쪽
        float x = Input.GetAxisRaw("Horizontal");

        // 2. [신규] 마우스 클릭 또는 터치 입력 감지
        // (Input.GetMouseButton(0)은 마우스를 누르고 있거나, 화면을 누르고 있을 때 True)
        if (Input.GetMouseButton(0))
        {
            // 마우스(터치) 위치가 화면 가로 길이의 절반(중앙)보다 왼쪽이냐?
            if (Input.mousePosition.x < Screen.width / 2)
            {
                x = -1; // 왼쪽 이동
            }
            else
            {
                x = 1; // 오른쪽 이동
            }
        }

        // 3. 이동 처리 (키보드든 터치든 결정된 x값으로 이동)
        transform.Translate(Vector3.right * x * speed * Time.deltaTime);

        // 4. 화면 밖 이동 제한 (기존 코드 유지)
        Vector3 currentPos = transform.localPosition;
        currentPos.x = Mathf.Clamp(currentPos.x, -650f, 650f);
        transform.localPosition = currentPos;
    }
}