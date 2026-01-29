using UnityEngine;

public class StudyRoomGuideController : MonoBehaviour
{
    public GameObject guideHUD; 

    // 게임 시작 시 가이드가 켜져 있는지 확인
    void Start()
    {
        if (guideHUD != null) guideHUD.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 이 구역(스터디룸 문)을 완전히 벗어나면 가이드 OFF
        if (other.CompareTag("Player"))
        {
            if (guideHUD != null)
            {
                guideHUD.SetActive(false);
                Debug.Log("스터디룸 탈출! 가이드를 종료합니다.");
            }
        }
    }
}