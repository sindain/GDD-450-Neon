using UnityEngine;
using System.Collections;
// works for any turret
public class bulletScript : MonoBehaviour {
    public int speed;
    public float life;
	public UIHandler UIH;
	public GameObject canvas;
    Rigidbody rb;
    public Transform target;
    GameObject arena;
    float targetDistance;


	


	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();

        life = 0.0f;

		canvas = GameObject.FindGameObjectWithTag ("UIH");
		UIH = canvas.GetComponent<UIHandler> ();
       

    }
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "RPlayer") 
		{
			UIH.TakeDamage (1);
			Destroy(gameObject);
		}
	}
	// Update is called once per frame
	void Update ()
    {
       
        life = life + Time.deltaTime;
        if (gameObject.name != "bombBullet")
        {
            rb.AddForce(transform.forward * speed);
        }
        if (gameObject.name == "bombBullet")
        {
            if (life > 5)
            {
                Destroy(gameObject);
                print("boom");
            }

        }

        else
        {
            if (life > 2)
            {
                Destroy(gameObject);
                print("boom");
            }
        }

    }

}
