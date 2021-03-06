﻿using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    private GameObject player;

    private float speed;

    [SerializeField] public float circleColliderRadius;
    [SerializeField] private float distanceWithLight;
    [SerializeField] private float seekTheLeaderProbability = 0.2f;
    private bool preventingTheLight;
    private float timeCountDown;
    private Vector3 prevStreetLigthCollider;

    private List<GameObject> humans = new List<GameObject>();

    public void SetSpeed(float newSpeed) { speed = newSpeed; }

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerBehaviour>().gameObject;
        gameObject.GetComponent<CircleCollider2D>().radius = circleColliderRadius;
        preventingTheLight = false;
        timeCountDown = 3f;
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.IsGameOver()) return;
        // TODO: need to balance game at here
        // ...


        // Find the nearest human
        if (preventingTheLight)
        {
            // Moving the opposite with the light 
            var desired_velocity = (this.transform.position - prevStreetLigthCollider).normalized * speed * Time.deltaTime;
            transform.position += desired_velocity;

            // Check 
            if (Vector3.Distance(transform.position, prevStreetLigthCollider) >= distanceWithLight)
            {
                preventingTheLight = false;
                // TODO: dont set hard code here
                timeCountDown = 3f;

            }
        }
        else if (timeCountDown > 0)
        {
            timeCountDown -= Time.deltaTime;
        }
        else
        {
            var closestPlayer = FindClosestPlayer();
            if (closestPlayer)
            {
                transform.position = Vector2.MoveTowards(transform.position, closestPlayer.transform.position,
                    speed * Time.deltaTime);
            }
        }
    }

    public GameObject FindClosestPlayer()
    {
        humans = player.GetComponent<PlayerParty>().humans;
        var r = Random.Range(0, 1);
        if (r < seekTheLeaderProbability)
        {
            return humans[0];
        }

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        for (int i = 0; i < humans.Count; i++)
        {
            var go = humans[i].gameObject;
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        if (humans.Count == 0)
        {
            return FindObjectOfType<PlayerBehaviour>().gameObject;
        }

        return closest;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle end game
        if (other.GetComponent<StreetLight>())
        {
            preventingTheLight = true;
            prevStreetLigthCollider = other.transform.position;
        }
        else if (other.GetComponent<PlayerBehaviour>())
        {
            EnemyManager.instance.ClearAllEnemy();
            other.GetComponent<PlayerBehaviour>().enabled = false;
            GameManager.instance.EndGame();
        }

    }


}
