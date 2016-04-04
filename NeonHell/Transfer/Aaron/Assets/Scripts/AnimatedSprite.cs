using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimatedSprite : MonoBehaviour {
    //The above sheet has 1
    public int iFrames;
    public float framesPerSecond = 10.0f;
    public Texture[] DiffuseFrames;
    public Texture[] EmissionFrames;

	// Use this for initialization
	void Start () {
        StartCoroutine("animateTexture");
	}
    IEnumerator animateTexture()
    {
        for (int i = 0; i < iFrames; i++)
        {

            GetComponent<Renderer>().material.SetTexture("_MainTex", DiffuseFrames[i]);
            GetComponent<Renderer>().material.SetTexture("_EmissionMap", EmissionFrames[i]);
            yield return new WaitForSeconds(1.0f / framesPerSecond);
        }
        StartCoroutine("animateTexture");
    }
}
