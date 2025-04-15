// using Ironcow.BT;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UniRx;
// using UniRx.Triggers;
// using Unity.VisualScripting;
// using UnityEngine;

// /// <summary>
// /// 행동 트리 시스템을 사용하여 적의 행동, 이동, 전투를 처리하는 몬스터 클래스
// /// 오브젝트 풀링 기능을 위해 PoolBase를 상속받음
// /// </summary>
// public class Monster : PoolBase
// {
//     [SerializeField] private int MaxHp = 1000;
//     public int Hp { get => enemyInfo.hp; set => enemyInfo.hp = value; }
//     private EnemyInfo enemyInfo;
//     public string rcode => enemyInfo.Data.rcode;

//     // 전투 및 감지 범위
//     public float attackRange = 5;    // 몬스터가 공격할 수 있는 범위
//     public float detectRange = 10;   // 몬스터가 적을 감지할 수 있는 범위

//     // 컴포넌트 참조
//     [SerializeField] private SpriteRenderer spriteRenderer;
//     [SerializeField] private Animator animator;
//     [SerializeField] private Rigidbody2D rb;
//     [SerializeField] private float speed;
//     private Vector3 originPos;       // 몬스터의 원래 생성 위치

//     [SerializeField] BTRunner bt;    // AI 로직을 위한 행동 트리 실행기
//     Collider2D[] colliders = new Collider2D[10];  // 물리 오버랩 체크를 위한 버퍼
//     Collider2D nearbyTarget;         // 현재 타겟팅된 적

//     /// <summary>
//     /// 몬스터를 적 데이터로 초기화하고 행동 트리를 설정
//     /// </summary>
//     /// <param name="data">적 설정 데이터</param>
//     public void Init(EnemyData data)
//     {
//         enemyInfo = new EnemyInfo(data);
//         bt = new BTRunner(name.Replace("(Clone)", "")).SetActions(this);
//         Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.1f))
//             .Subscribe(_ => bt.Operate()).AddTo(gameObject);
//     }

//     /// <summary>
//     /// 몬스터의 위치를 설정하고 원래 위치로 저장
//     /// </summary>
//     /// <param name="pos">설정할 위치</param>
//     public void SetPosition(Vector3 pos)
//     {
//         originPos = transform.position = pos;
//     }

//     /// <summary>
//     /// 몬스터가 받는 데미지 처리
//     /// </summary>
//     /// <param name="damage">받는 데미지 양</param>
//     public void OnDamage(int damage = 1)
//     {
//         Hp -= damage;
//         if (Hp <= 0)
//         {
//             OnDead();
//         }
//     }

//     /// <summary>
//     /// 몬스터의 사망 처리 및 정리
//     /// </summary>
//     public void OnDead()
//     {
//         GameManager.Instance.ReleaseMonster(this);
//         PoolManager.Instance.Release(this);
//     }

//     /// <summary>
//     /// 행동 트리 노드: 사망 시퀀스 처리
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState DoDead()
//     {
//         GameManager.Instance.ReleaseMonster(this);
//         PoolManager.Instance.Release(this);
//         return eNodeState.success;
//     }

//     /// <summary>
//     /// 행동 트리 노드: 감지 범위 내의 근처 적을 탐색
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState FindEnemy()
//     {
//         if(Physics2D.OverlapCircleNonAlloc(transform.position, detectRange, colliders, 1 << LayerMask.NameToLayer("Character")) > 0)
//         {
//             var list = colliders.ToList();
//             list.RemoveAll(obj => obj == null);
//             nearbyTarget = list.OrderBy(col => (transform.position - col.transform.position).sqrMagnitude).First();
//             return eNodeState.success;
//         }
//         else
//         {
//             nearbyTarget = null;
//         }
//         return eNodeState.failure;
//     }

//     /// <summary>
//     /// 행동 트리 노드: 현재 타겟이 공격 범위 내에 있는지 확인
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState CheckEnemyWithAttackRange()
//     {
//         if (nearbyTarget == null) return eNodeState.failure;
//         var dist = (transform.position - nearbyTarget.transform.position).sqrMagnitude;
//         if (dist < attackRange)
//         {
//             rb.velocity = Vector3.zero;
//             return eNodeState.success;
//         }
//         else
//         {
//             return eNodeState.failure;
//         }
//     }

//     /// <summary>
//     /// 행동 트리 노드: 현재 타겟에 대한 공격 실행
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState DoAttackEnemy()
//     {
//         rb.velocity = Vector3.zero;
//         if (nearbyTarget != null)
//         {
//             spriteRenderer.flipX = (nearbyTarget.transform.position - transform.position).normalized.x < 0;
//             animator.SetTrigger("Attack");
//             return eNodeState.success;
//         }

//         return eNodeState.failure;
//     }

//     /// <summary>
//     /// 행동 트리 노드: 공격 애니메이션이 현재 재생 중인지 확인
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState CheckAttacking()
//     {
//         if (IsAnimationRunning("Attack"))
//         {
//             rb.velocity = Vector3.zero;
//             return eNodeState.running;
//         }
//         return eNodeState.success;
//     }

//     /// <summary>
//     /// 행동 트리 노드: 현재 타겟을 향해 이동
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState DoMoveToTarget()
//     {
//         if (nearbyTarget == null) return eNodeState.failure;
//         if ((nearbyTarget.transform.position - transform.position).sqrMagnitude < attackRange)
//         {
//             rb.velocity = Vector3.zero;
//             return eNodeState.success;
//         }
//         var dir = (nearbyTarget.transform.position - transform.position).normalized;
//         OnMove(dir);
//         return eNodeState.running;
//     }

//     /// <summary>
//     /// 특정 애니메이션이 현재 재생 중인지 확인
//     /// </summary>
//     /// <param name="stateName">확인할 애니메이션 상태 이름</param>
//     /// <returns>애니메이션이 재생 중이면 true, 아니면 false</returns>
//     bool IsAnimationRunning(string stateName)
//     {
//         if (animator != null)
//         {
//             if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
//             {
//                 var normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

//                 return normalizedTime != 0 && normalizedTime < 1f;
//             }
//         }

//         return false;
//     }

//     /// <summary>
//     /// 행동 트리 노드: 몬스터를 원래 위치로 복귀
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState DoMoveOrigin()
//     {
//         if((originPos - transform.position).sqrMagnitude < 0.5f)
//         {
//             rb.velocity = Vector3.zero;
//             return eNodeState.success;
//         }
//         var dir = (originPos - transform.position).normalized;
//         OnMove(dir);
//         return eNodeState.running;
//     }

//     /// <summary>
//     /// 행동 트리 노드: 몬스터가 사망했는지 확인
//     /// </summary>
//     /// <returns>노드 실행 상태</returns>
//     public eNodeState CheckDead()
//     {
//         if(Hp <= 0)
//         {
//             return eNodeState.success;
//         }
//         return eNodeState.failure;
//     }

//     /// <summary>
//     /// 주어진 방향으로 몬스터 이동 처리
//     /// </summary>
//     /// <param name="dir">이동할 방향</param>
//     public void OnMove(Vector2 dir)
//     {
//         spriteRenderer.flipX = dir.x < 0;
//         rb.velocity = dir * speed;
//     }

//     /// <summary>
//     /// 에디터에서 공격 및 감지 범위를 시각적으로 표시
//     /// </summary>
//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.green;
//         Gizmos.DrawWireSphere(transform.position, detectRange);

//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);
//     }
// }
