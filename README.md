# 2D 횡스크롤 싱글 RPG

Unity 6와 C#으로 제작 중인 Windows PC용 2D 횡스크롤 싱글 액션 RPG 프로젝트입니다.

옛날 메이플스토리 계열의 단순하고 직관적인 조작 감각을 참고하되, 포트폴리오에서는 게임의 규모보다 **조작감, 전투 피드백, 안정적인 시스템 구조와 문제 해결 과정**을 보여주는 것을 목표로 합니다.

## 개발 환경

- Engine: Unity 6
- Language: C#
- Platform: Windows PC
- Render: Universal 2D
- Version Control: Git / GitHub
- Input: Unity Input System

## 기본 조작

| 입력 | 기능 |
| --- | --- |
| 방향키 ← → | 좌우 이동 |
| 방향키 ↑ ↓ | 사다리/플랫폼 계열 조작 |
| Alt | 점프 |
| ↓ + Alt | 원웨이 플랫폼 아래 점프 |
| Ctrl | 기본 공격 |
| Space | 상호작용 / 포털 |
| Z | 아이템 줍기 |
| Q W E R A S D F | 퀵슬롯 |

현재 1주차 프로토타입에서는 이동, 점프, 아래 점프, 기본 공격을 우선 구현했습니다.

## 직업 구성

최종 기본 직업은 다음 3종을 계획하고 있습니다.

- 전사
- 궁수
- 마법사

현재 프로토타입은 전사 캐릭터를 기준으로 제작하고 있습니다.

## 현재 구현 상태 - Week 01

### Player

- Rigidbody2D 기반 좌우 이동
- Alt 점프
- Alt 키 홀드 시 연속 점프
- GroundCheck 기반 착지 판정
- 상승 중 지면으로 오인하지 않도록 Y 속도를 포함한 Ground 판정
- Platform Effector 2D 기반 원웨이 플랫폼
- ↓ + Alt 입력을 이용한 아래 점프
- Visual 오브젝트 분리를 통한 캐릭터 좌우 반전
- Idle / Run / Jump / Fall / Attack 애니메이션 연결
- Animation Event 기반 공격 타이밍 처리

### Combat

- AttackPoint 기반 원형 공격 범위
- LayerMask를 이용한 Enemy 판정
- Enemy HP 및 데미지 처리
- 월드 공간 데미지 숫자 표시
- HP가 0 이하가 되면 사망 처리
- 사망 후 추가 피격 방지
- 사망 시 Collider 비활성화 및 오브젝트 제거
- 실험용 Hit Slow
- 공격 적중 시 Screen Shake

### Camera

- 플레이어 추적
- SmoothDamp 기반 부드러운 이동
- 맵 경계 Clamp
- 점프 시 카메라가 과도하게 따라가지 않도록 Vertical Dead Zone
- 공격 적중 시 짧은 Screen Shake

## 현재 플레이 루프

```text
이동 / 점프
    ↓
몬스터 접근
    ↓
Ctrl 기본 공격
    ↓
Animation Event에서 실제 타격 판정
    ↓
Enemy HP 감소
    ↓
데미지 숫자 출력
    ↓
Hit Slow + Screen Shake
    ↓
HP 0
    ↓
Enemy 사망
```

## 프로젝트 구조

```text
Assets/
├─ Animations/
├─ Prefabs/
├─ Scenes/
├─ Scripts/
└─ Sprites/
```

현재 Player의 주요 Hierarchy 구조는 다음과 같습니다.

```text
Player
├─ Visual
├─ AttackPoint
└─ GroundCheck
```

## 개발 기록

주차별 구현 내용과 문제 해결 과정은 `Docs/DevLog`에 기록합니다.

- [Week 01 - 기본 조작 및 전투 프로토타입](Docs/DevLog/Week01.md)

## 개발 방향

초기 목표는 대규모 콘텐츠 제작이 아니라 핵심 플레이 루프를 먼저 검증하는 것입니다.

1. 조작이 즉각적이고 안정적으로 동작할 것
2. 공격 입력과 실제 타격 시점이 자연스럽게 일치할 것
3. 데미지 숫자, 카메라 반응 등 전투 피드백을 확인할 수 있을 것
4. 시스템을 이후 직업, 몬스터, 스킬로 확장할 수 있을 것

6개월 차에는 플레이 가능한 Vertical Slice 완성을 1차 목표로 하며, 필요 시 최대 12개월까지 콘텐츠와 완성도를 확장할 계획입니다.
