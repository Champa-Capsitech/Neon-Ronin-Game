using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float breakVelocity = 9f;
    private float rotationSpeed = 90f;

    public GameObject breakFX;
    private float fxDuration = 0.5f;
    private bool isBroken = false;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBroken)
            return;
        if (!other.CompareTag("Player"))
            return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null)
            return;

        float playerSpeed = playerRb.linearVelocity.magnitude;

        if (playerSpeed >= breakVelocity)
        {
            isBroken = true;
            SpawnBreakFX(other.transform);
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
        if (breakFX == null)
            return;

        GameObject fx = Instantiate(breakFX, transform.position, Quaternion.identity);
        fx.transform.SetParent(followTarget);
        GameManager.instance.AddExtraScore(500);
        GameManager.instance.ShowExecutedText();
        GameManager.instance.FullEnergy();

        StartCoroutine(DestroyFXAfterTime(fx));
    }

    private IEnumerator DestroyFXAfterTime(GameObject fx)
    {
        yield return new WaitForSeconds(fxDuration);
        Destroy(fx);
    }
}
