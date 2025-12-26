using UnityEngine;

public class YellowWallBox : MonoBehaviour
{
    [Header("Break Settings")]
    private float breakVelocity = 12f;

    [Header("FX")]
    public GameObject breakFX;

    //bool isBroken = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //if (isBroken)
        //    return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed >= breakVelocity)
        {
            Break();
        }
    }

    private void Break()
    {


        if (breakFX != null)
        {
            Instantiate(breakFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
