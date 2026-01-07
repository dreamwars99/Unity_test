using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class Mole : MonoBehaviour
{
    public float showTime = 1.0f; 
    
    private Button btn;
    private Image img;
    private MoleManager manager;
    private bool isActive = false;

    // 색상 정의
    private Color hideColor = Color.gray; 
    private Color activeColor = new Color(0.8f, 0.5f, 0.2f); // 갈색
    private Color hitColor = Color.red; 

    void Awake()
    {
        btn = GetComponent<Button>();
        img = GetComponent<Image>();

        // 🔥 [핵심 수정 1] 버튼 컴포넌트가 색깔을 맘대로 바꾸지 못하게 막음!
        btn.transition = Selectable.Transition.None; 

        btn.onClick.AddListener(OnHit);
    }

    public void Setup(MoleManager mgr)
    {
        manager = mgr;
        Hide();
    }

    public void PopUp()
    {
        if (isActive) return;

        isActive = true;
        
        // 🔥 [핵심 수정 2] 색상 변경 확실하게 적용
        img.color = activeColor;
        btn.interactable = true; 

        Debug.Log($"{name} 두더지 등장! (색상: 갈색)"); // 로그 확인용

        StopAllCoroutines();
        StartCoroutine(AutoHide());
    }

    public void Hide()
    {
        isActive = false;
        img.color = hideColor;
        btn.interactable = false;
    }

    void OnHit()
    {
        if (!isActive) return;

        manager.AddScore(10);
        
        img.color = hitColor;
        isActive = false;
        btn.interactable = false;

        Debug.Log("두더지 잡았다! (색상: 빨강)");

        StopAllCoroutines();
        Invoke("Hide", 0.2f); 
    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(showTime);
        Hide();
    }
}