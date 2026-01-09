using UnityEngine;
using System.Collections;

public class YellowWallBox : MonoBehaviour
{
    private float breakVelocity = 5f;
    public GameObject breakFX;
    private float fxDuration = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        float impactSpeed = collision.relativeVelocity.magnitude;


        if (impactSpeed >= breakVelocity)
        {
            Break(collision.gameObject.transform);
        }
    }

    private void Break(Transform followTarget)
    {
        if (breakFX != null)
        {
            GameObject fx = Instantiate(breakFX, transform.position, Quaternion.identity);
            GameManager.instance.AddExtraScore(100);
            GameManager.instance.ShowSmashText();
            GameManager.instance.FullEnergy();
            fx.transform.SetParent(followTarget);

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
