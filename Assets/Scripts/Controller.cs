using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public GameObject Explosion;
    public float Radius = 0.15f;
    public float Power = 1000.0f;

    /// SteamVR controllers that should be polled.
    [Tooltip("Array of SteamVR controllers that may used to fly around. Set to size 1 (right) so the other is free for adjustment purposes.")]
    public SteamVR_TrackedObject[] Controllers;
    private SteamVR_Controller.Device[] _devices;

    private bool[] triggerPressed;

    // Use this for initialization
    void Start () {
        _devices = new SteamVR_Controller.Device[Controllers.Length];
        triggerPressed = new bool[Controllers.Length];
        UpdateControllers();
    }
	
	// Update is called once per frame
	void Update () {
        if (!UpdateControllers())
        {
            return;
        }

        for (int i = 0; i < _devices.Length; i++)
        {
            var device = _devices[i];
            var controller = Controllers[i];

            triggerPressed[i] = device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
            // if trigger being pressed
            if (triggerPressed[i])
            {
                // make boom
                RaycastHit hit;
                if (Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, 300))
                {
                    Debug.Log("Boom");

                    Vector3 explosionPos = hit.point;
                    //var cube = Instantiate(Explosion, explosionPos, Quaternion.identity);
                    //cube.SetActive(true);

                    Collider[] colliders = Physics.OverlapSphere(explosionPos, Radius);
                    foreach (Collider collision in colliders)
                    {
                        var enemy = collision.gameObject.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.OnPlayerInteraction();
                        }

                        Rigidbody rb = collision.GetComponent<Rigidbody>();

                        if (rb != null)
                            rb.AddExplosionForce(Power, explosionPos, Radius, 1.0F);
                        else
                            Debug.Log("No rb!");
                    }
                }
            }
            else
            {
                // something
            }
        }
    }

    bool UpdateControllers()
    {
        for (int i = 0; i < Controllers.Length; i++)
        {
            var controller = Controllers[i];
            var index = (int)controller.index;
            if (index == -1)
            {
                //Debug.Log("Skipped device " + i);
                return false;
            }

            if (_devices[i] != null)
            {
                //Debug.Log("Already set " + i);
                continue;
            }

            _devices[i] = SteamVR_Controller.Input(index);
            Debug.Log("Set controller " + i);
        }

        return true;
    }
}
