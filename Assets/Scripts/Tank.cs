using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(AudioSource))]
public class Tank : MonoBehaviour
{
    private IObjectPool<Bullet> m_bulletPool;
    [SerializeField] private Bullet bulletPre;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform bulletPos;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private AudioClip clip;

    private GameObject player;
    float rotationSpeed = 100.0f;

    private float currentHealth =  1000;
    private float maxHealth = 1000;
    private float detectionRange = 20f;
    private float shootOverTime = 2f;
    private Quaternion targetRotation;
    private AudioSource audioSource;

    
    private bool collectionCheck;
    private void Awake()
    {
        m_bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, collectionCheck, 3, 6);
    }

    Bullet CreateBullet()
    {
        Bullet bulletInstance  = Instantiate(bulletPre);
        bulletInstance.ObjectPool = m_bulletPool;
        return bulletInstance;
    }

    void OnGetFromPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
        bullet.IsTriger = false;
        StartCoroutine(bullet.SetupRb());
    }


    void OnReleaseToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    void OnDestroyPooledObject(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        healthSlider.value = 1;
        player = FindAnyObjectByType<Aircraft>().gameObject;
        
    }

    private void Update()
    {
        if (FindAnyObjectByType<GameManager>().isEndGame) return;

        if (player != null)
        {
            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(turret.position, player.transform.position);

         
                Vector3 directionToTarget = player.transform.position - turret.position;

                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                this.targetRotation = targetRotation;

                turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            shootOverTime -= Time.deltaTime;
            if (shootOverTime < 0f)
            {
                shootOverTime = 2f;
                StartCoroutine(ShotCoroutine(1));

            }
        }
       
        



    }


  
  

 
    IEnumerator ShotCoroutine(int amount)
    {
        for (int i = 0; i < amount; i++) {
            var bulletScript = m_bulletPool.Get();
            //yield return b;
            if (player)
            {
                bulletScript.transform.position = bulletPos.position;
                bulletScript.transform.rotation = targetRotation;
               // var bulletScript = b.GetComponent<Bullet>();
                bulletScript.SetTarget(bulletPos.forward);
            }
              
                
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(0.1f);
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boom"))
        {
            currentHealth -= Random.Range(100, 200);
            healthSlider.value = currentHealth/maxHealth;
            if(currentHealth <= 0)
            {
                FindAnyObjectByType<GameManager>().tanks.Remove(this);
                Destroy(gameObject,.5f);
            }
        }
    }
}
