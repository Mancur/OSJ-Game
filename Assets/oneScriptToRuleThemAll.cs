using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class oneScriptToRuleThemAll : MonoBehaviour
{
    [Header("Player")]
    public oneScriptToRuleThemAll player;
    public float movementSpeed = 10f;
    public float jumpForce = 1f;
    private Rigidbody2D rb;
    private float movementX = 0;
    public bool isGrounded = false;
    //unlocked controls:
    [SerializeField] private bool right_unlocked = true;
    [SerializeField] private bool left_unlocked = false;
    [SerializeField] private bool jump_unlocked = false;

    [Header("Keys")]
    public string keyType;

    [Header("Camera")]
    public Transform cam;
    public Transform playerToFollow;
    public float camereVerticalOffset = 3f;

    [Header("Menu")]
    public GameObject pauseMenu;
    private bool isPaused;

    private void Start()
    {
        if (gameObject.CompareTag("Player"))
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
            movementX = 0;
            if (Input.GetKey(KeyCode.D) && right_unlocked)
            {
                movementX = 1;
            }
            if (Input.GetKey(KeyCode.A) && left_unlocked)
            {
                movementX = -1;
            }
            if (Input.GetKey(KeyCode.W) && jump_unlocked && isGrounded)
            {
                jump();
            }

            Move();
        }
        if (gameObject.CompareTag("MainCamera"))
        {
            transform.position = new Vector3(playerToFollow.position.x, playerToFollow.position.y + camereVerticalOffset, -10);
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.CompareTag("Player"))
        {

        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(movementX * Time.fixedDeltaTime * movementSpeed, rb.velocity.y);
    }

    private void jump()
    {
        Vector2 jumpSpeed = new Vector2(0f, jumpForce);
        rb.AddForce(jumpSpeed, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (collision.CompareTag("Key"))
            {
                addKey();
                Destroy(collision.gameObject);
            }
        }
        if (gameObject.CompareTag("groundChecker"))
        {
            if (collision.CompareTag("Platform"))
            {
                player.isGrounded = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.CompareTag("groundChecker"))
        {
            if (collision.CompareTag("Platform"))
            {
                player.isGrounded = false;
            }
        }
    }

    private void addKey()
    {

    }

    //Menu functions

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}