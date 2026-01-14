using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    [Header("--- Settings ---")]
    public float speed = 3200f;      // 이동 속도 (UI 기준이라 600 정도는 되어야 시원함!)
    public float deadZone = -1200f; // 화면 왼쪽 끝 좌표 (이거 넘어가면 삭제)

    void Update()
    {
        // 1. 왼쪽으로 계속 이동
        // Vector3.left = (-1, 0, 0)
        // Time.deltaTime을 곱해야 프레임 상관없이 일정한 속도로 움직임
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // 2. 화면 왼쪽 밖으로 나가면 삭제 (메모리 관리)
        if (transform.localPosition.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}