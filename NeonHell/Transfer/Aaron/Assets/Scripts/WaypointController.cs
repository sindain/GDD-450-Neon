using UnityEngine;
using System.Collections;

public class WaypointController : MonoBehaviour {

    public bool         bFinishLine = false;
	public bool         bMagnetize = false;
    public bool         bSplit = false;
    public int          iDirection = 1;
    public int          iPolarity = 0;
    public int          iWeight = 1;
	public GameObject[] nextPoint;
    public GameObject loopFinish;

    void Start()
    {
        if (!bSplit)
        {
            nextPoint = new GameObject[10];
        }
    }
	//Setters
	public void setNextPoint(GameObject pNextPoint, int index){
        print("Setting next point to: " + pNextPoint.name);
        nextPoint[index] = pNextPoint;
    }
	public void setbMagnetize(bool pbMagnetize){bMagnetize = pbMagnetize;}

	//Getters
	public GameObject[] getNextPoint(){return nextPoint;}
	public GameObject getPoint(){return this.gameObject;}
	public bool getbMagnetize(){return bMagnetize;}
    public bool getbSplit(){return bSplit;}
    public bool getbFinishLine(){return bFinishLine;}
    public int getiPolarity(){return iPolarity;}
    public int getiWeight(){return iWeight;}
}
