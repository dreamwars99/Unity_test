

## [2026.01.07] (수) [4차] - 유니티 미니게임 9호: Drop & Merge 개발

### 🎯 오늘의 목표 (Daily Goal)
- 물리 엔진(Rigidbody 2D)을 활용한 '수박 게임(Suika Game)' 스타일의 머지(Merge) 아케이드 게임 구현.
- 마우스 입력을 UI 좌표계로 변환하여 정밀한 오브젝트 투하(Drop) 시스템 구축.

### 🎮 게임 설명 (Game Description)
- **Drop & Merge**: 마우스를 움직여 위치를 조준하고, 클릭하여 공을 즉시 떨어트리는 하이 템포 아케이드 게임.
- **Merge Logic**: 같은 색상(Level)의 공이 물리적으로 충돌하면 합체하여 다음 단계의 공으로 진화.
- **Physics**: 중력 가속(gravityScale)을 10배로 높여 묵직한 조작감을 구현하고, 합체 시 튀어 오르는 연출(AddForce)로 타격감 강화.

### 💻 스크립트 로직 & 구조 (Detailed Logic)
- **`MergeManager.cs` (Core)**:
  - **Input Handling**: 마우스 X좌표를 Screen.width 비율로 계산하여 게임 내 좌표로 변환, 클릭 시 즉시 투하 로직 구현.
  - **Object Pooling Logic**: 손에 들고 있는 공(currentBall)은 물리 연산을 끄고 대기하다가, 투하 시 Dynamic 모드로 전환.
  - **Speed Tuning**: 게임의 속도감을 위해 공 생성 쿨타임을 0.05초로 극한까지 단축.
- **`MergeBall.cs` (Object)**:
  - `OnCollisionEnter2D`에서 상대방 공의 ID(`GetInstanceID`)를 비교하여 중복 합체 방지.
- **`MergeWallBuilder.cs` (Tool)**:
  - 해상도 변경에 대응하기 위해 게임 시작 시 자동으로 화면 크기에 맞는 바닥과 벽(BoxCollider2D)을 생성.

### 🐛 트러블슈팅 및 시행착오 (Troubleshooting & Trial and Error)
- **좌표계 혼동 (UI vs World)**:
  - **문제**: 마우스 좌표(Screen)를 그대로 사용하여 공이 엉뚱한 위치에 생성됨.
  - **해결**: RectTransformUtility를 사용하려다 복잡하여, 화면 비율(Input.mousePosition.x / Screen.width)을 이용한 상대 좌표 계산법으로 단순화하여 해결.
- **조작 방식 변경 (UX)**:
  - **시도**: 드래그 앤 드롭 방식을 구현했으나 속도감이 떨어지고 답답함.
  - **변경**: 마우스를 따라다니다 클릭하면 즉시 발사되는 'Point & Click' 방식으로 변경하여 아케이드성 강화.
- **스폰 충돌 버그 (Spawn Collision)**:
  - **문제**: 연사 속도를 높이자, 생성된 공이 떨어지기 전에 다음 공과 겹쳐서 공중에서 합체되거나 에러 발생.
  - **해결**: 대기 중인 공(currentBall)의 Collider를 비활성화(Ghost Mode)하여, 투하 전까지는 물리 충돌을 무시하도록 수정.
- **물리 튕김 현상 (Physics Force)**:
  - **문제**: 합체 시 AddForce(500f)를 적용하자 공이 화면 밖으로 날아가 복귀하지 않음.
  - **해결**: 튕기는 힘을 300f로 하향 조정하고, 중력 가속을 10배로 늘려 바닥으로 빠르게 복귀하도록 밸런스 조정.
- **바닥 높이 이슈**:
  - **문제**: 자동 생성된 바닥이 너무 높아 플레이 영역이 좁음.
  - **해결**: MergeWallBuilder에서 gameHeight를 2100으로 늘리고 floorHeight 오프셋을 -350으로 조정하여 바닥을 화면 하단으로 내림.

### 📂 파일 구조 및 변경 사항
- **New Scripts**: `MergeManager.cs`, `MergeBall.cs`, `MergeWallBuilder.cs`
- **Updated Hierarchy (Game_MergeDrop 구조)**:
  - `Game_MergeDrop`
    ├── `MergeManager` (Script: MergeManager)
    ├── `Walls`
    │   ├── `Wall_Left` (BoxCollider2D)
    │   ├── `Wall_Right` (BoxCollider2D)
    │   └── `Wall_Bottom` (BoxCollider2D)
    ├── `SpawnPoint`
    ├── `DeadZone`
    ├── `Score_UI` (점수판)
    └── `Popup_GameOver` (게임오버 팝업)

## [2026.01.07] (수) [3차] - 유니티 미니게임 8호: Classic Snake 개발

### 🎯 오늘의 목표 (Daily Goal)
- `List<RectTransform>` 자료구조를 활용하여 뱀의 꼬리가 늘어나고 따라오는 알고리즘 구현.
- UI 좌표계(`AnchoredPosition`)를 기반으로 한 그리드(격자) 이동 시스템 구축.

### 🎮 게임 설명 (Game Description)
- **Classic Snake (스네이크)**: 방향키로 뱀을 조종하여 사과를 먹고 몸을 늘리는 고전 게임.
- **Grid Movement**: 뱀은 정해진 간격(Tick)마다 한 칸씩 이동하며, 사과를 먹으면 꼬리가 생성되어 뒤따라옴.
- **Game Over**: 벽(화면 밖)에 부딪히거나, 길어진 자기 자신의 몸통에 부딪히면 종료.

### 💻 스크립트 로직 & 구조 (Detailed Logic)
- **`SnakeGame.cs`**:
  - **Data Structure**: `List<RectTransform> snakeBody`를 사용하여 뱀의 머리와 몸통 객체들을 순서대로 관리 (파이썬의 List와 유사).
  - **Movement Algorithm**: 이동 시 가장 뒤에 있는 꼬리부터 앞쪽 꼬리의 위치로 좌표를 덮어씌우는 방식으로 '따라가는' 움직임 구현 (`for`문 역순 순회).
  - **Input Safety**: `hasMovedThisTick` 변수를 두어, 한 번의 이동 틱(Tick) 내에 방향을 두 번 바꿔서 자기 목을 조르는(180도 회전) 버그 방지.
  - **Collision**: `Vector2.Distance`를 사용하여 먹이 획득 및 자기 자신과의 충돌 판정 처리.

### 📂 파일 구조 및 변경 사항 (Hierarchy & Files)
- **New Script**: `SnakeGame.cs`
- **Updated Hierarchy (Game_Snake 구조)**:
  - `Game_Snake` (Root)
    ├── `SnakeManager` (SnakeGame 스크립트 부착)
    │    ├── (Inspector) Body Prefab: `BodyPart`
    │    ├── (Inspector) Food Prefab: `Food`
    │    └── (Inspector) Game Area: `GameArea` 연결
    ├── `GameArea` (뱀과 먹이가 생성/이동하는 실제 플레이 영역)
    ├── `Score_UI` (점수판 Prefab)
    └── `Popup_GameOver` (게임 종료 팝업 Prefab)

## [2026.01.07] (수) [2차] - 유니티 미니게임 7호: Whack-A-Mole 개발

### 🎯 오늘의 목표 (Daily Goal)
- `Grid Layout Group`을 활용한 3x3 두더지 구멍 배치 및 UI 구조화.
- `Coroutine`을 사용한 랜덤 생성 로직과 스테이지별 난이도 상승(Speed Up) 시스템 구현.

### 🎮 게임 설명 (Game Description)
- **Whack-A-Mole (두더지 잡기)**: 9개의 구멍에서 무작위로 튀어나오는 두더지를 제한 시간 내에 클릭하여 점수를 얻는 아케이드 게임.
- **Stage System**: 게임 오버 후 'Retry' 시 다음 스테이지로 진입하며, 스테이지가 오를수록 두더지가 튀어나오는 속도가 `0.1초`씩 빨라짐 (최소 `0.2초`까지).
- **Score Multiplier**: 높은 스테이지일수록 두더지 한 마리당 획득 점수가 증가함 (1단계=1배, 5단계=5배).

### 💻 스크립트 로직 & 구조 (Detailed Logic)
- **`MoleManager.cs`**:
  - **Spawn Logic**: `StartCoroutine`과 `Random.Range(0, 9)`를 사용하여 두더지를 랜덤한 위치에서 소환. 중복 위치 방지 로직 포함.
  - **Dynamic Difficulty**: `Mathf.Max(0.2f, baseSpawnInterval - ((currentStage - 1) * 0.1f))` 공식을 통해 스테이지별로 생성 주기를 단축.
  - **Score System**: `AddScore` 함수에서 `baseScore * currentStage`를 계산하여 고득점 플레이 유도.
  - **UI Management**: 점수, 남은 시간, 스테이지 표시 및 최고 점수(`PlayerPrefs`) 저장/로드 처리.

### 📂 파일 구조 및 변경 사항 (Hierarchy & Files)
- **New Script**: `MoleManager.cs`, `Mole.cs` (개별 두더지 제어용)
- **Updated Hierarchy (Game_WhackAMole 구조)**:
  - `Game_WhackAMole` (Root)
    ├── `MoleManager` (MoleManager 스크립트 부착 / 오디오 소스 등 관리)
    ├── `Grid_Moles` (Grid Layout Group: 3x3 배치)
    │    ├── `Mole_0` ~ `Mole_8` (각각 Mole 컴포넌트 및 Button 포함)
    ├── `Txt_Time` (남은 시간 표시)
    ├── `Txt_Stage` (현재 스테이지 표시)
    ├── `Score_UI` (Prefab)
    │    ├── `Txt_HighScore`
    │    └── `Txt_Score`
    └── `Popup_GameOver` (Prefab)
         ├── `Txt_GameOver`
         ├── `Txt_FinalScore` (다음 스테이지 예고 포함)
         └── `Btn_Retry` (클릭 시 Next Stage 진행)

## [2026.01.07] (수) [1차] - 유니티 미니게임 6호: Arrow Beat 개발

### 🎯 오늘의 목표 (Daily Goal)
- `Input.GetKeyDown(KeyCode)`를 활용한 키보드 방향키 입력 처리 시스템 구현.
- 게임이 진행될수록 제한 시간이 줄어드는 **동적 난이도 조절(Dynamic Difficulty Adjustment)** 알고리즘 적용.
- `PlayerPrefs`를 활용한 최고 점수(Best Score) 저장 기능 구현.

### 🎮 게임 설명 (Game Description)
- **Arrow Beat (방향키 맞추기)**: 화면에 뜨는 지시(UP, DOWN, LEFT, RIGHT)에 맞춰 키보드 방향키를 빠르게 누르는 순발력 게임.
- **Dynamic Speed**: 정답을 맞출 때마다 제한 시간이 `0.1초`씩 줄어들어 점점 빨라짐 (최소 `0.5초`까지).
- **Reaction Bonus**: 얼마나 빨리 반응했는지(`reactionTime`)를 계산하여 점수에 가산점 부여.

### 💻 스크립트 로직 & 구조 (Detailed Logic)
- **`ArrowGameManager.cs`**:
  - **Input System**: `Input.anyKeyDown`으로 입력을 감지한 후, `Input.GetKeyDown(KeyCode.UpArrow)` 등으로 구체적인 방향 판별.
  - **Random Generator**: `Random.Range(0, 4)`를 사용해 4가지 방향 중 하나를 무작위로 생성(`NextArrow`).
  - **Timer Logic**: `currentMaxTime` 변수를 두어, 성공 시 `Mathf.Max(minTimeLimit, currentMaxTime - timeReduction)`을 통해 난이도를 상승시킴.
  - **Scoring**: 기본 점수 + (남은 시간 * 100) 보너스 점수 합산 방식.
  - **Penalty**: 틀린 키를 입력하면 점수 차감(-50) 및 남은 시간 차감(-0.5초)으로 긴장감 부여.

### 📂 파일 구조 및 변경 사항 (Hierarchy & Files)
- **New Script**: `ArrowGameManager.cs`
- **Updated Hierarchy (Panel_Super 구조)**:
  - `Panel_Super` (Root)
    ├── `Lobby_Main` (게임 선택 화면)
    └── **`Game_ArrowBeat`** (New! - 6호기)
        ├── `Txt_Target` (중앙 화살표/지시어)
        ├── `ArrowBeat_UI` (UI 그룹)
        │    ├── `Txt_HighScore`
        │    ├── `Txt_Score`
        │    └── `Popup_GameOver`
        │         ├── `Txt_GameOver`
        │         ├── `Txt_FinalSocre`
        │         └── `Btn_Retry`
        │              └── `Text (TMP)`
        ├── `Txt_Time` (남은 시간)
        ├── `Txt_Feedback` (판정 텍스트)
        └── `Manager_Arrow` (ArrowGameManager 스크립트 부착)


## [2026.01.06] (화) [4차] - 통합 로비 시스템 (Arcade Lobby) 구축

### 🎯 오늘의 목표 (Daily Goal)
- 개발된 5종의 미니게임을 하나의 앱에서 선택하여 실행할 수 있는 '통합 런처(Launcher)' 시스템 구현.
- 씬(Scene) 전환 없이 `SetActive`를 활용한 가벼운 화면 전환 내비게이션 구조 설계.

### 🎮 시스템 설명 (System Description)
- 앱 실행 시 가장 먼저 표시되는 메인 메뉴 화면.
- **Game Selector**: 5개의 미니게임(Clicker, Dodge, Tower, Fly, Stopwatch)을 버튼 클릭 한 번으로 진입 가능.
- **Navigation**: 어떤 게임을 플레이하든 상단의 '홈(Home) 버튼'을 통해 즉시 로비로 복귀 가능.

### 💻 스크립트 로직 & 구조 (Scripting & Logic)
- **`LobbyManager.cs`**:
  - **Game Management**: `GameObject[] games` 배열을 사용하여 5개의 게임 컨테이너를 인덱스(`0~4`)로 관리.
  - **Switching Logic**:
    - `OpenGame(int index)`: 선택된 인덱스의 게임 오브젝트만 `SetActive(true)`로 활성화하고, 나머지와 로비는 비활성화.
    - `GoToLobby()`: 실행 중인 모든 게임을 비활성화하고, 메인 로비 패널만 다시 활성화.
  - **State Reset**: 로비 복귀 시 `Time.timeScale = 1.0f`을 호출하여, 일시정지(Game Over) 상태였던 게임의 시간 설정을 정상화.

### 📂 파일 구조 및 변경 사항
- **New Scripts**: `LobbyManager.cs`
- **UI Components**:
  - `Grid Layout Group`: 버튼 5개를 균일한 간격으로 자동 정렬하기 위해 사용.
- **Hierarchy 구조 변경 (Final Structure)**:
  - `Panel_Super`
    ├── `Lobby_Main` (메인 메뉴 패널)
    ├── `Btn_BackToLobby` (전역 뒤로가기 버튼)
    ├── `Game_GemClicker`
    ├── `Game_DodgeRain`
    ├── `Game_TowerStack`
    ├── `Game_FlyJump`
    └── `Game_Stopwatch`

## [2026.01.06] (화) [3차] - 유니티 미니게임 5호: Just 10 Seconds 개발

### 🎯 오늘의 목표 (Daily Goal)
- `Time.deltaTime`을 활용한 정밀 시간 측정 로직과 `Mathf.Abs`를 이용한 오차 범위 판정 시스템 구현.
- 게임의 상태(Menu/In-Game)를 UI 그룹(`Group`) 단위로 관리하는 UI 구조 설계.

### 🎮 게임 설명 (Game Description)
- 흐르는 시간을 보며 정확히 **10.00초**에 버튼을 눌러 멈추는 타이밍 게임.
- **Normal Mode**: 숫자를 끝까지 보여주는 연습 모드.
- **Blind Mode**: 3초 이후 숫자를 가리고 오직 감각에 의존해야 하는 실전 모드. (작동 중임을 알리기 위해 숫자가 랜덤으로 뒤섞이는 시각적 연출 포함).

### 💻 스크립트 로직 & 구조 (Detailed Logic)
- **`StopwatchGame.cs`**:
  - **Time Management**: `Update` 문에서 `currentTime += Time.deltaTime`으로 시간을 누적 계산. `ToString("F2")`를 사용하여 소수점 2자리까지 UI 표시.
  - **Blind Effect**: 블라인드 모드 활성화 시, 3초 경과 후 `Random.Range`를 이용해 숫자를 무작위로 셔플(Shuffle)하여, 시간은 흐르고 있음을 시각적으로 피드백.
  - **Victory Condition**: `Mathf.Abs(currentTime - targetTime) <= tolerance(0.05f)` 공식을 사용하여, 목표 시간과의 오차가 0.05초 이내일 경우 성공 판정.
  - **UI State Machine**:
    - **Menu State**: 모드 선택 버튼(`Btn_Normal`, `Btn_Blind`) 활성화.
    - **Game State**: 타이머 및 액션 버튼 활성화.
    - **Action Logic**: 하나의 버튼으로 `Start` -> `Stop` -> `Result/Reset`의 3단계 상태를 순환 처리.

### 📂 파일 구조 및 변경 사항
- **New Scripts**: `StopwatchGame.cs`
- **Hierarchy 구조 개선 (UI Grouping)**:
  - `Game_Stopwatch` (Root)
    ├── `Group_Menu` (모드 선택 화면: Normal / Blind)
    └── `Group_Game` (게임 플레이 화면: Timer / Result / Action Button)


## [2026.01.06] (화) [2차] - 유니티 미니게임 4호: Fly Jump 개발

### 🎯 오늘의 목표 (Daily Goal)
- 중력 가속도(Gravity)와 점프(Velocity) 물리를 코드로 구현한 'Flappy Bird' 스타일 게임 개발.
- 점수에 따라 게임 속도와 장애물 생성 패턴이 변화하는 '동적 난이도 시스템' 구축.

### 🎮 게임 설명 (Game Description)
- 중력에 의해 떨어지는 캐릭터를 터치하여 점프시키고, 다가오는 파이프 사이의 구멍을 통과하는 게임.
- **Dynamic Difficulty**: 점수가 오를수록 파이프 이동 속도가 빨라지고, 생성 간격이 불규칙하게 줄어들어 긴장감을 유발함.

### 💻 스크립트 로직 & 구조 (Detailed Logic)
- **`FlyManager.cs` (Central Control)**:
  - **Global Speed**: `currentPipeSpeed` 변수를 통해 게임 전체의 속도를 중앙에서 관리. 점수 획득 시 속도를 점진적으로 증가(Max 1000).
  - **Position Fix**: 게임 재시작 시 플레이어 위치를 `-550`으로 강제 초기화하여 안정적인 시작 지점 확보.
  - **UI & State**: 점수, 최고 기록(Key: `FlyBest`), 게임 오버 팝업 관리.

- **`FlyPlayer.cs` (Physics)**:
  - **Custom Gravity**: Unity Rigidbody 대신 `velocity -= gravity * Time.deltaTime` 공식을 사용하여 UI 환경에 최적화된 물리 움직임 구현.
  - **Boundaries**: 화면 상단/하단 이탈 시 사망 처리.

- **`FlyPipe.cs` (Obstacle)**:
  - **Synced Movement**: 자체 속도 대신 `FlyManager`의 속도 변수를 참조하여 이동(동기화).
  - **Math Collision**: Collider 컴포넌트 없이 `Mathf.Abs`를 활용하여 가로/세로 거리 차이로 충돌 및 통과(점수) 여부 정밀 판정.
  - **Tuning**: 플레이어의 통과 난이도를 고려하여 충돌 판정 범위를 `250`으로 미세 조정.

- **`FlySpawner.cs` (Pattern)**:
  - **Irregular Interval**: `기본 간격 - (점수 비례 감소) + 랜덤 딜레이` 공식을 적용하여, 예측 불가능한 엇박자 생성 패턴 구현.
  - **Random Position**: 생성 시 Y축 좌표를 랜덤으로 변경하여 구멍 위치 다양화.

### 📂 파일 구조 및 변경 사항
- **New Scripts**: `FlyPlayer.cs`, `FlyPipe.cs`, `FlySpawner.cs`, `FlyManager.cs`
- **Prefabs**: `PipeSet` (Top/Bottom 파이프를 하나의 세트로 묶고, 길이를 2000으로 늘려 화면 공백 제거).
- **Hierarchy**: `Game_FlyJump` 그룹 생성 및 기존 UI 구조(`Fly_UI`) 재사용.

## [2026.01.06] (화) [1차] - 유니티 미니게임 3호: Tower Stacker 개발

### 🎯 오늘의 목표 (Daily Goal)
- `Mathf.PingPong`을 활용한 자동 이동 로직과 정밀한 좌표 판정 시스템을 갖춘 'Tower Stacker' 개발.
- UI와 게임 오브젝트의 계층(Hierarchy) 분리를 통해 화면 스크롤 시 UI가 고정되도록 구조 개선.

### 🎮 게임 설명 (Game Description)
- 좌우로 자동 왕복하는 블록을 타이밍에 맞춰 클릭하여 정지시키고, 이전 블록 위에 정확히 쌓아 올리는 피지컬 게임. 층수가 높아질수록 블록이 작아지고 속도가 빨라지며, 15층 이상 쌓으면 화면이 아래로 스크롤됨.

### 💻 스크립트 로직 & 구조 (Scripting & Logic)
- **`TowerBlock.cs`**:
  - `Mathf.PingPong` 함수를 사용하여 별도의 방향 전환 조건문(if) 없이 부드러운 왕복 이동 구현.
  - `Init()` 함수를 통해 생성 시 매니저로부터 속도, 크기, 색상 정보를 주입받도록 설계.
- **`TowerManager.cs`**:
  - **Core Loop**: 블록 생성 -> 정지 -> 판정 -> 다음 레벨(스크롤/난이도 상승) -> 재생성.
  - **Judgment Logic**: `Mathf.Abs(현재X - 이전X)`를 계산하여 블록 너비를 벗어나면 Game Over 처리.
  - **Scrolling**: 점수가 15점 이상일 때 `Container`의 Y좌표를 이동시켜 탑이 계속 쌓이는 시각적 효과 구현.

### 📂 파일 구조 및 변경 사항
- **New Scripts**: `TowerBlock.cs`, `TowerManager.cs`
- **Prefabs**: `Block` (TowerBlock 스크립트 포함)
- **Hierarchy 구조 개선**:
  - `Game_TowerStack` (Root)
    ├── `Tower_MovingArea` (블록이 쌓이고 움직이는 영역)
    └── `Tower_UI` (점수판 등 고정된 UI 영역)


## [2026.01.05] (월) [2차] - 유니티 미니게임 2호: Dodge Rain 개발

### 🎯 오늘의 목표 (Daily Goal)
- 오브젝트 생성(Instantiate), 이동(Translate), 충돌(Collision) 로직을 활용한 아케이드 회피 게임 'Dodge Rain' 개발.
- 게임 매니저(Singleton 패턴)를 도입하여 게임 상태 관리 및 난이도 조절 구현.

### 🎮 게임 설명 (Game Description)
- 하늘에서 랜덤하게 떨어지는 빗방울을 좌우 이동(PC: 방향키 / Mobile: 터치)으로 피하는 생존 게임. 시간이 지날수록(점수가 오를수록) 비가 내리는 속도가 빨라지며, 로컬 최고 기록(Best Score) 갱신 시스템을 포함함.

### 💻 스크립트 로직 & 구조 (Scripting & Logic)
- **`DodgePlayer.cs`**:
  - `Input.GetAxisRaw`와 `Input.GetMouseButton`을 혼합하여 PC/모바일 통합 컨트롤러 구현.
  - `Mathf.Clamp`로 플레이어의 이동 범위를 화면 내로 제한.
- **`RainSpawner.cs` & `RainMovement.cs`**:
  - Prefab을 활용한 오브젝트 풀링 기초(생성 및 파괴).
  - 점수(`score`)에 비례하여 생성 주기(`spawnInterval`)가 짧아지는 난이도 알고리즘 적용.
  - `Vector3.Distance`를 이용한 충돌 감지 및 게임 오버 처리.
- **`DodgeManager.cs` (Singleton)**:
  - 게임의 시작, 재시작(Soft Reset), 점수 관리, UI 갱신을 총괄하는 중앙 관리자.
  - `PlayerPrefs`를 활용하되, 1차 게임과 키(`Key`)를 분리하여 독립적인 최고 기록 저장 구현.

### 📂 파일 구조 및 변경 사항
- **New Scripts**: `DodgePlayer.cs`, `RainSpawner.cs`, `RainMovement.cs`, `DodgeManager.cs`
- **Prefabs**: `Rain` (빗방울 오브젝트 프리팹화)
- **Hierarchy**: `Panel_Super` > `Game_DodgeRain` 그룹화 (1차 게임과 분리 운영).

## [2026.01.05] (월) [1차] - 유니티 미니게임 1호: Super Gem Clicker 개발

### 🎯 오늘의 목표 (Daily Goal)
- 유니티 기초 UI 학습 및 첫 번째 미니게임 'Super Gem Clicker' (방치형 클릭커) MVP 개발 완료.
- 게임 오브젝트 구조화 및 로컬 데이터 저장 구현.

### 🎮 게임 설명 (Game Description)
- 화면 중앙의 타겟을 반복 클릭하여 재화(Gem)를 획득하고, 모은 재화로 클릭 효율을 업그레이드하며 로컬 데이터에 저장된 최고 기록(High Score)에 도전하는 단순 중독성 클리커 게임.

### 💻 스크립트 로직 & 구조 (Scripting & Logic)
- **`ClickerGame.cs` 스크립트 구현**:
  - **Core Logic**: 버튼 클릭 시 점수(`score`) 증가, 텍스트 UI 실시간 갱신.
  - **Upgrade System**: 재화 소비를 통한 클릭 효율(`clickPower`) 증가 및 비용(`upgradeCost`) 증가 알고리즘 적용.
  - **Data Persistence**: `PlayerPrefs`를 활용하여 앱 재실행 시에도 최고 기록(`HighScore`)이 유지되도록 구현.

- **UI/UX 구조 (Hierarchy)**:
  - `Panel_Super` 하위에 `Game_GemClicker` 컨테이너를 생성하여 게임 단위로 모듈화.
  - `TextMeshPro`를 사용하여 가독성 높은 UI 구성.

### 📂 파일 구조 변경
- `Assets/_Scripts/ClickerGame.cs` 생성.
- Hierarchy: `Panel_Super` > `Game_GemClicker` (Toggle 가능하도록 그룹화).


## [2025.12.29] (월) [2차] - 화면 내비게이션 및 스크롤 구현

### 🎯 오늘의 목표 (Daily Goal)
- 버튼 클릭을 통한 화면 전환(Navigation) 기능 및 피드 콘텐츠의 스크롤 기능 구현.

### 💻 스크립트 로직 (Scripting & Logic)
- **`PageNavigator.cs` 스크립트 신규 추가**:
  - `targetScreen`과 `currentScreen` 변수를 `public`으로 선언하여 인스펙터에서 제어할 패널을 할당 가능하게 함.
  - `GoToPage()` 함수: `currentScreen`을 비활성화하고 `targetScreen`을 활성화하여 화면 전환을 처리.
  - `GoBack()` 함수: 현재 오브젝트(`gameObject`)를 비활성화하여 팝업이나 서브 화면을 닫는 기능 구현.

### 🎨 UI 및 연출 (UI & Visuals)
- **버튼 내비게이션 연결**:
  - 주요 UI 버튼에 `PageNavigator` 컴포넌트를 추가하고 `OnClick` 이벤트에 `GoToPage()` 함수를 연결하여 화면 간 이동이 가능하도록 설정함.
- **스크롤 뷰 구현**:
  - `Feed_ScrollContent` 오브젝트에 `ScrollRect` 컴포넌트를 추가하여, 콘텐츠가 화면을 벗어날 경우 터치 스크롤이 가능하도록 구현함.

### 🐛 이슈 해결 (Troubleshooting)
- 특이사항 없음.

## [2025.12.29] (월) [1차] - 피그마 디자인 임포트

### 🎯 오늘의 목표 (Daily Goal)
- Figma로 제작된 모바일 앱 디자인 전체를 Unity 프로젝트로 임포트하여 UI 기반 구축.

### 💻 스크립트 로직 (Scripting & Logic)
- **`UnityFigmaBridge` 플러그인 활용**:
  - [UnityFigmaBridge](https://github.com/simonoliver/UnityFigmaBridge) 라이브러리를 프로젝트에 도입하여 Figma 데이터 연동 환경을 구축함.
  - 플러그인의 기능을 활용하여 Figma의 데이터 구조를 Unity의 UI 시스템으로 변환하는 작업을 수행함.

### 🎨 UI 및 연출 (UI & Visuals)
- **전체 앱 디자인 임포트 완료**:
  - [Figma Prototyping Kit](https://www.figma.com/design/RPmavLkRUOOIqZA3bfIHCj/Mobile-Apps-%E2%80%93-Prototyping-Kit--Community-?node-id=283-5892&t=onZM6b5Sz6xHizOc-0)에 포함된 모든 스크린과 UI 컴포넌트를 유니티 씬으로 가져옴.
  - Figma의 프레임과 레이어가 Unity의 `Canvas`, `Panel`, `Image`, `Text` 등으로 자동 변환되어 `Hierarchy`에 배치됨.
  - 디자인 원본의 레이아웃과 스타일을 유지하며 유니티 환경으로 이식 완료.

### 🐛 이슈 해결 (Troubleshooting)
- 특이사항 없음.

## [2025.12.12] (금) [2차] - Lottie 애니메이션 프레임 제한 해제

### 🎯 오늘의 목표 (Daily Goal)
- Lottie 애니메이션 재생 성능 향상을 위해 프레임 제한 제거.

### 💻 스크립트 로직 (Scripting & Logic)
- **`LottieFilterFix.cs` 업데이트**:
  - `Start()` 함수 내에 **`Application.targetFrameRate = 60;` 코드를 추가**하여, 애플리케이션의 목표 프레임 레이트를 60FPS로 설정했습니다.
  - 이는 기존의 잠재적인 프레임 제한을 해제하고 애니메이션이 더 부드럽게 재생되도록 하는 목적을 가집니다.

### 🎨 UI 및 연출 (UI & Visuals)
- Lottie 애니메이션 및 전반적인 UI의 움직임이 60FPS로 더욱 부드럽게 표현됩니다.

### 🐛 이슈 해결 (Troubleshooting)
- 특정 환경에서 발생할 수 있는 애니메이션의 부자연스러운 끊김 현상을 개선했습니다.

## [2025.12.12] (금) [1차] - Lottie 화질 개선

### 🎯 오늘의 목표 (Daily Goal)
- Lottie 애니메이션 재생 시 발생하는 텍스처 깨짐(Aliasing) 현상 해결.

### 💻 스크립트 로직 (Scripting & Logic)
- **`LottieFilterFix.cs` 스크립트 신규 추가**:
  - `RawImage` 컴포넌트에서 재생되는 Lottie 텍스처의 `filterMode`를 감시하는 기능을 구현함.
  - 텍스처 필터 모드가 `Point`로 설정되어 있을 경우, 이를 **`Bilinear`로 강제 변경**하여 텍스처가 부드럽게 표시되도록 함.
  - 이 스크립트는 Lottie를 사용하는 `RawImage` 오브젝트에 추가하여 화질 저하 문제를 자동으로 보정하는 역할을 함.
  - 화질 보정이 한 번 완료되면 `Update` 함수가 더 이상 동작하지 않도록 하여 불필요한 부하를 방지함.

### 🎨 UI 및 연출 (UI & Visuals)
- Lottie 애니메이션이 기존보다 부드럽고 선명하게 보이도록 시각적 품질을 개선함.

### 🐛 이슈 해결 (Troubleshooting)
- Lottie-Unity 플러그인이 `RawImage`에 텍스처를 할당할 때 기본 필터 모드를 `Point`로 설정하여 발생하는 계단 현상(깨진 것처럼 보이는 문제)을 해결함.

## [2025.12.11] (목) [3차] - 토르X법사

### 🎯 오늘의 목표 (Daily Goal)
- Lottie/TGV 스포너 및 링커 로직 개선 및 ThorVG 해상도 이슈 해결을 위한 다양한 플러그인 테스트 진행.

### 💻 스크립트 로직 (Scripting & Logic)
- **`LottieSpawner.cs`, `MultiSpawner.cs`**: 스포너 관련 로직을 수정하여 인스턴스 생성 및 관리 효율을 개선했습니다.
- **`SlaveLinker.cs`**: 원본-복제본 간의 데이터 연결 로직을 수정하여 안정성을 높였습니다.
- **`TvgToRawImage.cs`**: ThorVG 플러그인의 렌더링 결과(Texture)를 RawImage에 표시하는 기능을 유지하며, 해상도 저하 문제 해결을 위해 다른 플러그인과의 비교 테스트 기반을 마련했습니다.

### 🐛 이슈 및 진행 상황 (Issues & Progress)
- **해상도 저하 문제**: ThorVG 플러그인 사용 시 발생하는 해상도 문제를 해결하기 위해, 현재 다양한 벡터 그래픽 렌더링 플러그인을 조사하고 테스트하고 있습니다.

## [2025.12.11] (목) [2차]

### 🎯 오늘의 목표 (Daily Goal)
- ThorVG 렌더링 엔진의 결과를 Unity UI(`RawImage`)에 실시간으로 표시하는 기능 구현.

### 💻 스크립트 로직 (Scripting & Logic)
- **`TvgToRawImage.cs` 스크립트 신규 추가**:
  - `MeshRenderer`에서 렌더링된 `Texture`를 `RawImage` UI 컴포넌트로 실시간 복사하는 로직을 구현함.
  - `Start()`에서 `targetUI` (`RawImage`)와 `thorEngine` (`MeshRenderer`) 컴포넌트를 자동으로 탐색하여 할당.
  - `Update()`에서 `thorEngine`의 `sharedMaterial.mainTexture`를 지속적으로 확인하여 `targetUI.texture`에 연결함.
  - 3D 씬에서는 렌더링 소스가 보이지 않도록 `thorEngine.enabled = false`로 설정하여, UI 전용 렌더링 파이프라인으로 작동하도록 함.

### 🎨 UI 및 연출 (UI & Visuals)
- `TvgToRawImage` 컴포넌트를 통해, 보이지 않는 3D 객체의 렌더링 결과를 UI에 표시할 수 있는 기반을 마련함.
- 이 기능을 활용할 `RawImage` UI 요소가 `Hierarchy` 상에 준비되어야 함.

### 🐛 이슈 해결 (Troubleshooting)
- 해당 사항 없음.

## 2025년 12월 11일 [1차]

### 🎯 오늘의 목표 (Daily Goal)
- ThorVG 인스턴싱 기반의 Lottie 대량 생성 및 렌더링 최적화 1차 테스트.

### 💻 스크립트 로직 (Scripting & Logic)
- **Lottie 생성 로직 변경**: `LottieSpawner`를 `LottieGridManager.cs`로 리팩토링.
  - 기존의 개별 `Instantiate` 방식 대신, ThorVG 플러그인의 인스턴싱 기능을 활용하도록 코드 수정.
  - 단 하나의 Lottie 객체만 `Load()` 및 `Play()` 하고, 나머지 객체들은 해당 렌더링 결과를 복제하여 화면에 표시하는 방식으로 구현.
- **`LottieLinker.cs` 추가**:
  - `LottieGridManager`가 생성한 복제 인스턴스들이 원본 Lottie 객체의 렌더링 상태를 참조하도록 연결하는 역할.

### 🎨 UI 및 연출 (UI & Visuals)
- **Lottie 플러그인 교체**: 프로젝트의 Lottie 렌더링 플러그인을 기본(Lottie-Unity)에서 `ThorVG`로 변경.
  - 이는 렌더링 인스턴싱 기능을 사용하기 위함.
- **스트레스 테스트 환경 구성**:
  - `LottieGridManager`를 통해 대량의 Lottie 애니메이션(복제본)을 `GridLayoutGroup` 내에 배치.

### 🐛 이슈 해결 (Troubleshooting)
- **1차 구현의 한계점 명확화**:
  - 현재 방식은 모든 인스턴스가 동일한 애니메이션 프레임을 공유하는 '복제'이므로, 다양한 애니메이션을 동시에 테스트하는 '정석적인' 스트레스 테스트에는 적합하지 않음을 확인함.
  - 현 상태를 GitHub에 [1차] 버전으로 백업 후, 추후 개별 제어가 가능한 방식으로 2차 구현을 진행할 예정.

## [2025.12.10] (수)

### 🎯 오늘의 목표 (Daily Goal)
- 탭 전환 시 Lottie 애니메이션 대량 생성을 통한 앱 스트레스 테스트 및 최적화 가능성 점검.

### 💻 스크립트 로직 (Scripting & Logic)
- **Lottie 스폰(spawn) 시스템 구현**: `LottieSpawner.cs` 작성.
  - `SpawnLottie()` 함수를 통해 지정된 `lottieAnimation` 프리팹을 `spawnParent` 내부에 대량으로 생성.
  - 생성된 각 인스턴스의 `LottieAnimationView` 컴포넌트에 `lottieFile`을 할당하고, `Load()` 및 `Play()`를 호출하여 즉시 재생되도록 구현함.
- **테스트용 이벤트 전달 시스템**: `LottieTransmitter.cs` 작성.
  - 유니티 에디터에서 버튼 클릭 시 `OnSpawnLottie` 이벤트를 호출.
  - `UnityEvent`를 사용하여 `LottieSpawner`의 `SpawnLottie()` 함수와 에디터상에서 연결. 이로써 코드 수정 없이 테스트를 수행할 수 있는 구조를 마련함.

### 🎨 UI 및 연출 (UI & Visuals)
- **Lottie 프리팹 준비**: `Anim_Lego.prefab`을 테스트용 Lottie 애니메이션 프리팹으로 사용.
  - 내부에 `LottieAnimationView` 컴포넌트가 포함되어 있으며, `Play On Enable`은 비활성화하여 스크립트로 재생을 제어하도록 설정함.
- **UI 구성**:
  - `LottieTransmitter`가 포함된 테스트용 버튼을 씬에 배치.
  - `LottieSpawner`의 `spawnParent`로 `GridLayoutGroup`이 적용된 `GameObject`를 연결하여, 생성된 Lottie 애니메이션들이 격자 형태로 자동 정렬되도록 구성.

### 🐛 이슈 해결 (Troubleshooting)
- **과도한 생성으로 인한 앱 프리징(Freezing)**:
  - 초기 테스트에서 한 번에 100개 이상의 Lottie 파일을 생성하자 UI 스레드가 멈추는 현상 발생.
  - 이는 `Instantiate`와 `Load`가 동기적으로 처리되어 프레임 드랍을 유발하기 때문으로 분석됨. 향후 코루틴을 사용한 순차적 생성 또는 오브젝트 풀링(Object Pooling) 도입의 필요성을 확인함.

## [2025.12.08] (월)

### 🎯 오늘의 목표 (Daily Goal)
- UI 연출 강화: 씬 전환 시 가짜 로딩과 특정 이벤트용 콘페티(폭죽) 효과 구현.

### 💻 스크립트 로직 (Scripting & Logic)
- **`SceneConnector.cs` 추가**: 씬 전환 시 부드러운 사용자 경험을 위한 가짜 로딩 화면을 구현함.
  - `GoToMainScene()` 함수가 호출되면 `ProcessFakeLoading()` 코루틴을 실행.
  - 코루틴은 `loadingPanel`을 활성화해 로딩 애니메이션을 보여주고, `yield return new WaitForSeconds(1f)`로 1초간 대기 후 `SceneManager.LoadScene("DuoMain")`으로 실제 씬을 로드함.
- **`ConfettiEffect.cs` 추가**: 퀘스트 완료 등 특정 이벤트에 사용할 콘페티(폭죽) 효과를 제어함.
  - `Fire()` 함수를 통해 `ProcessConfetti()` 코루틴을 실행.
  - 코루틴은 `confettiPanel`을 2초간 활성화했다가 비활성화하는 방식으로, `OnEnable` 시 자동 재생되는 애니메이션을 제어하고 리소스를 자동 정리함.

### 🎨 UI 및 연출 (UI & Visuals)
- **가짜 로딩 연출**: `SceneConnector`와 연동될 `loadingPanel` 오브젝트를 `Hierarchy`에 구성. 내부에 로딩 애니메이션(예: Lottie)을 포함하여 씬 전환 동안 시각적 피드백을 제공함.
- **콘페티 효과 연출**: `ConfettiEffect`와 연동될 `confettiPanel` 오브젝트를 `Hierarchy`에 구성. 파티클 시스템 또는 프레임 애니메이션을 배치하여 이벤트 성공 시 축하 효과를 보여줌.

### 🐛 이슈 해결 (Troubleshooting)
- 특이사항 없음.

## [2025.12.03] (수)

### 🎯 오늘의 목표 (Daily Goal)
- 리그 순위를 표시하는 리더보드 UI 및 데이터 연동 기능 구현.

### 💻 스크립트 로직 (Scripting & Logic)
- **리더보드 동적 생성**: `LeagueManager.cs` 스크립트 작성.
  - `Start()` 시점에 `CreateRankingList` 함수를 호출하여 랭킹 목록을 자동 생성.
  - `Instantiate`를 통해 `rankItemPrefab`을 `contentArea` 내부에 순차적으로 생성함.
  - 점수는 1등을 기준으로 `Random.Range`를 사용해 차감하는 방식으로 부여하여, 실제 랭킹과 유사한 데이터 분포를 시뮬레이션함.
- **개별 랭킹 아이템 제어**: `RankItem.cs` 스크립트 작성.
  - `SetData(rank, name, score, totalXP)` 함수를 `public`으로 선언하여, `LeagueManager`에서 각 `Prefab`의 UI(`TextMeshPro`)를 제어할 수 있도록 함.
- **순위에 따른 시각적 처리**:
  - `RankItem.cs`: 등수가 3등 이내일 경우 등수 텍스트(`textRank`) 색상을 금색으로, 4등 이상은 회색으로 변경하여 상위 랭커를 시각적으로 구분함.
  - `LeagueManager.cs`: 특정 순위(7등)를 '나'로 가정하고, 해당 아이템의 배경 `Image` 색상을 연두색으로 변경하여 현재 플레이어의 위치를 강조하는 기능 구현.

### 🎨 UI 및 연출 (UI & Visuals)
- **랭킹 아이템 Prefab 활용**: `Item_Rank.prefab`을 리더보드의 기본 단위로 사용.
  - `LeagueManager`가 `ScrollView`의 `contentArea`에 `Item_Rank` 인스턴스들을 동적으로 채워넣는 구조로 UI를 구성함. 이는 `Vertical Layout Group`과 함께 사용될 것을 상정함.

### 🐛 이슈 해결 (Troubleshooting)
- **'나'의 순위 강조 로직의 의존성 문제**: `LeagueManager`의 하이라이트 기능은 `Item_Rank.prefab`의 최상위 루트에 `Image` 컴포넌트가 존재해야만 정상 작동함. `Hierarchy` 구조 변경 시 주의가 필요함을 인지함.

## [2025.12.02] (화)

### 🎯 오늘의 목표 (Daily Goal)
- '학습 시작 확인' 및 '콕 찌르기' 등 개별 팝업 기능 구현.

### 💻 스크립트 로직 (Scripting & Logic)
- `LessonPopup.cs`: '학습 시작' 팝업의 표시/숨김 로직 구현. `OpenPopup`, `ClosePopup`, `StartLesson` 함수를 `public`으로 만들어 유니티 에디터의 `Button` 컴포넌트와 연동.
- `PopupManager.cs`: '콕 찌르기' 기능의 `popupPoke` 팝업을 관리하는 스크립트 추가. `OpenPokePopup`, `ClosePokePopup` 등의 함수를 통해 팝업을 제어. 현재는 '콕 찌르기' 전용 로직으로 구성됨.

### 🎨 UI 및 연출 (UI & Visuals)
- **개별 팝업 UI 구성**: `LessonPopup`과 `PokePopup`에 해당하는 `GameObject`를 `Hierarchy`에 구성.
- **버튼 이벤트 연동**: 각 팝업 내의 버튼(시작, 닫기, 확인 등)에 `LessonPopup.cs`와 `PopupManager.cs`의 `public` 함수들을 `OnClick()` 이벤트로 연결하여 기능 활성화.

### 🐛 이슈 해결 (Troubleshooting)
- 특이사항 없음.

## [2025.12.01] (월)

### 🎯 오늘의 목표 (Daily Goal)
- 학습 경로(러닝맵)의 기본 UI 및 동적 생성 기능 구현.

### 💻 스크립트 로직 (Scripting & Logic)
- **러닝맵 자동 생성 기능 추가**: `MapGenerator.cs` 스크립트 작성.
  - `Mathf.Sin` 함수를 이용해 유닛 버튼이 지그재그(Sine 파형) 형태로 배치되도록 구현함.
  - `xAmplitude`와 `frequency` 변수를 통해 지그재그의 너비와 굴곡을 조절할 수 있도록 설계.
- **유닛 상태에 따른 시각적 분기 처리**:
  - 현재 학습 중인 유닛은 `activeColor`(핑크색)로, 아직 잠긴 유닛은 `lockedColor`(회색)로 표시.
  - `List<Sprite>`에 아이콘을 미리 할당해두고, 유닛 인덱스에 따라 다른 아이콘(`iconList`)이 표시되도록 설정.
- **에디터 편의 기능**: `[ContextMenu("Generate Map Now")]` 어트리뷰트를 추가하여, 에디터에서 바로 맵 생성을 테스트할 수 있도록 함.

### 🎨 UI 및 연출 (UI & Visuals)
- **유닛 Prefab 제작**: `Row_center.prefab`을 생성하여 러닝맵의 기본 단위로 사용.
  - 내부에 아이콘을 표시할 `Img_Main`과 버튼 컴포넌트를 포함.
- **ScrollView 연동**: `MapGenerator`가 `ScrollView`의 `contentParent` 내에 유닛들을 자동으로 생성하고 배치하도록 구성함.
- **퀘스트 패널 UI**:
  - 미션 내용을 표시할 기본 퀘스트 패널 UI의 와이어프레임을 `Hierarchy`에 구성함.
  - `Vertical Layout Group`을 사용하여 퀘스트 목록이 순차적으로 쌓이도록 구조를 잡음.

### 🐛 이슈 해결 (Troubleshooting)
- **맵 재생성 시 장식 요소가 삭제되는 문제 해결**:
  - 기존에는 `contentParent`의 모든 자식 오브젝트를 삭제하여, 수동으로 추가한 장식용 오브젝트까지 사라지는 문제가 있었음.
  - `MapGenerator.cs`의 청소 로직을 수정하여, 이름에 `"Row_Unit"`이 포함된 오브젝트만 선택적으로 삭제하도록 변경함.