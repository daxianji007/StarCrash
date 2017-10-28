using UnityEngine;
using System.Collections;
using System.Reflection;

public class RotationController : MonoBehaviour {
    public Vector3 d_vector;
    public Behaviour halo;
    public GameObject exposion;

    // Use this for initialization
    void Start ()
    {
        //halo = GetComponent("Halo") as Behaviour;
        //halo.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (CompareTag("Selected"))
        {
            //transform.Rotate(d_vector * Time.deltaTime * -1);
        }
        else
        {
            //enabled.SetValue(halo, false, null);
            transform.Rotate(d_vector * Time.deltaTime);
        }
    }
}