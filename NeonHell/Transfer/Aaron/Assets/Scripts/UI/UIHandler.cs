using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIHandler : MonoBehaviour
{
    
	/*Rect pauseMenu = new Rect (10, 10, 200, 200);
	Rect Background = new Rect (10, 10, 200, 200);
	Rect Quit = new Rect (10, 10, 200, 200);
	Rect Resume = new Rect (10, 10, 200, 200);
	Rect Menu = new Rect (10, 10, 200, 200);

	public Texture PauseImage;
	public Texture BackGroundImage;
	public Texture ResumeImage;
	public Texture QuitImage;
	public Texture MenuImage;
	*/
	//used to get other variables form other scripts
	//objectWithOtherScript.GetComponent.< script1 >().someVariable = someNumber;
	public GameObject pauseMenu;
	public GameObject particle;
	public GameObject lapPoint;
	public GameObject hud;
	public GameObject loseScreen;
	public GameObject winScreen;
	public GameObject player;
	public Text lapCount;
 	public RectTransform healthTransform;
	public int startingHealth = 8;
	public int currentHealth;
	public float maxHealth;
	public Image visualHealth;
	public int lap;
	public static bool lose = false;
	public static bool win= false;

	private float cachedY;
	private float minXValue;
	private float maxXValue;
	private float timeLimit = 10;
	private float delay = 2;
	private float deathDelay=1;
	private bool noExplode= true;

	
	bool isPause = false;
	// Use this for initialization
	
	void Start () 
	{
		lap = 0;
		startingHealth = 8;
		lose = false;
		win= false;
		currentHealth = startingHealth;
		cachedY = healthTransform.position.y;
		maxXValue = healthTransform.position.x;
		minXValue= healthTransform.position.x - healthTransform.rect.width;
	}

  
    void Update (){
        if( Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
			if(!lose)
			{
				if(isPause)
				{
	            	Time.timeScale =0;
					pauseMenu.SetActive(true);
					hud.SetActive(false);
				}
	            else
				{
	            	Time.timeScale =1;
					pauseMenu.SetActive(false);
					hud.SetActive(true);
				}
			}
        }
		if (Input.GetKeyDown (KeyCode.H))
		{
			TakeDamage (1);
		}
		if (win)
		{

			winScreen.SetActive(true);
			hud.SetActive(false);
			delay -= Time.deltaTime;
			if ( delay < 0 )
			{
				Application.LoadLevel("Credits");
			}
		}
		else if(lose && noExplode)
		{
			Instantiate(particle,player.transform.position,player.transform.rotation);
			Destroy(player.gameObject);
			noExplode=false;
		}
		else if(lose)
		{
			deathDelay -= Time.deltaTime;
			Rotate.isRotate=false;

			if(deathDelay<=0)
			{
				loseScreen.SetActive(true);
				Time.timeScale =0;
				hud.SetActive(false);
			}
		}
		//timeLimit -= Time.deltaTime;
		if ( lap == 2 )
		{
			win = true;
		}
		if (currentHealth <= 0) 
		{
			lose = true;
		}
    }
	//handles the health grapic
	private void HandleHealth()
	{
		float currentXvalue = mapValues (currentHealth, 0, maxHealth, minXValue, maxXValue);
		healthTransform.position = new Vector3 (currentXvalue, cachedY);
		if (currentHealth > maxHealth/2) 
		{
			visualHealth.color = new Color32((byte)mapValues(currentHealth,maxHealth/2,maxHealth,255,0),255,0,255);
		}
		else
		{
			visualHealth.color = new Color32(255,(byte)mapValues(currentHealth,0,maxHealth/2,0,255),0,255);
		} 
	}
	public void TakeDamage (int amount)
	{
		currentHealth -= amount;
		HandleHealth();
	}
	public void HandleLap()
	{
		lapCount.text = "Lap " + lap + "/2";
	}
    void OnGUI()
    {
        if (isPause) {
			/*GUI.DrawTexture(pauseMenu, PauseImage);
			GUI.DrawTexture(Background, BackGroundImage);
			GUI.Button(Quit, QuitImage);
			GUI.Button(Resume, ResumeImage);
			GUI.Button(Menu, MenuImage);*/
		}	         
    }
	public void Transition(){
		isPause = false;
		Time.timeScale =1;
		pauseMenu.SetActive(false);
		hud.SetActive(true);
	}
	private float mapValues(float x, float inmin, float inmax, float outmin,float outmax)
	{
		return(x - inmin) * (outmax - outmin) / (inmax - inmin) + outmin;
	}
}
