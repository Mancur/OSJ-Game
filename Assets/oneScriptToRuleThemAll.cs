using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class oneScriptToRuleThemAll : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [Header("Player")]
    public oneScriptToRuleThemAll player;
    public float movementSpeed = 10f;
    public float jumpForce = 1f;
    private Rigidbody2D rb;
    public Animator animator;
    private float movementX = 0;
    public bool isGrounded = false;
    private bool lookingRight = true;
    //unlocked controls:
    [SerializeField] private oneScriptToRuleThemAll leftKeyhole;
    [SerializeField] private oneScriptToRuleThemAll rightKeyhole;
    [SerializeField] private oneScriptToRuleThemAll jumpKeyhole;
    [SerializeField] private bool right_unlocked = true;
    [SerializeField] private bool left_unlocked = false;
    [SerializeField] private bool jump_unlocked = false;

    [Header("Keys")]
    public string keyType;
    public GameObject keyParent;
    public TextMeshProUGUI keyText;
    public GameObject keyUI;
    public RectTransform rectTransform;
    public Canvas canvas;
    public CanvasGroup canvasGroup;

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
            animator = GetComponent<Animator>();
        }
        if (gameObject.CompareTag("Key"))
        {
            keyText = GetComponentInChildren<TextMeshProUGUI>();
            keyText.text = keyType;
        }
        if (gameObject.CompareTag("KeyUI"))
        {
            keyText = GetComponentInChildren<TextMeshProUGUI>();
            keyText.text = keyType;
            rectTransform = GetComponent<RectTransform>();
            canvas = GameObject.FindGameObjectWithTag("PauseMenuCanvas").GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        if (gameObject.CompareTag("Player"))
        {
            CheckUnlockedFunctions();
            AnimatorDependencies();
            GetPlayerInput();

            Move();
        }
        if (gameObject.CompareTag("MainCamera"))
        {
            transform.position = new Vector3(playerToFollow.position.x, playerToFollow.position.y + camereVerticalOffset, -10);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (collision.CompareTag("Key"))
            {
                addKey(collision.GetComponent<oneScriptToRuleThemAll>());
                Destroy(collision.gameObject);
            }
            if (collision.CompareTag("Spike"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    #region Player Functions
    private void GetPlayerInput()
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
        if (right_unlocked)
        {
            if (Input.GetKey(rightKeyhole.keyUI.GetComponent<oneScriptToRuleThemAll>().keyType))
            {
                movementX = 1;
            }
        }
        if (left_unlocked)
        {
            if (Input.GetKey(leftKeyhole.keyUI.GetComponent<oneScriptToRuleThemAll>().keyType))
            {
                movementX = -1;
            }
        }
        if (jump_unlocked && isGrounded)
        {
            if (Input.GetKeyDown(jumpKeyhole.keyUI.GetComponent<oneScriptToRuleThemAll>().keyType))
            {
                jump();
            }
        }
    }

    private void CheckUnlockedFunctions()
    {
        if (leftKeyhole.keyUI != null)
        {
            left_unlocked = true;
        }
        else
        {
            left_unlocked = false;
        }
        if (rightKeyhole.keyUI != null)
        {
            right_unlocked = true;
        }
        else
        {
            right_unlocked = false;
        }
        if (jumpKeyhole.keyUI != null)
        {
            jump_unlocked = true;
        }
        else
        {
            jump_unlocked = false;
        }
    }


    private void Move()
    {
        movementX = movementX * Time.fixedDeltaTime * movementSpeed;
        FlipSprite();
        rb.velocity = new Vector2(movementX, rb.velocity.y);
    }

    private void AnimatorDependencies()
    {
        animator.SetFloat("YVelocity", rb.velocity.y);
        if (rb.velocity.y <= 0)
        {
            animator.SetBool("Jump", false);
        }
        animator.SetBool("Grounded", isGrounded);
    }

    private void FlipSprite()
    {
        animator.SetFloat("Speed", Mathf.Abs(movementX));
        if (movementX < 0 && lookingRight)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            lookingRight = false;
        }
        else if (movementX > 0 && !lookingRight)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            lookingRight = true;
        }
    }

    private void jump()
    {
        Vector2 jumpSpeed = new Vector2(0f, jumpForce);
        animator.SetBool("Jump", true);
        rb.AddForce(jumpSpeed, ForceMode2D.Force);
    }
    #endregion

    #region Menu functions
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
        SceneManager.LoadScene(SceneManager.sceneCount - 1);
    }

    public void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Key drag and drop functions
    private void addKey(oneScriptToRuleThemAll script)
    {
        GameObject go = Instantiate(keyUI);
        go.transform.SetParent(keyParent.transform);
        go.GetComponent<oneScriptToRuleThemAll>().keyType = script.keyType;
        go.GetComponent<RectTransform>().anchoredPosition = keyParent.GetComponent<RectTransform>().anchoredPosition;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (gameObject.CompareTag("KeyUI"))
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (gameObject.CompareTag("KeyHole"))
        {
            if(eventData.pointerDrag != null)
            {
                if (eventData.pointerDrag.transform.CompareTag("KeyUI"))
                {
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                    keyUI = eventData.pointerDrag;
                    eventData.pointerDrag.GetComponent<oneScriptToRuleThemAll>().keyParent = gameObject;
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameObject.CompareTag("KeyUI"))
        {
            if (keyParent != null)
            {
                keyParent.GetComponent<oneScriptToRuleThemAll>().keyUI = null;
                keyParent = null;
            }
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (gameObject.CompareTag("KeyUI"))
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
    #endregion
}