using UnityEngine;

public class PopupManager : MonoBehaviour
{
    // 2번 문제 해결용 변수
    [Header("Popups")]
    public GameObject popupPoke; 

    void Start()
    {
        // 시작하자마자 팝업 끄기
        if(popupPoke != null) popupPoke.SetActive(false);
    }

    // [중요] public이 있어야 Inspector에서 선택 가능!
    public void OpenPokePopup()
    {
        if(popupPoke != null) popupPoke.SetActive(true);
    }

    public void ClosePokePopup()
    {
        if(popupPoke != null) popupPoke.SetActive(false);
    }
    
    public void ConfirmPoke()
    {
        Debug.Log("👉 콕 찔렀습니다!");
        ClosePokePopup(); 
    }
}