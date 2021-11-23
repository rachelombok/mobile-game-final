using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots : MonoBehaviour
{
    //private void OnCollisionEnter2D(Collision collision)
    //{
    //    if (collision.collider.gameObject.CompareTag("enemy"))
    //    {
    //        Destroy(collision.collider.gameObject);
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
