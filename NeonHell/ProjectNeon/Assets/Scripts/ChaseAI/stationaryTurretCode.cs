using UnityEngine;
using System.Collections;
// this script is for rotating turret 
public class stationaryTurretCode : MonoBehaviour
{
    Animator anim;
    public float attack;
    public float Lookdistance;
    public float targetDistance;
    public float AmmoSpeed;
    public Transform target;
    float turretAngle;
    Renderer myRender;
    public float Range;
    float v;
    float f;
    public float fireCoolDown;
    bool canFire;
    public Transform EnemyBullet;
    GameObject player;
    float life;
    float blink;
    bool isDying;
    void Start()
    {
        //initializes animator as well as the renderer for animations
        myRender = GetComponent<Renderer>();
        anim = GetComponent<Animator>();
        canFire = true;
       turretAngle= Random.Range(0.0f,100.0f);
        setRotation();
        player = GameObject.Find("RPlayer");
        target = player.transform;
        life = 0.0f;
        isDying = false;
        blink = 0;
     


    }
    void Update()
    {
		targetDistance = Vector3.Distance(target.position, transform.position);
        attackPlayer();
        /*life = life + Time.deltaTime;
        if (isDying == true)
        {
            blink = blink + Time.deltaTime;
        }
        if (life > 5)
        {
            isDying = true;

            if (blink > .5)
            {
                myRender.material.color = Color.red;

            }
            if (blink < .5)
            {
                myRender.material.color = Color.white;
            }
            if (life > 10)
            {
                Destroy(gameObject);
            }
            if (blink > 1)
            {
                blink = 0;
            }
        }*/



    }
    //instantiates a bullet if the player is within range, and uses a cooldown to decide how many bullets.
    public void attackPlayer()
    {

        fireCoolDown = fireCoolDown + Time.deltaTime;

        if (targetDistance < Range)
        {


            if (fireCoolDown > 1)
            {
                canFire = true;

            }
            if (canFire)
            {
                Instantiate(EnemyBullet, transform.position, transform.rotation);

                fireCoolDown = 0;


                canFire = false;
            }
        }
    }
    void setRotation()
    {
        if (turretAngle>25)
        {
            Vector3 leadVector = new Vector3(0, 40, 0);
            transform.Rotate(leadVector);
        }
        else if(turretAngle>50)
        {
            Vector3 leadVector = new Vector3(0, 60, 0);
            transform.Rotate(leadVector);
        }
        else if (turretAngle > 75)
        {
            Vector3 leadVector = new Vector3(0, -40, 0);
            transform.Rotate(leadVector);
        }
        else if (turretAngle > 50)
        {
            Vector3 leadVector = new Vector3(0, -60, 0);
            transform.Rotate(leadVector);
        }
        else
        {
            
        }
    }
}
