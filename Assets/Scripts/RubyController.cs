using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public GameObject projectilePrefab;
    public ParticleSystem collectibleEffect;

    float timerDisplay;

    AudioSource audioSource;
    AudioSource audioSourceAdd;
    public AudioClip throwCogClip;
    public AudioClip getHitClip;
    public GameObject walkingsStepsObj;

    // Start is called before the first frame update
    void Start()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource= GetComponent<AudioSource>();
        audioSourceAdd= walkingsStepsObj.GetComponent<AudioSource>();
        audioSourceAdd.volume = 0;
    }

    void Update()
    {
        if ((Input.GetAxis("Horizontal") != 0) || (Input.GetAxis("Vertical") != 0))
        {
            audioSourceAdd.volume = 0.1f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
                        
        Vector2 move = new Vector2(horizontal, vertical);
                
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        Vector2 position = rigidbody2d.position;
        
        //if new pos = old pos == walking sound volume off
        if (position == position + move * speed * Time.deltaTime)
        {
            audioSourceAdd.volume = 0;
        }                 

        position = position + move * speed * Time.deltaTime;
                
        rigidbody2d.MovePosition(position);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
        Launch();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }  
                }
            // if (hit.collider != null)
            // {
            // Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
            // }
}
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
            {
                if (isInvincible)
                    return;
                
                animator.SetTrigger("Hit");
                PlaySound(getHitClip);
                isInvincible = true;
                invincibleTimer = timeInvincible;
            }
        if (amount > 0)
        {
            Instantiate(collectibleEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity, rigidbody2d.transform);
            collectibleEffect.Play();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        //Debug.Log(currentHealth + "/" + maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        animator.SetTrigger("Launch");
        PlaySound(throwCogClip);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
