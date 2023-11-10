
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Inimigo : MonoBehaviour
{
    [Header("Atributos")]
    public float life = 100f;
    public int atack;
    public float speed;
    public float lockradius;
    public float coliderradius = 2f;    
    
    [Header("componentes")] 
    public Animator anim;
    public CapsuleCollider capsule;
    public BoxCollider box;
    public NavMeshAgent agent;
    public AudioSource source;
    public AudioClip[] audios;

    [Header("outros")] 
    public Transform player;
    public bool atacking;
    public bool walking;
    public bool waitfor;
    public bool hitting;
    public bool playerdead;

    [Header("WayPoints")]
    public List<Transform> points = new List<Transform>();
    public int currentpatch;
    public float patchdistance;
    void Start()
    {
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if (life > 0)
        {

            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= lockradius)
            {
                agent.isStopped = false;

                if (!atacking)
                {
                    agent.SetDestination(player.position);
                    anim.SetBool("Walk Forward", true);
                    walking = true;
                    play(1);
                }

                if (distance <= agent.stoppingDistance)
                {
                    StartCoroutine("Matack");
                }
                else
                {
                    atacking = false;
                }
            }
            else
            {
                anim.SetBool("Walk Forward", false);
                atacking = false;
                walking = false;
                movetowaypoint();
            }
        }
    }
    
    IEnumerator Matack()
    {
        if(!waitfor && !hitting && !playerdead)
        {
            waitfor = true;
            atacking = true;
            walking = false;
            play(2);
            anim.SetBool("Walk Forward", false);
            anim.SetBool("Bite Attack", true);
            yield return new WaitForSeconds(1.2f);
            GetPlayer();
            waitfor = false;
        }

        if (playerdead)
        {
            anim.SetBool("Walk Forward", false);
            anim.SetBool("Bite Attack", false);
            walking = false;
            atacking = false;
            agent.isStopped = true;
        }
    }

    void GetPlayer()
    {
        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * coliderradius),coliderradius))
        {
            if (c.gameObject.CompareTag("Player"))
            {
                c.gameObject.GetComponent<Player>().getHit(atack);
                playerdead = c.gameObject.GetComponent<Player>().isdead;
            }
        }
    }

    public void getHit(int dmg)
    {
        life -= dmg;
        if (life > 0)
        {
            StopCoroutine("Matack");
            anim.SetTrigger("Take Damage");
            hitting = true;
            StartCoroutine("recovery");
        }
        else
        {
            play(0);
            anim.SetTrigger("Die");
            Destroy(gameObject, 3f);
        }
    }

    IEnumerator recovery()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Walk Forward", false);
        anim.SetBool("Bite Attack", false);
        hitting = false;
        waitfor = false;
    }

    private void movetowaypoint()
    {
        if (points.Count > 0)
        {
            float distance = Vector3.Distance(points[currentpatch].position, transform.position);
            agent.destination = points[currentpatch].position;
                
            if (distance <= patchdistance)
            {
                currentpatch = Random.Range(0, points.Count);
                
            }
            anim.SetBool("Walk Forward", true);
            walking = true;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, lockradius);
    }

    void play(int valor)
    {
        if (valor >= 0 && valor < audios.Length)
        {
            source.clip = audios[valor];
            source.Play();
        }
    }
}
