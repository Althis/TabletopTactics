using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour {

	Transform trans;
	public float rotationSpeed = 5.0f;
	public float translationSpeed = 20.0f;
	int screeWidth;
	int screenHeight;
	int boundary = 75;

	void Start () {
		trans = GetComponent<Transform> ();
		screeWidth = Screen.width;
		screenHeight = Screen.height;
	}

	void Update () {
		
		if (Input.GetKey("x")){
			float h = rotationSpeed * Input.GetAxis ("Mouse X");
			float v = rotationSpeed * Input.GetAxis ("Mouse Y");
			trans.Rotate(new Vector3(0, h, 0));
		}

		if (Input.GetKey(KeyCode.W) /*|| (Input.mousePosition.y > screenHeight - boundary)*/){
			trans.Translate (new Vector3 (0, 0, 1) * Time.deltaTime * translationSpeed);
		}

		if (Input.GetKey(KeyCode.S) /*|| (Input.mousePosition.y < 0 + boundary)*/){
			trans.Translate (new Vector3 (0, 0, -1) * Time.deltaTime * translationSpeed);
		}

		if (Input.GetKey(KeyCode.A) /*|| (Input.mousePosition.x < 0 + boundary)*/){
			trans.Translate (new Vector3 (-1, 0, 0) * Time.deltaTime * translationSpeed);
		}

		if (Input.GetKey(KeyCode.D) /*|| (Input.mousePosition.x > screeWidth - boundary)*/){
			trans.Translate (new Vector3 (1, 0, 0) * Time.deltaTime * translationSpeed);
		}

        if (Input.GetKey(KeyCode.T) /*|| (Input.mousePosition.x < 0 + boundary)*/)
        {
            trans.Translate(new Vector3(0, 1, 0) * Time.deltaTime * translationSpeed);
        }

        if (Input.GetKey(KeyCode.G) /*|| (Input.mousePosition.x > screeWidth - boundary)*/)
        {
            trans.Translate(new Vector3(0, -1, 0) * Time.deltaTime * translationSpeed);
        }

        if (Input.GetKey(KeyCode.Q) /*|| (Input.mousePosition.x > screeWidth - boundary)*/)
        {
            trans.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * rotationSpeed);
        }

        if (Input.GetKey(KeyCode.E) /*|| (Input.mousePosition.x > screeWidth - boundary)*/)
        {
            trans.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * rotationSpeed);
        }

        if (Input.GetKey(KeyCode.R) /*|| (Input.mousePosition.x > screeWidth - boundary)*/)
        {
            trans.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * rotationSpeed);
        }

        if (Input.GetKey(KeyCode.F) /*|| (Input.mousePosition.x > screeWidth - boundary)*/)
        {
            trans.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * rotationSpeed);
        }


    }
} 