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
