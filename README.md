# My project

## 프로젝트 개요

이 프로젝트는 Unity로 만들어진 간단한 시작 화면 애플리케이션입니다. '시작하기' 버튼을 누르면 메인 씬('DuoMain')으로 이동하며, 아직 구현되지 않은 '로그인' 기능도 포함하고 있습니다.

## 주요 기능

- **시작 화면:** '시작하기'와 '로그인' 버튼이 있는 기본 메뉴를 제공합니다.
- **씬 이동:** '시작하기' 버튼을 클릭하면 `DuoMain` 씬으로 전환됩니다. (`StartManager.cs`에서 관리)

## 실행 방법

1.  Unity Hub에서 이 프로젝트를 엽니다.
2.  `Assets/Scenes` 폴더에 있는 `SampleScene` 또는 다른 시작 씬을 엽니다.
3.  Unity 에디터의 재생 버튼(▶)을 눌러 프로젝트를 실행합니다.

## 프로젝트 종속성 (Packages)

이 프로젝트는 Unity 패키지 관리자를 통해 다음 패키지 및 모듈을 사용합니다. 전체 목록은 `Packages/manifest.json` 파일을 참고하세요.

-   `com.unity.ugui`: Unity UI 시스템
-   `com.unity.textmeshpro`: 고급 텍스트 렌더링
-   `com.unity.collab-proxy`: Unity Collaborate
-   `com.unity.timeline`: 시네마틱 타임라인 제어
-   기타 Unity 엔진 모듈 (Animation, Audio, Physics 등)
