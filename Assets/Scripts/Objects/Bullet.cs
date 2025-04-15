using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Bullet : PoolBase
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 5;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance.monsters.ContainsKey(collision.gameObject.GetInstanceID()))
        {
            var monster = GameManager.Instance.monsters[collision.gameObject.GetInstanceID()];
            monster.OnDamage();
            PoolManager.Instance.Release(this);
        }
    }

    public void SetMove(Vector2 dir)
    {
        rb.velocity = dir * speed;
    }

    private void Update()
    {
        var distance = (GameManager.Instance.userCharacter.transform.position - transform.position).sqrMagnitude;
        if(distance > 200)
        {
            PoolManager.Instance.Release(this);
            //Destroy(gameObject);
        }
    }
}
