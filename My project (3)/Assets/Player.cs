using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController Controller;
    public float speed;
    public float gravity;
    private Transform cam;
    public float smoothRotTime;
    private float turnSmoothVelocit;
    private bool iswalking;
    private Animator anim;
    public List<Transform> enemyList = new List<Transform>();
    private Vector3 movedirection;
    public float ColliderRadius;
    public int life = 40;
    public int dano = 15;
    public bool isAtack;
    public bool isdead;
    public bool ishiting;
    public Slider healthBar;
    public float coliderradius;
    [Header("List")]
    public List<Transform> enemylist = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        healthBar.value = life;
        healthBar.maxValue = life; 
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GetMouseInput();
    }

    void Move()
    {
        if (Controller.isGrounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            if (direction.magnitude > 0)
            {
                if (!anim.GetBool("Atacking"))
                {
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocit, smoothRotTime);

                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                    float currentSpeed = speed; // Velocidade normal

                    // Verifica se a tecla Shift está sendo pressionada para aumentar a velocidade
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        currentSpeed *= 1.6f; // Aumento de velocidade quando Shift é pressionado
                    }

                    movedirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * currentSpeed;

                    anim.SetInteger("Transition", 1);
                    iswalking = true;
                }
                else
                {
                    anim.SetBool("Walk", false);
                    movedirection = Vector3.zero;
                }
            }
            else if (iswalking)
            {
                anim.SetBool("Walk", false);
                anim.SetInteger("Transition", 0);
                movedirection = Vector3.zero;
                iswalking = false;
            }
        }

        movedirection.y -= gravity * Time.deltaTime;

        Controller.Move(movedirection * Time.deltaTime);
    }
    
    
    public void getHit(int dmg)
    {
        life -= dmg;
        if (life > 0)
        {
            StopCoroutine("Matack");
            anim.SetInteger("Transition", 3);
            ishiting = true;
            StartCoroutine("recovery");
            healthBar.value = life;
        }
        else
        {
            isdead = true;
            anim.SetTrigger("dead");
            
        }
    }

    void GetMouseInput()
    {
        if (Controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (anim.GetBool("Walk"))
                {
                    anim.SetBool("Walk", false);
                    anim.SetInteger("Transition", 0);
                }

                if (!anim.GetBool("Walk"))
                {
                    StartCoroutine("Atacking ");
                }

                StartCoroutine("Attack");
            }
        }
    }

    IEnumerator Attack()
    {
        anim.SetBool("Atacking", true);
        anim.SetInteger("Transition", 3);

        yield return new WaitForSeconds(0.5f);

        GetEnemiesList();

        foreach (Transform e in enemyList)
        {
            Debug.Log(e.name);
        }

        yield return new WaitForSeconds(1.6f);

        anim.SetInteger("Transition", 0);
        anim.SetBool("Atacking", false);
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * ColliderRadius), ColliderRadius))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemyList.Add(c.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, ColliderRadius);
    }
}
