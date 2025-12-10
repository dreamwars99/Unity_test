using UnityEngine;
using System.Collections;

public class ConfettiEffect : MonoBehaviour
{
    // 아까 만든 투명 패널을 여기에 연결
    public GameObject confettiPanel;

    public void Fire()
    {
        // 이미 터지고 있다면 무시하거나, 껐다 켜서 다시 터뜨림
        if (confettiPanel.activeSelf)
        {
            confettiPanel.SetActive(false); // 끄고
        }
        
        StartCoroutine(ProcessConfetti());
    }

    IEnumerator ProcessConfetti()
    {
        // 1. 폭죽 발사! (활성화되면 AutoPlay: OnEnable 덕분에 바로 재생됨)
        confettiPanel.SetActive(true);

        // 2. 2초 정도 기다림 (애니메이션이 끝날 때까지)
        yield return new WaitForSeconds(2.0f);

        // 3. 쓰레기 정리 (패널 끄기)
        confettiPanel.SetActive(false);
    }
}