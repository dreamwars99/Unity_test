using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 코루틴(Delay)을 쓰기 위해 필요

public class SceneConnector : MonoBehaviour
{
    // 아까 만든 '가짜 로딩 패널'을 여기에 연결할 거야
    public GameObject loadingPanel; 

    public void GoToMainScene()
    {
        // 그냥 가는 게 아니라, 코루틴(시간차 공격)을 시작한다!
        StartCoroutine(ProcessFakeLoading());
    }

    IEnumerator ProcessFakeLoading()
    {
        // 1. 로딩 패널을 켠다! (로티 애니메이션이 돌기 시작함)
        loadingPanel.SetActive(true);

        // 2. 2.5초 동안 아무것도 안 하고 기다린다. (가짜 로딩 타임)
        // 파이썬의 time.sleep(2.5)와 같음
        yield return new WaitForSeconds(1f);

        // 3. 기다림이 끝나면 진짜 씬으로 이동!
        SceneManager.LoadScene("DuoMain");
    }
}