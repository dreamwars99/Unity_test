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