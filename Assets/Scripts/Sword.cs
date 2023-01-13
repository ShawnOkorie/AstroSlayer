using System;
using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private CircleCollider2D _myCollider2D;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _currentDirection;
    public Player player;
    public LayerMask swordMask;
    public float knockBackPower = 2;
    public float knockBackTime = 0.5f;
    public float attackCooldown = 3;
    private float _cooldownTimer;
    private float _attackDurationTimer = 0.25f;
    private float _attackDuration = 0.25f;
    public int damage = 3;

    void Awake()
    {
        _myCollider2D = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _cooldownTimer = attackCooldown;
    }

    void Update()
    {
        if (player.moveInput != Vector3.zero)
        {
            _currentDirection = player.moveInput;
        }

        transform.position = player.transform.position + _currentDirection * 0.7f;

        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
            _spriteRenderer.enabled = false;
            _myCollider2D.enabled = false;
            
        }
        else if (_attackDurationTimer > 0)
        {
            _attackDurationTimer -= Time.deltaTime;
            _spriteRenderer.enabled = true;
            _myCollider2D.enabled = true;
           
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _myCollider2D.radius, swordMask);

            foreach (var collider2D in hits)
            {
                if (collider2D == _myCollider2D)
                {
                    continue;
                }

                var enemy = collider2D.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.ApplyDamage(damage);
                }
            }
        }
        else
        {
            _cooldownTimer = attackCooldown;
            _attackDurationTimer = _attackDuration;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody2D enemy = other.GetComponent<Rigidbody2D>();
            if (enemy != null)
            {
                enemy.isKinematic = false;
                Vector2 difference = enemy.transform.position - transform.position;
                difference = difference.normalized * knockBackPower;
                enemy.AddForce(difference, ForceMode2D.Impulse);
                StartCoroutine(KnockbackCo(enemy));
            }
        }
    }

    private IEnumerator KnockbackCo(Rigidbody2D enemy)
    {
        if (enemy != null)
        {
            yield return new WaitForSeconds(knockBackTime);
            enemy.velocity = Vector2.zero;
            enemy.isKinematic = true;
        }
    }
}