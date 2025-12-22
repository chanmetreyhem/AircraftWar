using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(AudioSource))]
public class Tank : MonoBehaviour
{
    [SerializeField]private GameObject bullet;
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
                StartCoroutine(ShotCoroutine(3));

            }
        }
       
        



    }

    IEnumerator ShotCoroutine(int amount)
    {
        for (int i = 0; i < amount; i++) {
            var b = Instantiate(bullet, bulletPos.position, targetRotation);
            if (player)
                b.GetComponent<Bullet>().SetTarget(bulletPos.forward);
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
