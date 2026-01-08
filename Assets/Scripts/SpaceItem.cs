using UnityEngine;

public class SpaceItem : MonoBehaviour
{
    public float speed = 200f; // 내려오는 속도

    void Start()
    {
        // 얘도 Z축 0 고정 + 충돌체 크기 자동 조절 (안전빵)
        SyncCollider();
    }

    void SyncCollider()
    {
        Vector3 pos = transform.localPosition;
        pos.z = 0;
        transform.localPosition = pos;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        RectTransform rect = GetComponent<RectTransform>();
        if (col != null && rect != null)
        {
            col.size = rect.sizeDelta;
            col.offset = Vector2.zero;
        }
    }

    void Update()
    {
        // 아래로 이동
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Z축 감시
        if (transform.localPosition.z != 0)
        {
             Vector3 pos = transform.localPosition;
             pos.z = 0;
             transform.localPosition = pos;
        }

        // 화면 밖 삭제
        if (transform.localPosition.y < -1300f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 부딪히면
        if (collision.CompareTag("Player"))
        {
            // 플레이어 무기 강화 시키고
            SpacePlayer player = collision.GetComponent<SpacePlayer>();
            if (player != null)
            {
                player.UpgradeWeapon();
            }
            
            // 아이템은 사라짐
            Destroy(gameObject);
        }
    }
}