using UnityEngine;
using System.Collections;
// this script is for rotating turret 
public class EnemyBehavior : MonoBehaviour
{
    Animator anim;
    public float attack;
    public float targetDistance;
    public float AmmoSpeed;
    public float damping;
    public Transform target;
    Rigidbody theRigidBody;
    Renderer myRender;
    public float Range;
    float v;
    float f;
    public float fireCoolDown;
    bool canFire;
    public Transform bullet;
    GameObject player;
    float life;
    float blink;
    bool isDying;
    public float lifeSpan;
    float range;
    

    void Start()
    {
        //initializes animator as well as the renderer for animations
        myRender=GetComponent<Renderer>();
        anim = GetComponent<Animator>();
        canFire = true;
        player = GameObject.Find("RPlayer");
        target = player.transform;
        life = 0;
       
        isDying = false;
        blink = 0;

    }
    // update method calculates the target distance, and then rotates the turret 2 degress in front of the turret. Only works if the object is going Right.
    void Update()
    {
        targetDistance = Vector3.Distance(target.transform.position, transform.position);
        Vector3 leadVector = new Vector3(0, 30, 0);
        Quaternion rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = rotation;
        transform.Rotate(leadVector);
        attackPlayer();
        /*life = life + Time.deltaTime;
        if (isDying == true)
        {
            blink = blink + Time.deltaTime;
        }
        if (life > 5)
        {
            isDying = true;

            if (blink>.5)
            {
                myRender.material.color = Color.red;

            }
            if (blink<.5)
            {
                myRender.material.color = Color.white;
            }
            if(life > lifeSpan)
            {
                Destroy(gameObject);
            }
            if(blink>1)
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
           

            if (fireCoolDown > .5)
            {
                canFire = true;

            }
            if (canFire)
            {
                    
                Instantiate(bullet, transform.position, transform.rotation);
                fireCoolDown = 0;

                
                canFire = false;
            }
        }
    }
}
