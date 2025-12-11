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