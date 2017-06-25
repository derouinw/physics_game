using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject EnemyTemplate;
    public double SpawnTime = 2.0f;

    private double currentTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if (currentTime > SpawnTime)
        //if (currentTime < 0)
        {
            // spawn new enemy
            var newEnemy = Instantiate(EnemyTemplate);
            newEnemy.SetActive(true);

            var position = Random.insideUnitCircle.normalized;
            newEnemy.transform.position = new Vector3(position.x * 0.70f, 0.75f, position.y * 0.70f);
            
            // reset spawn
            currentTime = 0.0f;
            if (SpawnTime > 0.5f)
            {
                //SpawnTime -= 0.05f;
            }
        }
	}
}
