using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IETest : MonoBehaviour {

	bool canRotate;
	// Use this for initialization
	IEnumerator Start () {
		//StartCoroutine("startRotate");
		canRotate = !canRotate;
		yield return new WaitForSeconds(2f);
		canRotate = !canRotate;
		yield return new WaitForSeconds(2f);
		canRotate = !canRotate;
	}
	
	// Update is called once per frame
	void Update () {
		if(canRotate)
		this.transform.Rotate(new Vector3(0, 1, 0));
	}

	IEnumerator startRotate()
	{
		canRotate = !canRotate;
		yield return new WaitForSeconds(2f);
		canRotate = !canRotate;
		yield return new WaitForSeconds(2f);
		canRotate = !canRotate;
	}
}
