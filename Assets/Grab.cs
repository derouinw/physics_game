using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    private GameObject selectedObj;
    private GameObject grabbableEnemy;
    public Transform gripTransform;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device _controllerDevice;
    private SteamVR_TrackedController _controller;

    private Vector3[] positions = new Vector3[2]; // positions[0] last frame, positions[1] this frame;

    private bool gripGripped;

    void Awake()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () {
        _controllerDevice = SteamVR_Controller.Input((int)trackedObject.index);
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.Gripped += HandleGripGripped;
        _controller.Ungripped += HandleGripUngripped;
        positions[0] = Vector3.zero;
        positions[1] = Vector3.zero;

        selectedObj = null;
        grabbableEnemy = null;
        gripGripped = false;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (selectedObj)
        {
            positions[0] = positions[1];
            positions[1] = selectedObj.transform.position;
        }
        if (_controllerDevice.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            if (grabbableEnemy)
            {
                selectedObj = grabbableEnemy;
                Debug.Log(selectedObj.name);
                selectedObj.transform.position = gripTransform.position;
                var joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = selectedObj.GetComponent<Rigidbody>();
            }
        }
        else
        {
            if (gameObject.GetComponent<FixedJoint>() && selectedObj)
            {
                Vector3 velocity = (positions[1] - positions[0]) / Time.deltaTime;
                selectedObj.GetComponent<Rigidbody>().velocity = velocity;
                Destroy(gameObject.GetComponent<FixedJoint>());
                Debug.Log(velocity);
                selectedObj = null;
            }
        }
	}

    private void HandleGripGripped(object sender, ClickedEventArgs e)
    {
        gripGripped = true;
       
    }

    private void HandleGripUngripped(object sender, ClickedEventArgs e)
    {
        gripGripped = false;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Triggered");
        if (other.gameObject.tag == "Enemy")
        {
            grabbableEnemy = other.gameObject;
        }
    }
}
