using UnityEngine;
using System.Collections;

public class BuildingGeneration : MonoBehaviour {

	//public variables
	public GameObject building;
	public int iStreetSize = 25;
	public int iBlockWidth = 80;
	public int iBlockDepth = 275;
	public int iNumBlockW = 18;
	public int iNumBlockD = 5;
	public int iLotsPerBlock = 4;
	
	//private variables
	private int iNumBlocks;

	
	// Use this for initialization
	void Start () {
		iNumBlocks = iNumBlockW * iNumBlockD;
		
		//Main loop for building generation
		for(int i = 0; i < iNumBlocks; i++){
			for(int j = 0; j < iLotsPerBlock*2; j++){
				//Pick a random height for this building   ****TEMPORARY*****
				//Algorithm for making buildings taller towards center of city
				int iX = iNumBlockW /2 - Mathf.Abs (iNumBlockW/2 - i%iNumBlockW);
				int iY = iNumBlockD/2 - Mathf.Abs (iNumBlockD/2-i/iNumBlockW);

				//print("X: " + iX.ToString() + " Y: " + iY.ToString());
				//int height = ((iX + iY) / 2); 
				//height = Random.Range((int)(height/3), (int)(height*3));
				//height *=height;
				int height = Random.Range (20, 100) * 3;

				if(height != 0){
					//Find the position of this building       **** SORT OF TEMPORARY ****
					float x = i%iNumBlockW * (iBlockWidth + iStreetSize) //Block location
							+ j%2*(iBlockWidth/2); //Building location inside of block
					float z = i/iNumBlockW * (iBlockDepth + iStreetSize) //Block location
							+ j/2*(iBlockDepth/iLotsPerBlock); //Building location inside of block
					Vector3 position = new Vector3(-(iBlockWidth + iStreetSize)*iNumBlockW/2 + iStreetSize + x, //Extra calculations to keep city centered
												   height/2,
												   -(iBlockDepth + iStreetSize)*iNumBlockD/2 + iStreetSize + z); //Extra calculations to keep city centered
					//Create Building
					GameObject ob = Instantiate(building, position, Quaternion.identity) as GameObject;
					ob.transform.localScale = new Vector3(iBlockWidth/2,height,iBlockDepth/iLotsPerBlock);
				}
			}//End for(int j = 0; j < iLotsPerBlock*2; j++)
		}//End for(int i = 0; i < iNumBuildings; i++)
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
