using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionScript : MonoBehaviour
{
    public Collider2D coll;
    public Animator anim;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Asteroid"))
        {
            //si se choca con el asteroide, que le quite vida y haga lo de la corrutina
            GameManager.instance.DecreaseHealth();
            StartCoroutine(InvencibleTime());
        }
    }


    IEnumerator InvencibleTime()
    {
        //deactiva el collider y hace una animación
        //tras un tiempo lo vuelve a activar y vuelve a la animación default
        anim.SetBool("damaged", true);
        coll.enabled = false;
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("damaged", false);
        coll.enabled = true;
    }
}
