using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject rivePrefab; // 아까 만든 파란 상자(Prefab)
    public int count = 100;       // 100개 도전!

    void Start()
    {
        // 파이썬의 for i in range(count): 와 똑같아
        for (int i = 0; i < count; i++)
        {
            // Instantiate(누구를, 어디에_자식으로);
            Instantiate(rivePrefab, transform);
        }
    }
}