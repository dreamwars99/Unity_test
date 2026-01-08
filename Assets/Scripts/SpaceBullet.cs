using UnityEngine;

public class SpaceBullet : MonoBehaviour
{
    public float speed = 1000f; // 총알 속도
    public int damage = 1;
    public bool isEnemyBullet = false; 

    void Start()
    {
        // 🔥 [핵심 해결책] 총알 이미지 크기에 맞춰 충돌 박스 자동 조절
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        RectTransform rect = GetComponent<RectTransform>();

        if (col != null && rect != null)
        {
            col.size = rect.sizeDelta;
            col.offset = Vector2.zero;
            // Debug.Log($"총알 충돌체 크기 조정됨: {col.size}"); // 확인용 로그
        }
    }

    void Update()
    {
        float dir = isEnemyBullet ? -1f : 1f;
        transform.Translate(Vector3.up * speed * dir * Time.deltaTime);

        // Z축 보정 (혹시 모르니)
        if (transform.localPosition.z != 0)
        {
             Vector3 pos = transform.localPosition;
             pos.z = 0;
             transform.localPosition = pos;
        }

        if (Mathf.Abs(transform.localPosition.y) > 1200f)
        {
            Destroy(gameObject);
        }
    }
}