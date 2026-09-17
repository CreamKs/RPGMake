# Week 01 - 기본 조작 및 전투 프로토타입

## 1. 이번 주 목표

첫 주에는 완성된 RPG를 만들기보다 **2D 횡스크롤 RPG의 핵심 조작과 기본 전투 루프가 Unity에서 정상적으로 구현 가능한지 기술적으로 검증하는 것**을 목표로 잡았습니다.

주요 목표는 다음과 같았습니다.

- Unity 6 프로젝트 및 기본 폴더 구조 구성
- Unity Input System 학습 및 적용
- 좌우 이동과 점프
- 원웨이 플랫폼과 아래 점프
- 캐릭터 애니메이션 연결
- 기본 공격 판정
- Enemy HP / 데미지 / 사망
- 데미지 숫자 및 간단한 타격 피드백
- 카메라 추적 및 화면 흔들림

---

## 2. 프로젝트 초기 설정

Unity 6의 Universal 2D 프로젝트로 시작했습니다.

Assets는 기능과 리소스를 구분하기 위해 다음과 같이 구성했습니다.

```text
Assets/
├─ Animations/
├─ Prefabs/
├─ Scenes/
├─ Scripts/
└─ Sprites/
```

Player는 처음에는 Square Sprite를 사용하여 물리와 이동을 먼저 검증한 뒤 실제 캐릭터 Sprite를 Visual 자식 오브젝트에 배치했습니다.

```text
Player
├─ Visual
├─ AttackPoint
└─ GroundCheck
```

이 구조를 사용하면서 물리 판정을 담당하는 Player와 화면에 표시되는 캐릭터 이미지를 분리했습니다.

---

## 3. Input System

Unity의 새로운 Input System을 사용했습니다.

`PlayerInputAction.inputactions`에 Player Action Map을 만들고 다음 Action을 구성했습니다.

```text
Move   : Value / Vector2
Jump   : Button
Attack : Button
```

Move는 방향키 Composite를 사용했고 현재 프로토타입 입력은 다음과 같습니다.

```text
← / →      이동
Alt        점프
↓ + Alt    아래 점프
Ctrl       기본 공격
```

Input Action에서 C# Class를 생성하여 `PlayerInputAction` 클래스를 통해 입력을 읽도록 구성했습니다.

### 배운 점

Unreal의 Enhanced Input에서 Input Action과 Mapping Context를 구성하는 것처럼, Unity에서도 키 입력을 코드에 직접 하드코딩하기보다 Input Action을 통해 입력과 게임 로직을 분리할 수 있었습니다.

---

## 4. Player 이동과 점프

Player에는 다음 Component를 사용했습니다.

```text
Rigidbody2D
BoxCollider2D
PlayerController
```

좌우 이동은 `Rigidbody2D.linearVelocity`의 X 값을 변경하고 기존 Y 속도는 유지하는 방식으로 구현했습니다.

점프 가능 여부는 Player 아래의 `GroundCheck`에서 `Physics2D.OverlapCircle`을 사용해 검사합니다.

초기에는 점프 직후에도 GroundCheck가 지면과 잠시 겹쳐 있어 공중에서 다시 점프 가능한 것처럼 판정되는 문제가 있었습니다.

이를 해결하기 위해 단순 충돌 검사뿐 아니라 Player의 Y 속도도 함께 검사했습니다.

```csharp
bool isGrounded = Physics2D.OverlapCircle(
    groundCheck.position,
    groundCheckRadius,
    groundLayer
) && rb.linearVelocity.y <= 0f;
```

즉, GroundCheck가 지면에 닿아 있더라도 Player가 위로 상승하고 있다면 Ground 상태로 인정하지 않습니다.

점프는 기획 의도에 따라 Alt를 누르고 있는 동안 착지 즉시 다시 점프할 수 있도록 `IsPressed()`를 사용했습니다.

---

## 5. 원웨이 플랫폼과 아래 점프

횡스크롤 RPG에서 아래에서 위로 통과할 수 있는 발판을 구현하기 위해 `PlatformEffector2D`를 사용했습니다.

Platform에는 별도의 Platform Layer를 지정하고 다음 구조로 사용했습니다.

```text
Platform
├─ BoxCollider2D
└─ PlatformEffector2D
```

`Used By Effector`와 `Use One Way`를 활성화하여 아래에서는 통과하고 위에서는 착지할 수 있도록 했습니다.

### 아래 점프

↓ + Alt 입력 시 Player와 현재 밟고 있는 Platform 사이의 충돌을 잠시 무시합니다.

전체 Layer 충돌을 끄는 대신 `Physics2D.IgnoreCollision`을 이용해 **현재 Player와 해당 Platform Collider 사이의 충돌만** 일시적으로 해제했습니다.

일정 시간이 지나면 Coroutine을 통해 충돌을 다시 활성화합니다.

또한 ↓를 누르고 있을 때 일반 점프 로직까지 실행되는 것을 막기 위해 아래 점프 입력을 처리한 뒤 Update 흐름을 종료하도록 구성했습니다.

---

## 6. 캐릭터 Sprite 및 Animation

무료 Warrior Sprite Set을 사용하여 프로토타입 캐릭터를 구성했습니다.

Sprite의 Pixels Per Unit을 32로 통일하고 Player 아래의 `Visual` 오브젝트에서 Sprite Renderer와 Animator를 관리했습니다.

사용한 주요 Animation State는 다음과 같습니다.

```text
Idle
Run
Jump
Fall
Attack
```

Animator Parameter:

```text
Speed      : Float
JumpSpeed  : Float
Attack     : Trigger
```

좌우 이동 시 Player 전체 Transform을 뒤집지 않고 `Visual.localScale.x`만 변경했습니다.

이 방식으로 Rigidbody2D, Collider2D, GroundCheck 등의 물리 구조에는 영향을 주지 않고 그래픽 방향만 변경할 수 있었습니다.

---

## 7. 공격 Animation과 실제 타격 시점 분리

처음에는 Ctrl 입력 순간에 데미지를 처리할 수도 있었지만, 그렇게 하면 검이 적에게 닿기 전에 데미지가 발생할 수 있습니다.

따라서 다음 흐름으로 구현했습니다.

```text
Ctrl 입력
    ↓
Attack Trigger
    ↓
Attack Animation 재생
    ↓
검이 실제로 닿는 Frame
    ↓
Animation Event - HitAttack()
    ↓
실제 공격 판정
```

`Visual`에는 `PlayerAnimationEvents`를 추가하여 Animation Event를 Player의 전투 코드와 연결했습니다.

공격 애니메이션 마지막에는 `EndAttack()` Event를 배치하여 공격 상태를 해제했습니다.

또한 `isAttacking` 변수를 사용하여 공격 중 Ctrl 연타로 Attack Animation이 처음부터 계속 재시작되는 현상을 막았습니다.

---

## 8. 공격 판정

Player 아래에 `AttackPoint`를 두고 `Physics2D.OverlapCircleAll`을 이용해 공격 범위를 검사했습니다.

```text
Player
├─ Visual
├─ AttackPoint
└─ GroundCheck
```

Enemy Layer만 검사하도록 LayerMask를 사용했습니다.

공격 방향이 바뀌면 Visual뿐 아니라 AttackPoint의 Local Position X도 반대로 변경하여 실제 공격 판정 위치가 캐릭터가 바라보는 방향을 따라가도록 했습니다.

### 현재 공격 흐름

```text
HitAttack()
    ↓
AttackPoint 주변 Collider 검색
    ↓
Enemy Layer 확인
    ↓
EnemyHealth 검색
    ↓
TakeDamage()
```

이 구조를 통해 PlayerCombat은 '누구를 맞혔는가'를 판정하고 EnemyHealth는 '얼마나 피해를 받고 죽었는가'를 담당하도록 역할을 나누었습니다.

---

## 9. Enemy HP와 사망 처리

테스트 Enemy는 Square Sprite로 구성했습니다.

EnemyHealth에서 `maxHealth`와 `currentHealth`를 관리하고 공격을 받을 때마다 HP를 감소시킵니다.

현재 테스트 값은 다음과 같습니다.

```text
Enemy HP      : 100
Player Damage : 20
필요 공격 횟수 : 5회
```

HP가 0 이하가 되면 `Die()`를 호출합니다.

사망 시에는:

```text
isDead = true
Collider2D 비활성화
1초 후 Enemy GameObject 제거
```

순서로 처리합니다.

`isDead`를 추가한 이유는 HP가 이미 0이 된 Enemy가 추가 공격을 받아 계속 데미지 처리되는 것을 방지하기 위해서입니다.

현재 Enemy는 기술 검증용 Square이므로 Death Animation은 실제 몬스터 리소스를 적용할 때 추가하기로 결정했습니다.

---

## 10. 데미지 숫자

공격 적중 여부를 눈으로 확인하기 위해 데미지 숫자를 구현했습니다.

각 데미지 숫자는 독립된 World Space Canvas Prefab으로 구성했습니다.

```text
DamagePopup
└─ DamageText (TextMeshProUGUI)
```

Enemy 위에는 `DamagePopupPoint`를 두고 피격 시 해당 위치에 DamagePopup Prefab을 생성합니다.

DamagePopup은 생성 후 위로 천천히 이동하며 일정 시간이 지나면 스스로 제거됩니다.

이를 통해 테스트 단계에서도 공격 판정과 실제 데미지 적용 여부를 즉시 확인할 수 있게 되었습니다.

---

## 11. Camera Follow

Main Camera에 직접 작성한 `CameraFollow` Script를 사용했습니다.

현재 카메라 기능은 다음과 같습니다.

- Player 추적
- SmoothDamp 기반 부드러운 이동
- Map Min / Max를 이용한 카메라 이동 범위 제한
- Vertical Dead Zone
- Screen Shake

### Vertical Dead Zone

처음에는 카메라가 Player의 Y 위치를 계속 따라가도록 했습니다.

하지만 점프할 때마다 화면 전체가 위아래로 움직여 플레이 시 어지럽고 불안정한 느낌이 있었습니다.

따라서 Player가 일정 Y 범위를 벗어나기 전까지는 카메라 Y 위치를 유지하는 Vertical Dead Zone을 추가했습니다.

이 결정으로 일반적인 작은 점프에서는 배경이 고정되어 횡스크롤 게임에 더 적합한 화면 움직임을 얻을 수 있었습니다.

---

## 12. Hit Slow 실험

타격감을 확인하기 위해 공격 적중 순간 게임 전체의 `Time.timeScale`을 매우 짧게 낮추는 Hit Slow를 시험 구현했습니다.

`WaitForSecondsRealtime`을 사용하여 Time Scale이 낮아진 상태에서도 지정한 실제 시간이 지나면 정상 속도로 복구되도록 했습니다.

현재 이 기능은 **최종 채택이 확정된 기능이 아닙니다.**

피격 애니메이션, 타격 이펙트, 사운드, 넉백 등이 추가된 완성 단계에서 Hit Slow가 있는 버전과 없는 버전을 비교한 뒤 유지 여부를 결정할 예정입니다.

---

## 13. Screen Shake

공격이 실제 Enemy에게 적중했을 때만 짧은 Screen Shake가 발생하도록 구현했습니다.

CameraFollow에서 흔들림 시간과 세기를 받아 랜덤한 2D Offset을 최종 카메라 위치에 추가합니다.

Hit Slow가 적용되어도 흔들림 시간이 지나치게 늘어나지 않도록 흔들림 시간 계산에는 `Time.unscaledDeltaTime`을 사용했습니다.

허공 공격에는 Screen Shake가 발생하지 않고 실제 공격 판정이 성공했을 때만 실행되도록 구성했습니다.

현재 테스트 결과 Screen Shake는 타격 피드백 요소로 유지하기로 했습니다.

---

## 14. Week 01 주요 문제 해결

### 14.1 점프 직후 Ground로 잘못 판정

**문제**

GroundCheck가 지면과 겹쳐 있는 짧은 순간 때문에 Player가 상승 중인데도 Ground로 판정될 수 있었습니다.

**해결**

OverlapCircle 결과와 `rb.linearVelocity.y <= 0f` 조건을 함께 검사했습니다.

---

### 14.2 아래 점프 후 일반 점프까지 실행

**문제**

↓ + Alt로 Platform을 통과한 직후 Alt 입력이 일반 점프 조건에도 사용될 가능성이 있었습니다.

**해결**

Down 입력 상태에서는 아래 점프 로직을 우선 처리하고 Update에서 `return`하도록 흐름을 분리했습니다.

---

### 14.3 공격 연타 시 Animation 재시작

**문제**

Ctrl을 빠르게 연타하면 Attack Trigger가 반복되어 공격 Animation이 계속 처음부터 재생될 수 있었습니다.

**해결**

`isAttacking` 상태를 추가하고 Attack Animation 마지막 Animation Event에서 `EndAttack()`을 호출해 상태를 해제했습니다.

---

### 14.4 캐릭터 방향과 공격 판정 방향 불일치

**문제**

Visual만 좌우 반전하면 Player의 자식이지만 Visual 밖에 있는 AttackPoint는 자동으로 반전되지 않습니다.

**해결**

방향 전환 시 Visual Scale X와 함께 AttackPoint의 Local Position X도 반전시켰습니다.

---

### 14.5 점프 시 카메라가 과도하게 움직임

**문제**

Player의 Y 위치를 카메라가 계속 추적하면서 작은 점프에서도 화면 전체가 흔들리는 느낌이 발생했습니다.

**해결**

Vertical Dead Zone을 추가하여 Player가 일정 범위를 넘어갈 때만 카메라 Y가 이동하도록 변경했습니다.

---

## 15. Week 01 결과

첫 주가 끝난 시점에서 다음 플레이 루프가 동작합니다.

```text
Player 이동
    ↓
점프 / 플랫폼 이동
    ↓
Enemy 접근
    ↓
Ctrl 공격
    ↓
Attack Animation
    ↓
Animation Event
    ↓
AttackPoint 타격 판정
    ↓
Enemy HP 감소
    ↓
데미지 숫자
    ↓
Hit Slow(실험) + Screen Shake
    ↓
5회 피격
    ↓
Enemy 사망
```

단순한 테스트 Sprite 단계이지만 **캐릭터를 움직여 적을 공격하고, 공격 결과를 시각적으로 확인한 뒤 적을 처치하는 최소 전투 루프**를 완성했습니다.

---

## 16. 다음 단계

다음 주차부터는 1주차 프로토타입을 기반으로 전투 시스템을 확장합니다.

개발기준상 초기 기술 검증에서는 전사의 기본 공격과 몬스터 피격/사망 루프를 안정화한 뒤, 타격 피드백과 데이터 구조를 정리하고 Vertical Slice로 확장하는 것을 목표로 합니다.

Week 01에서 기능을 무리하게 늘리지 않고 현재 동작 상태를 Git에 남긴 뒤 다음 단계로 진행합니다.
