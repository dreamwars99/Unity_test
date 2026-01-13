using UnityEngine;

// 이 스크립트는 떨어지는 "테트리로미노 프리팹" 그 자체에 붙는 거야.
public class TetrisPiece : MonoBehaviour
{
    // 마지막으로 떨어진 시간 (파이썬의 time.time 과 비슷)
    private float previousTime;
    
    // 떨어지는 속도 (낮을수록 빠름)
    public float fallTime = 1.0f;

    void Start()
    {
        // 처음엔 기본 속도로 설정
        // 난이도 조절을 위해 Manager에서 fallTime을 바꿀 수도 있음
    }

    void Update()
    {
        // 1. 좌우 이동 (PC 테스트용 키보드 + 모바일 버튼 연동 가능)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(new Vector3(-100, 0, 0)); // 왼쪽으로 100px (블록 크기)
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(new Vector3(100, 0, 0)); // 오른쪽으로 100px
        }
        // 2. 회전 (위쪽 화살표)
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        // 3. 아래로 이동 (타이머 or 아래 화살표)
        else if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
            MoveDown();
        }
    }

    // 이동 함수 (파이썬의 def move(self, dir):)
    public void Move(Vector3 direction)
    {
        transform.localPosition += direction;

        // 이동했는데 유효하지 않은 위치라면? (벽을 뚫거나 다른 블록과 겹침)
        if (!TetrisManager.instance.IsValidMove(transform))
        {
            transform.localPosition -= direction; // 원상복구 (Backtracking)
        }
    }

    // 회전 함수
    public void Rotate()
    {
        // Z축 기준으로 90도 회전
        transform.Rotate(0, 0, -90);

        if (!TetrisManager.instance.IsValidMove(transform))
        {
            transform.Rotate(0, 0, 90); // 안되면 다시 되돌림
        }
    }

    // 한 칸 아래로
    void MoveDown()
    {
        transform.localPosition += new Vector3(0, -100, 0); // Y축 -100

        // 더 이상 내려갈 곳이 없다면? (바닥이거나 다른 블록 위)
        if (!TetrisManager.instance.IsValidMove(transform))
        {
            transform.localPosition -= new Vector3(0, -100, 0); // 다시 위로 한 칸 올리고
            AddToGrid(); // 그리드에 박제!
            CheckLines(); // 줄 지워졌나 확인
            this.enabled = false; // 이제 이 스크립트는 죽음 (더 이상 안 움직임)
            TetrisManager.instance.SpawnPiece(); // 다음 타자 등장
        }

        previousTime = Time.time; // 타이머 리셋
    }

    // 박제: 이제 움직이는 Piece가 아니라 배경의 일부가 됨
    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            // 월드 좌표가 아니라, Board_Origin 기준의 로컬 좌표를 그리드 인덱스로 변환
            // 100으로 나누고 반올림해서 0~9, 0~19 사이 정수로 만듦
            int roundedX = Mathf.RoundToInt(children.transform.position.x - TetrisManager.instance.origin.position.x) / 100;
            int roundedY = Mathf.RoundToInt(children.transform.position.y - TetrisManager.instance.origin.position.y) / 100;

            TetrisManager.instance.AddToGrid(children, roundedX, roundedY);
        }
    }

    void CheckLines()
    {
        TetrisManager.instance.CheckForLines();
    }
}