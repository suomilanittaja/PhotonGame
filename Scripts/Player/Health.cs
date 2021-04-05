using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{

    //player health
    public int health = 100;

    //healthbar which is not ready
    public Slider _health;


    //sync health
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
        }
    }

    //take damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }

    //Respawn
    IEnumerator Respawn()
    {
        health = 100;
        GetComponent<PlayerController>().enabled = false;
        transform.position = new Vector3(0, 10, 0);
        yield return new WaitForSeconds(1);
        GetComponent<PlayerController>().enabled = true;
    }

    void Update()
    {
        if (health <= 0)
        {
            StartCoroutine(Respawn());
        }

    }
}
