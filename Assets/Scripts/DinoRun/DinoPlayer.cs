using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 반드시 Rigidbody2D가 있는 오브젝트(Player)에 붙어야 해!
[RequireComponent(typeof(Rigidbody2D))]
public class DinoPlayer : MonoBehaviour
{
    [Header("--- Settings ---")]
    public float jumpForce = 1200f; // 점프 힘 (Inspector에서 조절 가능)

    [Header("--- State (View Only) ---")]
    public bool isGrounded = false; // 땅에 닿아있는지 확인용

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 게임 오버 상태면 조작 불가
        if (DinoManager.instance.isGameOver) return;

        // 1. 스페이스바(PC) 또는 화면 터치(Mobile) 감지
        // isGrounded가 true일 때만 점프 가능! (이단 점프 방지)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        // 점프 전 속도를 0으로 초기화 (일정한 점프 높이 보장)
        rb.velocity = Vector2.zero; 
        
        // 위쪽으로 힘을 가함
        rb.AddForce(Vector2.up * jumpForce);
        
        // 공중으로 떴으니 false로 변경
        isGrounded = false; 
    }

    // 물리적 충돌이 일어났을 때 자동 실행되는 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 땅에 닿으면 점프 가능 (Tag 확인 필수!)
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; 
        }
        // 2. 장애물에 닿으면 사망 (Tag 확인 필수!)
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 매니저에게 게임 오버 알림
            DinoManager.instance.GameOver();
        }
    }
}