﻿using System.Collections;
using UnityEngine;

public class StreetLight : MonoBehaviour
{

    public GameObject itemInside;
    public float fadeDuration = 5.0f;
    public float minSpawnObjectTime = 2.5f;
    public float maxSpawnObjectTime = 5f;
    public GameObject[] spawnObjects;
    public GameObject objectInside;
    [SerializeField] private float reduceFactor;
    [SerializeField] public float circleColliderRadius;
    private bool isInside;
    private Vector3 currentScale;  
    private float startTime;
    private SpriteRenderer sprite;
    

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        gameObject.GetComponent<CircleCollider2D>().radius = circleColliderRadius;
        currentScale = transform.localScale;

        SpawnObjectInside();
    }

    public void CoolDownSpawnObject()
    {
        var coolDownTime = Random.Range(minSpawnObjectTime, maxSpawnObjectTime);
        StartCoroutine(SpawnObjectAfterCoolDown(coolDownTime));
    }

    IEnumerator SpawnObjectAfterCoolDown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        SpawnObjectInside();
    }

    void Fading()
    {
        if (sprite.color.a < 0.3f) return;
        float t = 0.5f*(Time.time - startTime) / fadeDuration;
        sprite.color = new Color(1f, 1f, 1f, sprite.color.a - Mathf.SmoothStep(0, 1, t));
    }

    private void SpawnObjectInside()
    {
        var randomIndex = Random.Range(0, spawnObjects.Length);
        var spawnObject = Instantiate(spawnObjects[randomIndex], transform);
        if (spawnObject.GetComponent<HumanBehaviour>())
        {
            spawnObject.transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerBehaviour>())
        {
            startTime = Time.time;
            isInside = true;
            StartCoroutine(ScaleDown());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerBehaviour>())
        {
            isInside = false;
            StopCoroutine(ScaleDown());
        }
    }

    IEnumerator ScaleDown()
    {
        float timer = 0;

        while (transform.localScale.x > 0 && isInside)
        {
            timer += Time.deltaTime;
            transform.localScale -= currentScale * Time.deltaTime * reduceFactor;
            Fading();
            yield return null;
        }
        if (transform.localScale.x < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
