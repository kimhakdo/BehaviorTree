using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UniRx.Triggers;
using UniRx;
using UnityEngine;
using Unity.VisualScripting;
using System;
using Cysharp.Threading.Tasks;

class Pool : Singleton<Pool>
{
    Queue<AudioSource> sources = new Queue<AudioSource>();

    public AudioSource Spawn()
    {
        return sources.Dequeue();
    }

    public void Release(AudioSource source)
    {
        sources.Enqueue(source);
    }
}
public class Character : MonoBehaviour
{
    public CharacterInfo info;
    public List<ItemInfo> inventory => info.inventory;
    public Rigidbody2D rb;
    public Vector2 dir;
    public float speed = 3;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public async void Init()
    {
        info = new CharacterInfo(DataManager.Instance.GetData<CharacterData>("CharacterData"));
        InputManager.Instance.Subscribe(KeyCode.A, () =>
        {
            spriteRenderer.flipX = false;
            dir.x = -1;
        }, () =>
        {
            if(dir.x == -1)
            {
                dir.x = 0;
            }
        });
        InputManager.Instance.Subscribe(KeyCode.D, () =>
        {
            spriteRenderer.flipX = true;
            dir.x = 1;
        }, () =>
        {
            if (dir.x == 1)
            {
                dir.x = 0;
            }
        });
        InputManager.Instance.Subscribe(KeyCode.S, () =>
        {
            dir.y = -1;
        }, () =>
        {
            if (dir.y == -1)
            {
                dir.y = 0;
            }
        });
        InputManager.Instance.Subscribe(KeyCode.W, () =>
        {
            dir.y = 1;
        }, () =>
        {
            if (dir.y == 1)
            {
                dir.y = 0;
            }
        });
        this.UpdateAsObservable().Subscribe(_ => OnMove());
        Camera.main.AddComponent<TrackingCamera>().target = transform;
        //InputManager.Instance.Subscribe(KeyCode.Space, Shoot);

    }


    public void OnMove()
    {
        rb.velocity = dir.normalized * speed;
    }

    private void OnDestroy()
    {
        if(InputManager.IsInstance)
            InputManager.Instance.Unsubscribe(KeyCode.Space, Shoot);
    }

    public void Shoot()
    {
        var mousePos = Input.mousePosition;
        var mousePosToWorld = Camera.main.ScreenToWorldPoint(mousePos);
        var normal = (mousePosToWorld - transform.position).normalized;
        for (int i = 0; i < (GameManager.Instance.isPowerShot ? 500 : 1); i++)
        {
            var bullet = PoolManager.Instance.Spawn<Bullet>();// ResourceManager.Instance.InstantiateAsset("Bullet").GetComponent<Bullet>();
            bullet.transform.position = transform.position + normal;
            bullet.SetMove(normal);
        }
    }
}
