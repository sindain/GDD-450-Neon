using UnityEngine;
using System.Collections;

public class bombBehavior : MonoBehaviour {

    // Use this for initialization
    public float radius = 10.0F;
    public float power = 200.0F;
    public Transform bulletBomb;
    float life;
    Rigidbody rb;
    public GameObject Arena;
    Transform ground;
    float groundDistance;
   public float explosionRange;

    void Start()
    {
        
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        rb = GetComponent<Rigidbody>();
        Arena = GameObject.Find("Arena");
        ground = Arena.transform;
       
    }


    // Update is called once per frame
    void Update ()
    {
        life = life + Time.deltaTime;
        Vector3 explosionPos = transform.position;
      
        groundDistance = Vector3.Distance(ground.position, transform.position);
        if (groundDistance < explosionRange)
        {
            for (int i = 0; i < 25; i++)
            {
                Instantiate(bulletBomb, transform.position, transform.rotation);
            }
            rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
            print("boom");
            Destroy(gameObject);
        }
        if (life>1.5)
        {
            for (int i = 0; i < 25; i++)
            {
                Instantiate(bulletBomb, transform.position, transform.rotation);
            }
            rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
            print("boom");
            Destroy(gameObject);
        }
    }
        

}

