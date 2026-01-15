using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private float breakVelocity = 10f;
    private int scoreReward = 500;
    private float rotationSpeed = 90f;

    public GameObject breakFX;
    private float fxDuration = 0.5f;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        float playerSpeed = playerRb.linearVelocity.magnitude;

        if (playerSpeed >= breakVelocity)
        {
            SpawnBreakFX(other.transform);
            GameManager.instance.AddExtraScore(scoreReward);
            GameManager.instance.ShowExecutedText();
            GameManager.instance.FullEnergy();
            Destroy(gameObject);
        }
        else
        {
            GameManager.instance.GameOver();
            Destroy(other.gameObject);
        }
    }

    private void SpawnBreakFX(Transform followTarget)
    {
        if (breakFX == null) return;

        GameObject fx = Instantiate(breakFX, transform.position, Quaternion.identity);
        fx.transform.SetParent(followTarget);

        StartCoroutine(DestroyFXAfterTime(fx));
    }

    private IEnumerator DestroyFXAfterTime(GameObject fx)
    {
        yield return new WaitForSeconds(fxDuration);
        Destroy(fx);
    }
}
