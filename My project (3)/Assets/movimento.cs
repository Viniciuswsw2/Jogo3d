using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimento : MonoBehaviour
{
    private CharacterController Controller;
    public float speed;
    public float gravidy;
    private Transform cam;
    public float smoothRotTime;
    private float turnSmoothVelocit;
    

    private Animator anim;

    private Vector3 movedirection;

// Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
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
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float smoothAngle =
                    Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocit, smoothRotTime);

                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                movedirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;

                anim.SetInteger("Transition", 1);
            }
            else
            {
                movedirection = Vector3.zero;
                anim.SetInteger("Transition", 0);
            }
        }

        movedirection.y -= gravidy * Time.deltaTime;

        Controller.Move(movedirection * Time.deltaTime);
    }
   

    void GetMouseInput()
    {
        if (Controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetInteger("Transition", 3);
            }
        }
    }
}

