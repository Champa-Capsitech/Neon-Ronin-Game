using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameManager.instance.PlayCrashSound();
        GameManager.instance.GameOver(gameObject);
        gameObject.SetActive(false);
    }
}
