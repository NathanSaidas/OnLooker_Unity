using UnityEngine;
using System.Collections;

public class TempBounds : MonoBehaviour {
    public Transform cam;
    public Vector3 offset;
	// Use this for initialization
	void Start () {
        cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //if(cam != null)
        //{
        //    transform.position = cam.position + cam.rotation * offset;
        //}
	}
    void LateUpdate()
    {
        if (cam != null)
        {
            transform.position = cam.position + cam.rotation * offset;
        }
    }
}
