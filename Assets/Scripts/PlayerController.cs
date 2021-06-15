using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D Rb;
    private Animator anim;
    private enum State { idle,running,jumping,falling,hurt}
    private State state = State.idle;
    private Collider2D coll;
    

    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherrytext;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private int health;
    [SerializeField] private Text healthamount;
    [SerializeField] private AudioSource cherry;
    [SerializeField] private AudioSource footstep; 
    //public int cherries = 0;


    // Start is called before the first frame update
    private void Start() 
    {
        Rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        healthamount.text = health.ToString();
        //footstep = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    private void Update()
    {
        if (state != State.hurt)
        {
            Movement();
        }
        
        //Movement();
        AnimationState();
        anim.SetInteger("state", (int)state);
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        if (hDirection < 0)
        {
            Rb.velocity = new Vector2(-speed, Rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
            //anim.SetBool("running", true);
        }
        else if (hDirection > 0)
        {
            Rb.velocity = new Vector2(speed, Rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
            //anim.SetBool("running", true);
        }


        if (Input.GetButton("Jump") && coll.IsTouchingLayers(ground))
        {
            Rb.velocity = new Vector2(Rb.velocity.x, jumpForce);
            state = State.jumping;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            cherry.Play();
            Destroy(collision.gameObject);
            cherries += 1;
            cherrytext.text = cherries.ToString();
        
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (state == State.falling)
            {
                enemy.JumpedOn();
                
            }
            else
            {
                state = State.hurt;
                HandleHealth();

                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    Rb.velocity = new Vector2(-hurtForce, Rb.velocity.y);
                }
                else
                {
                    Rb.velocity = new Vector2(hurtForce, Rb.velocity.y);
                }
            }
        }
        
    }

    public void HandleHealth()
    {
        health -= 1;
        healthamount.text = health.ToString();

        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }

    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if (Rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }

        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(Rb.velocity.x) < .1f)
            {
                state = State.idle;
            
            }
        
        }



        else if (Mathf.Abs(Rb.velocity.x) > 2f)
        {
            state = State.running;

        }
        else
        {
            state = State.idle;

        }
      
    }

    private void FootStep()
    {
        footstep.Play();
    
    
    }
}
