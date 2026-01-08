using UnityEngine;

public class SpaceBoss : MonoBehaviour
{
    [Header("Status")]
    public float moveSpeed = 100f;
    public int maxHealth = 50;
    public int currentHealth;
    public int scoreValue = 1000;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public float fireRate = 1.0f;
    private float fireTimer = 0f;

    private SpaceManager manager;
    private bool isDead = false;
    private float startY = 1000f; // 보스 등장 Y 위치

    public void Init(SpaceManager _manager)
    {
        manager = _manager;
        currentHealth = maxHealth;
        
        // 안전하게 Z축 0 고정 + 충돌체 크기 맞춤
        Vector3 pos = transform.localPosition;
        pos.z = 0;
        transform.localPosition = pos;
        
        SyncCollider();
    }

    void SyncCollider()
    {
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
        if (isDead) return;

        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        // 1. 등장 연출 (처음엔 위에서 스르륵 내려옴)
        if (transform.localPosition.y > 600f)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }
        else
        {
            // 2. 다 내려왔으면 좌우 왕복 (PingPong)
            // X좌표 -300 ~ 300 사이를 왔다갔다
            float x = Mathf.PingPong(Time.time * moveSpeed, 600f) - 300f;
            
            Vector3 pos = transform.localPosition;
            pos.x = x;
            pos.z = 0; // Z축 고정
            transform.localPosition = pos;
        }
    }

    void HandleAttack()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            FirePattern();
            fireTimer = fireRate;
        }
    }

    void FirePattern()
    {
        // 3발 부채꼴 발사
        SpawnBullet(0);
        SpawnBullet(-20);
        SpawnBullet(20);
    }

    void SpawnBullet(float angle)
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, transform.parent);
        RectTransform bulletRect = bullet.GetComponent<RectTransform>();
        
        // 위치: 보스 약간 아래
        Vector2 spawnPos = GetComponent<RectTransform>().anchoredPosition + new Vector2(0, -100f);
        bulletRect.anchoredPosition = spawnPos;
        
        // 각도 회전
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Z축 고정
        Vector3 bPos = bulletRect.localPosition;
        bPos.z = 0;
        bulletRect.localPosition = bPos;

        // 태그 및 속성 설정
        bullet.tag = "EnemyBullet";
        SpaceBullet sb = bullet.GetComponent<SpaceBullet>();
        if (sb != null) sb.isEnemyBullet = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage(1);
        }
        else if (collision.CompareTag("Player"))
        {
            // 플레이어랑 박으면? 보스는 멀쩡, 플레이어는 즉사?
            // 일단 데미지만 입자.
            // TakeDamage(5); 
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // 깜빡임 효과 등 있으면 좋음 (생략)

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        if (manager != null)
        {
            manager.AddScore(scoreValue);
            manager.GameClear(); // 보스 잡으면 게임 클리어!
        }
        Destroy(gameObject);
    }
}