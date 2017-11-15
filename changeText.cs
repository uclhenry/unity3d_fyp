using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Text _currencyText = GameObject.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<Text>();
        Debug.Log(GameObject.Find("Canvas").transform.GetChild(0).gameObject.name);
        //Text _currencyText = GameObject.Find("Canvas").GetComponentInChildren<Text>();
        _currencyText.text = "This is a test!";
    }
}
