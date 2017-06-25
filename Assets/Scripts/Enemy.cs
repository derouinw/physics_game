using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float speed = 0.0001f;
	[Tooltip("Degree of impulse enemy can suffer without taking damage")]
	public float fallThreshold = 0.1f; 
    public int health = 100;


    private Transform target;
    public EnemyState CurrentEnemyState;

    private float StandingTimer = 0.0f;
    private float StandingDuration = 3.0f;

    private float StandingAngle;
    private Quaternion StandingTransition;

	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Castle").transform;
	}

    void OnCollisionEnter(Collision collision)
    {
        

        if (collision.collider.gameObject.tag.Equals("Table") && CurrentEnemyState == EnemyState.Flying)
        {
            CurrentEnemyState = EnemyState.Standing;
            StandingTimer = 0.0f;
            
			Debug.Log("Collision impulse: " + collision.impulse);
			if (collision.impulse.y > fallThreshold) {
				// Remove health upon impact
				health -= (int)collision.impulse.y * 10;
				if (health <= 0) {
					Debug.Log ("Killed enemy");
					Destroy (gameObject);
					return;
				}

				// Recolor according to health
				gameObject.GetComponent<Renderer> ().material.color = GetColorFromHealth (health);
			}
        }
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Current state: " + CurrentEnemyState);
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
        switch(CurrentEnemyState)
        {
            case EnemyState.Moving:
                var targetPosition = target.position;
                targetPosition.y = transform.position.y;
                if (transform.position.y <= 0.75f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
                }
                break;
            case EnemyState.Standing:
                StandingTimer += Time.deltaTime;
                if (StandingTimer < StandingDuration)
                {
                    // noop                    
                }
                else
                {
                    CurrentEnemyState = EnemyState.Moving;
                    transform.eulerAngles = Vector3.zero;
                }

                break;
            default:
                break;
        }
	}

    public void OnPlayerInteraction()
    {
        CurrentEnemyState = EnemyState.Flying;
    }

	private Color GetColorFromHealth(int health)
	{
		// return a new shade of gray for now (eventually could turn from green to red) --> HWPro for this function
		return new Color ((float)health / 100f, (float)health / 100f, (float)health / 100f);
	}	
}

public enum EnemyState
{
    Moving,

    Standing,

    Attacking,

    Flying
} 