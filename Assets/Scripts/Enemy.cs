using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float speed = 0.0001f;
    public int health = 100;

    private Transform target;
    private EnemyState CurrentEnemyState;

    private float StandingTimer = 0.0f;
    private float StandingDuration = 3.0f;

    private float StandingAngle;
    private Quaternion StandingTransition;

	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Castle").transform;
        CurrentEnemyState = EnemyState.Moving;
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision impulse: " + collision.impulse);
        health -= (int) collision.impulse.y * 10;
        if (health <= 0)
        {
            Debug.Log("Killed enemy");
            Destroy(gameObject);
        }

        if (collision.collider.gameObject.tag.Equals("Table") && CurrentEnemyState == EnemyState.Flying)
        {
            CurrentEnemyState = EnemyState.Standing;
            StandingTimer = 0.0f;
            //StandingTransition = Quaternion.Inverse(transform.rotation) * Quaternion.identity;
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
}

public enum EnemyState
{
    Moving,

    Standing,

    Attacking,

    Flying
} 