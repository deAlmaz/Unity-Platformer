using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;

    float timer;
    int direction = 1;

    Animator animator;

    bool broken = true;

    public ParticleSystem smokeEffect;
    public ParticleSystem robotFixEffect;

    public GameObject fixedClipbox;
    public GameObject hitClipbox;
    AudioSource audioSourceFix;
    AudioSource audioSourceHit;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        audioSource= GetComponent<AudioSource>();
        audioSourceFix= GetComponent<AudioSource>();
        audioSourceFix= fixedClipbox.GetComponent<AudioSource>();
        audioSourceHit= GetComponent<AudioSource>();
        audioSourceHit= hitClipbox.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        
        rigidbody2D.MovePosition(position);

        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
    }

    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();
        robotFixEffect.Play();

        StartCoroutine(waiter());
        IEnumerator waiter()
        {
            audioSourceHit.Play();

            yield return new WaitForSeconds(0.5f);

            audioSourceFix.Play();

            audioSource.Stop();

            yield break;
        }
        
        
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
    RubyController player = other.gameObject.GetComponent<RubyController>();

    if (player != null)
        {
            player.ChangeHealth(-1);
        }
}
}
