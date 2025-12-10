using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LottieSpawner : MonoBehaviour
{
    public GameObject lottiePrefab;
    public Transform parentPanel;
    public int spawnCount = 100; // 개수

    // 이제 이건 필요 없지만, 혹시 연결돼 있어도 상관없게 둠
    public GridLayoutGroup gridLayout; 

    IEnumerator Start()
    {
        // 안전장치: 그리드가 꺼져있다면 강제로 켠다!
        if (gridLayout != null) gridLayout.enabled = true;

        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(lottiePrefab, parentPanel);

            // 🐍 Python: time.sleep(0.05)
            // 0.05초마다 하나씩 생성. 
            // 렉은 절대 안 걸리고, 눈에는 '다다다닥' 차오르는 연출로 보임.
            yield return new WaitForSeconds(0.05f); 
        }
    }
}