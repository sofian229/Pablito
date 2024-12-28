using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteractor : MonoBehaviour
{
    public int playerMaxHealth = 3;
    public int playerHealth;
    public int damage = 1;
    public bool playerTookDamage = false;
    public bool enemyIsAlive = true;
    public float damagePushBack = 20f;
    public float damagePushUp = 30f;

    public Rigidbody rb;

    void Start()
    {
        playerMaxHealth = playerHealth;
        rb = GetComponent<Rigidbody>();
    }

//    void Update()
//    {
//        void damageThrow()
//        {
//            rb.AddForce(damagePushBack, damagePushUp, damagePushBack, ForceMode.Impulse);
//            //controller.Move(velocity * Time.deltaTime);
//        }
//    }

    private void OnTriggerEnter(Collider hit)
    {
        if(hit.transform.tag == "Take Damage")
        {
            //playerMaxHealth - damage = playerHealth;
            Debug.Log("Damage Taken.");
            playerTookDamage = true;
            rb.AddForce(damagePushBack, damagePushUp, damagePushBack, ForceMode.Impulse);
//            damageThrow();
        }

       if(playerHealth <= 0)
        {
            Debug.Log("Player Died.");
        }

        if(hit.transform.tag == "Die Enemy")
        {
            Destroy(hit.gameObject);
            Debug.Log("Enemy Died.");
            enemyIsAlive = false;
        }
    }
}