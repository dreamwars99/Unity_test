using UnityEngine;
using UnityEngine.UI;

public class TowerBlock : MonoBehaviour
{
    // 이제 inspector 값이 아니라 Manager가 주는 값을 쓸 거임
    public float moveSpeed; 
    public float moveRange;
    
    private bool isStopped = false;

    // [신규] 태어날 때 매니저가 이 함수를 부를 거야 (초기화)
    public void Init(float newSpeed, float newRange, float newWidth)
    {
        moveSpeed = newSpeed;
        moveRange = newRange;

        // 크기 변경 (너비 설정)
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);

        // 색상 랜덤
        GetComponent<Image>().color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f);
        
        // 시간 강제 흐름
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        if (isStopped) return;

        float totalWidth = moveRange * 2;
        float x = Mathf.PingPong(Time.time * moveSpeed, totalWidth) - moveRange;
        
        transform.localPosition = new Vector3(x, transform.localPosition.y, 0);
    }

    public void StopMoving()
    {
        isStopped = true;
    }
}