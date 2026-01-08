using UnityEngine;

public class SpacePlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f; 
    public float keyboardSpeed = 800f; 
    public float xLimit = 400f;   
    public float yLimit = 1300f;  

    [Header("Weapon")]
    public GameObject bulletPrefab;
    public Transform firePoint;   
    public float fireRate = 0.2f; 
    public int weaponLevel = 1;   

    private float fireTimer = 0f;
    private bool isDead = false;
    private Vector2 targetPos;    

    void Start()
    {
        // 🔥 [핵심 수정] 시작 시 충돌체 크기 자동 조절 (안전장치)
        SyncColliderSize();
        InitPlayer();
    }
    
    // 이미지 크기에 맞춰 충돌 박스(Collider) 늘려주는 함수
    void SyncColliderSize()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        RectTransform rect = GetComponent<RectTransform>();

        if (col != null && rect != null)
        {
            // 이미지 크기(rect.sizeDelta)를 충돌 박스 크기(col.size)에 덮어씌움
            col.size = rect.sizeDelta;
            col.offset = Vector2.zero;
        }
    }
    
    public void InitPlayer()
    {
        isDead = false;
        weaponLevel = 1; // 무기 초기화
        
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -1250f);
        targetPos = rect.anchoredPosition;
    }

    // 아이템 먹었을 때 호출
    public void UpgradeWeapon()
    {
        weaponLevel++;
        if (weaponLevel > 3) weaponLevel = 3;
        // Debug.Log($"🚀 무기 업그레이드! 현재 레벨: {weaponLevel}");
    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform, 
                Input.mousePosition, 
                null, 
                out mousePos
            );
            targetPos = mousePos;
        }
        else
        {
            float h = Input.GetAxis("Horizontal"); 
            float v = Input.GetAxis("Vertical");   

            if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)
            {
                targetPos += new Vector2(h, v) * keyboardSpeed * Time.deltaTime;
            }
        }

        targetPos.x = Mathf.Clamp(targetPos.x, -xLimit, xLimit);
        targetPos.y = Mathf.Clamp(targetPos.y, -yLimit, yLimit);

        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, moveSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Fire();
            fireTimer = fireRate;
        }
    }

    void Fire()
    {
        switch (weaponLevel)
        {
            case 1: CreateBullet(0); break;
            case 2: CreateBullet(-15); CreateBullet(15); break;
            default: CreateBullet(0); CreateBullet(-30, -15f); CreateBullet(30, 15f); break;
        }
    }

    void CreateBullet(float xOffset, float angle = 0f)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, transform.parent); 
        
        RectTransform bulletRect = bullet.GetComponent<RectTransform>();
        Vector2 spawnPos = GetComponent<RectTransform>().anchoredPosition + new Vector2(xOffset, 50f);
        bulletRect.anchoredPosition = spawnPos;

        bullet.transform.rotation = Quaternion.Euler(0, 0, -angle);
        bullet.tag = "PlayerBullet"; 
        
        SpaceBullet sb = bullet.GetComponent<SpaceBullet>();
        if(sb != null) sb.isEnemyBullet = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet"))
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        gameObject.SetActive(false); 
        
        SpaceManager manager = FindObjectOfType<SpaceManager>();
        if (manager != null) manager.GameOver();
    }
}