using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    private GameObject releasedObj;
    private GameObject selectedObj;
    private GameObject grabbableEnemy;
    public Transform gripTransform;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device _controllerDevice;
    private SteamVR_TrackedController _controller;

    private Vector3[] positions = new Vector3[2]; // positions[0] last frame, positions[1] this frame;
    private Vector3 releasedVelocity;
    private bool justReleased; 

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
        releasedVelocity = new Vector3(-99,-99,-99);
        justReleased = false;

        releasedObj = null;
        selectedObj = null;
        grabbableEnemy = null;
        gripGripped = false;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (selectedObj)
        {
            Debug.Log("Updating velocity");
            positions[0] = positions[1];
            positions[1] = selectedObj.transform.position;
            Debug.Log("Selected obj velocity: " + selectedObj.GetComponent<Rigidbody>().velocity);
        }
       
        if (!releasedVelocity.Equals(new Vector3(-99,-99,-99)) && justReleased)
        {
            selectedObj.GetComponent<Rigidbody>().velocity = releasedVelocity; // this will correct the odd velocity issue
            justReleased = false;
        }
        if (releasedObj)
        {
            Debug.Log("Released V: " + selectedObj.GetComponent<Rigidbody>().velocity);
        }
        if (_controllerDevice.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("Gripped");
            if (grabbableEnemy)
            {
                Debug.Log("Grab sequence begins");
                selectedObj = grabbableEnemy;
                Debug.Log(selectedObj.name);
                selectedObj.transform.position = gripTransform.position;
                var joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = selectedObj.GetComponent<Rigidbody>();
            }
        }
        if (_controllerDevice.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("Ungripped");
            if (gameObject.GetComponent<FixedJoint>() && selectedObj)
            {
                Debug.Log("Launch sequence begins");
                releasedVelocity = (positions[1] - positions[0]) / Time.deltaTime;
                
                foreach (FixedJoint joint in gameObject.GetComponents<FixedJoint>()) {
                    joint.connectedBody = null;
                    Destroy(joint);
                    //Debug.Log("Fixed Joint value: " + joint + " & Component value: " + gameObject.GetComponent<FixedJoint>());
                }
                //releasedObj = GameObject.Instantiate(selectedObj);
                selectedObj.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Rigidbody>().velocity;
                selectedObj.GetComponent<Rigidbody>().angularVelocity = gameObject.GetComponent<Rigidbody>().angularVelocity;

                Debug.Log("Released @ release: " + selectedObj.GetComponent<Rigidbody>().velocity);
                //justReleased = true;

                // For garbage collection?
                selectedObj = null;      
            }
        }
	}

    private void LateUpdate()
    {
        //if (!_controllerDevice.GetPress(SteamVR_Controller.ButtonMask.Grip))
        //    selectedObj = null;
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

    private void OnTriggerExit(Collider other)
    {
        grabbableEnemy = null;
    }
}
