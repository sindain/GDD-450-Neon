using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public static bool isRotate = true;
	public float speed;
	// Use this for initialization
	void Start () {
		isRotate = true;
	}

	void Update () {
		if (isRotate) {
			transform.Rotate (new Vector3 (0, speed, 0) * Time.deltaTime);
		}
	}
}
