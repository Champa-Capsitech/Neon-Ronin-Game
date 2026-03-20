using System.Collections;
using UnityEngine;

public class YellowWallBox : MonoBehaviour
{
    private float breakVelocity = 5f;
    public GameObject breakFX;
    private float fxDuration = 0.5f;
    private bool isBroken = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBroken)
            return;
        if (!collision.gameObject.CompareTag("Player"))
            return;
        float impactSpeed = collision.relativeVelocity.magnitude;
        AnalyticsLogger.LogYellowCollision();

        if (impactSpeed >= breakVelocity)
        {
            isBroken = true;

            GameManager.instance.PlayWallSmashSound();

            Break(collision.gameObject.transform);
        }
    }

    private void Break(Transform followTarget)
    {
        if (breakFX != null)
        {
            AnalyticsLogger.LogYellowBreak();
            GameObject fx = Instantiate(breakFX, transform.position, Quaternion.identity);
            fx.transform.SetParent(followTarget);
            GameManager.instance.AddExtraScore(100);
            GameManager.instance.ShowSmashText();
            GameManager.instance.FullEnergy();

            StartCoroutine(DestroyFXAfterTime(fx));
        }

        Destroy(gameObject);
    }

    private IEnumerator DestroyFXAfterTime(GameObject fx)
    {
        yield return new WaitForSeconds(fxDuration);
        Destroy(fx);
    }
}
