using Cebt.Utilities;
using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cebt.LastStand
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Anywhere] InputReader input;

        [Header("MovementSettings")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float rotationSpeed = 5f;
        [SerializeField] float smoothTime = 0.2f;
        [SerializeField] float inertia = 0.1f;

        [Header("AttackSettings")]
        [SerializeField] float attackRange = 5f;
        [SerializeField] float attackCooldownTime = 2f;

        const float ZeroF = 0f;

        Vector3 movement;
        Vector2 mousePosition;

        bool isAttacking = false;

        CountdownTimer attackCooldown;
        

        private void Awake()
        {
            rb.freezeRotation = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            attackCooldown = new CountdownTimer(attackCooldownTime);
        }

        private void OnEnable()
        {
            input.Fire += OnFire;
        }

        private void OnDisable()
        {
            input.Fire -= OnFire;
        }

        private void OnFire(bool performed)
        {
            if (performed)
            {
                isAttacking = true;
                HandleAttack();
                attackCooldown.OnTimerStop += OnAttackCooldownEnd;
            }
            else
            {
                isAttacking = false;
                attackCooldown.OnTimerStop -= OnAttackCooldownEnd;
            }
        }

        private void OnAttackCooldownEnd()
        {
            if (isAttacking)
            {
                HandleAttack();
                attackCooldown.Reset();
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            input.EnablePlayerActions();
        }

        // Update is called once per frame
        void Update()
        {
            movement = new Vector3(input.Direction.x, ZeroF, input.Direction.y);
            mousePosition = new Vector2(input.MousePosition.x, input.MousePosition.y);
            attackCooldown.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleAttack()
        {
            attackCooldown.Start();
            Debug.Log("Pew");
        }

        private void HandleMovement()
        {
            if (movement.magnitude > ZeroF) 
            {
                var direction = movement.normalized;
                var targetVelocity = direction * moveSpeed;
                var velocity = rb.velocity;
                velocity += targetVelocity;
                rb.velocity = Vector3.Lerp(rb.velocity, velocity, smoothTime);
            }
            else
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, inertia);
            }
        }

        private void HandleRotation()
        {
            if(mousePosition.magnitude > ZeroF)
            {
                Vector3 posOnScreen = Camera.main.WorldToScreenPoint(transform.position);
                var mousePoint = Camera.main.ScreenToWorldPoint(mousePosition);

                float angle = (Mathf.Atan2(posOnScreen.x - mousePosition.x, posOnScreen.y - mousePosition.y) * Mathf.Rad2Deg);

                rb.rotation = Quaternion.Euler(new Vector3(ZeroF, angle, ZeroF));
            }
        }
    }
}
