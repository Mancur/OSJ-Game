using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneScriptToRuleThemAll : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private Rigidbody2D rb;
    private float movementX = 0;

    private void Start()
    {
        if (transform.CompareTag("Player"))
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (transform.CompareTag("Player"))
        {
            movementX = Input.GetAxis("Horizontal");
        }
    }

    private void FixedUpdate()
    {
        if (transform.CompareTag("Player"))
        {
            Move();
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(movementX * Time.fixedDeltaTime * movementSpeed, rb.velocity.y);
    }
}
