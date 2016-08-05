using UnityEngine;
using System.Collections;

public class TestRot : MonoBehaviour {

    public float angle = 1; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, angle);
	}
}
