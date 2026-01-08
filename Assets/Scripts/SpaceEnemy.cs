using UnityEngine;

public class SpaceEnemy : MonoBehaviour
{
    public float speed = 300f; 
    public int health = 1;
    public int scoreValue = 100;

    public GameObject bulletPrefab; 
    public float fireRateMin = 1.0f;
    public float fireRateMax = 3.0f;

    private float fireTimer;
    private SpaceManager manager; 

    // 매니저가 적을 생성할 때 호출하는 초기화 함수
    public void Init(SpaceManager _manager, float moveSpeed)
    {
        manager = _manager;
        speed = moveSpeed;
        fireTimer = Random.Range(fireRateMin, fireRateMax);
        
        // 1. Z축 0으로 고정
        Vector3 pos = transform.localPosition;
        pos.z = 0;
        transform.localPosition = pos;

        // 🔥 [핵심 해결책] 이미지 크기에 맞춰 충돌 박스 크기 자동 조절!
        SyncColliderSize();
    }

    void SyncColliderSize()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        RectTransform rect = GetComponent<RectTransform>();

        if (col != null && rect != null)
        {
            // 이미지 크기(rect.sizeDelta)를 충돌 박스 크기(col.size)에 덮어씌움
            col.size = rect.sizeDelta;
            // 혹시 오프셋이 틀어졌을까봐 중앙 정렬
            col.offset = Vector2.zero;
        }
    }

    void Update()
    {
        // 움직일 때마다 Z축 튀지 않게 감시
        if (transform.localPosition.z != 0)
        {
            Vector3 pos = transform.localPosition;
            pos.z = 0;
            transform.localPosition = pos;
        }

        // 아래로 이동
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // 화면 밖 삭제
        if (transform.localPosition.y < -1300f)
        {
            Destroy(gameObject);
        }

        // 사격
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Fire();
            fireTimer = Random.Range(fireRateMin, fireRateMax);
        }
    }

    void Fire()
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.parent);
            RectTransform bulletRect = bullet.GetComponent<RectTransform>();
            
            // 위치 설정
            Vector2 spawnPos = GetComponent<RectTransform>().anchoredPosition + new Vector2(0, -50f);
            bulletRect.anchoredPosition = spawnPos;
            
            // 총알 Z축 0 고정
            Vector3 bulletPos = bulletRect.localPosition;
            bulletPos.z = 0;
            bulletRect.localPosition = bulletPos;
            
            bullet.tag = "EnemyBullet";
            
            SpaceBullet sb = bullet.GetComponent<SpaceBullet>();
            if(sb != null) sb.isEnemyBullet = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject); 
            TakeDamage(1);
        }
        else if (collision.CompareTag("Player"))
        {
            TakeDamage(100); 
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (manager != null) manager.AddScore(scoreValue);
            Destroy(gameObject); 
        }
    }
}