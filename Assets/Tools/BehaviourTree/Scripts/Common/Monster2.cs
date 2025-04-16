using Ironcow.BT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using MonsterSystem;

namespace MonsterSystem
{
    // 몬스터에 적용될 수 있는 모든 상태 효과의 기본 클래스
    public abstract class StatusEffect
    {
        protected float duration;        // 상태 효과의 총 지속 시간
        protected float remainingDuration; // 남은 지속 시간

        public StatusEffect(float duration)
        {
            this.duration = duration;
            this.remainingDuration = duration;
        }

        // 상태 효과가 적용될 때 호출되는 메서드
        public abstract void Apply(Monster2 target);
        // 상태 효과가 제거될 때 호출되는 메서드
        public abstract void Remove(Monster2 target);
        // 매 프레임마다 호출되는 업데이트 메서드
        public abstract void Update(Monster2 target);

        // 상태 효과의 지속 시간이 종료되었는지 확인
        public bool IsFinished()
        {
            return remainingDuration <= 0;
        }
    }
// 몬스터의 이동 속도를 감소시키는 상태 효과
    public class SlowEffect : StatusEffect
    {
        private float slowAmount;    // 감속 비율 (0~1 사이의 값)
        private float originalSpeed; // 원래 이동 속도

        public SlowEffect(float duration, float slowAmount) : base(duration)
        {
            this.slowAmount = slowAmount;
        }

        // 감속 효과 적용
        public override void Apply(Monster2 target)
        {
            originalSpeed = target.speed;
            target.speed = originalSpeed * (1 - slowAmount);
        }

        // 감속 효과 제거 및 원래 속도로 복구
        public override void Remove(Monster2 target)
        {
            target.speed = originalSpeed;
        }

        // 남은 시간 업데이트
        public override void Update(Monster2 target)
        {
            remainingDuration -= Time.deltaTime;
        }
    }
}

// 게임 내 몬스터의 기본 클래스
public class Monster2 : PoolBase
{
    [Header("Stats")]
    [SerializeField] private int MaxHp = 1000;  // 최대 체력
    public int Hp { get => enemyInfo.hp; set => enemyInfo.hp = value; }  // 현재 체력
    private EnemyInfo enemyInfo;  // 몬스터 정보
    public string rcode => enemyInfo.Data.rcode;  // 몬스터 코드

    [Header("Detection")]
    public float attackRange = 5f;    // 공격 가능 범위
    public float detectRange = 10f;   // 적 감지 범위
    private float maxChaseDistance = 15f;  // 최대 추적 거리

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;  // 스프라이트 렌더러
    [SerializeField] private Animator animator;  // 애니메이션 컨트롤러
    [SerializeField] private Rigidbody2D rb;  // 물리 컴포넌트
    [SerializeField] public float speed;  // 이동 속도
    [SerializeField] BTRunner bt;    // AI 로직을 위한 행동 트리 실행기

    [Header("Status Effects")]
    private List<StatusEffect> activeStatusEffects = new List<StatusEffect>();  // 활성화된 상태 효과 목록
    private bool isAggroed = false;  // 어그로 상태 여부

    private Vector3 originPos;  // 초기 위치
    private Collider2D[] colliders = new Collider2D[10];  // 충돌체 배열 (최적화용)
    private Collider2D nearbyTarget;  // 근처의 타겟


    //몬스터 초기화
    public void Init(EnemyData data)
    {
        enemyInfo = new EnemyInfo(data);
        originPos = transform.position;
        bt.SetActions(this);
        // UniRx를 사용하여 0.1초마다 행동 트리 실행
        Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ => bt.Operate())
            .AddTo(gameObject);
    }

    #region Status Effect Methods
    // 활성화된 상태 효과가 있는지 확인
    public bool HasStatusEffect()
    {
        return activeStatusEffects.Count > 0;
    }

    // 새로운 상태 효과 추가
    public void AddStatusEffect(StatusEffect effect)
    {
        activeStatusEffects.Add(effect);
        effect.Apply(this);
    }

    // 상태 효과 제거
    public void RemoveStatusEffect(StatusEffect effect)
    {
        if (activeStatusEffects.Contains(effect))
        {
            effect.Remove(this);
            activeStatusEffects.Remove(effect);
        }
    }

    // 상태 효과 확인 (행동 트리용)
    public eNodeState CheckStatusEffect()
    {
        return HasStatusEffect() ? eNodeState.success : eNodeState.failure;
    }

    // 상태 효과 처리 (행동 트리용)
    public eNodeState HandleStatusEffect()
    {
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeStatusEffects[i];
            effect.Update(this);
            
            if (effect.IsFinished())
            {
                RemoveStatusEffect(effect);
            }
        }
        return eNodeState.success;
    }
    #endregion

    // 몬스터 위치 설정
    public void SetPosition(Vector3 pos)
    {
        originPos = transform.position = pos;
    }

    // 데미지 처리
    public void OnDamage(int damage = 1)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            OnDead();
        }
    }

    // 사망 처리
    public void OnDead()
    {
        GameManager.Instance.ReleaseMonster(this);
        PoolManager.Instance.Release(this);
    }

    // 사망 행동 실행 (행동 트리용)
    public eNodeState DoDead()
    {
        GameManager.Instance.ReleaseMonster(this);
        PoolManager.Instance.Release(this);
        return eNodeState.success;
    }

    // 주변 적 탐색 (행동 트리용)
    public eNodeState FindEnemy()
    {
        if(Physics2D.OverlapCircleNonAlloc(transform.position, detectRange, colliders, 1 << LayerMask.NameToLayer("Character")) > 0)
        {
            var list = colliders.ToList();
            list.RemoveAll(obj => obj == null);
            nearbyTarget = list.OrderBy(col => (transform.position - col.transform.position).sqrMagnitude).First();
            return eNodeState.success;
        }
        else
        {
            nearbyTarget = null;
        }
        return eNodeState.failure;
    }

    // 공격 범위 내 적 확인 (행동 트리용)
    public eNodeState CheckEnemyWithAttackRange()
    {
        if (nearbyTarget == null) return eNodeState.failure;
        var dist = (transform.position - nearbyTarget.transform.position).sqrMagnitude;
        if (dist < attackRange)
        {
            rb.velocity = Vector3.zero;
            return eNodeState.success;
        }
        else
        {
            return eNodeState.failure;
        }
    }

    // 적 공격 실행 (행동 트리용)
    public eNodeState DoAttackEnemy()
    {
        rb.velocity = Vector3.zero;
        if (nearbyTarget != null)
        {
            spriteRenderer.flipX = (nearbyTarget.transform.position - transform.position).normalized.x < 0;
            animator.SetTrigger("Attack");
            return eNodeState.success;
        }

        return eNodeState.failure;
    }

    // 공격 애니메이션 확인 (행동 트리용)
    public eNodeState CheckAttacking()
    {
        if (IsAnimationRunning("Attack"))
        {
            rb.velocity = Vector3.zero;
            return eNodeState.running;
        }
        return eNodeState.success;
    }

    // 타겟 추적 이동 (행동 트리용)
    public eNodeState DoMoveToTarget()
    {
        if (nearbyTarget == null) return eNodeState.failure;
        if ((nearbyTarget.transform.position - transform.position).sqrMagnitude < attackRange)
        {
            rb.velocity = Vector3.zero;
            return eNodeState.success;
        }
        var dir = (nearbyTarget.transform.position - transform.position).normalized;
        OnMove(dir);
        return eNodeState.running;
    }

    // 특정 애니메이션이 재생 중인지 확인
    bool IsAnimationRunning(string stateName)
    {
        if (animator != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                return normalizedTime != 0 && normalizedTime < 1f;
            }
        }
        return false;
    }

    // 원위치로 복귀 (행동 트리용)
    public eNodeState DoMoveOrigin()
    {
        if((originPos - transform.position).sqrMagnitude < 0.5f)
        {
            rb.velocity = Vector3.zero;
            return eNodeState.success;
        }
        var dir = (originPos - transform.position).normalized;
        OnMove(dir);
        return eNodeState.running;
    }

    // 사망 상태 확인 (행동 트리용)
    public eNodeState CheckDead()
    {
        return Hp <= 0 ? eNodeState.success : eNodeState.failure;
    }

    // 이동 처리
    public void OnMove(Vector2 dir)
    {
        spriteRenderer.flipX = dir.x < 0;
        rb.velocity = dir * speed;
    }

    // 상태 효과 업데이트
    private void UpdateStatusEffects()
    {
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeStatusEffects[i];
            effect.Update(this);
            if (effect.IsFinished())
            {
                RemoveStatusEffect(effect);
            }
        }
    }

    // 매 프레임 업데이트
    private void Update()
    {
        UpdateStatusEffects();
    }

    // 기즈모 그리기 (디버그용)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }
}
