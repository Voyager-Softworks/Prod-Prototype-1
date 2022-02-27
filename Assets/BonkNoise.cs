using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkNoise : MonoBehaviour
{
    public AudioSource bonkSource;
    public AudioClip[] bonkClips;
    public AudioClip[] damageClips;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyBullet")
        {
            bonkSource.clip = damageClips[Random.Range(0, damageClips.Length)];
            bonkSource.pitch = Random.Range(0.8f, 1.2f);
            bonkSource.Play();
        }
        else
        {
            bonkSource.clip = bonkClips[Random.Range(0, bonkClips.Length)];
            bonkSource.pitch = Random.Range(0.4f, 0.8f);
            bonkSource.Play();
        }
        
    }
}
