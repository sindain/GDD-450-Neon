using UnityEngine;
using System.Collections;

public class BomberEnemyBehavior : MonoBehaviour {
    public Transform bomb;
    bool attack;
    float randomNumber;
    float time;
    // Use this for initialization
    void Start ()
    {


    }
	
	// Update is called once per frame
	void Update ()
    {
    
        time = time +Time.deltaTime;
        if (time > 2)
        {
            randomNumber = Random.Range(0, 100);
            bombDrop(randomNumber);
            time = 0;
        }
        
        transform.Translate(Vector3.forward * Time.deltaTime);
    }
    public void bombDrop(float n)
    {
        if (n  > 25)
        { 
            Instantiate(bomb, transform.position, transform.rotation);
        }
    }
}
