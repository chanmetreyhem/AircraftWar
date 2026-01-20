
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Audio;


public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Transform target;                  // គោលដៅ (Transform)
    private Rigidbody rb;

    [Header("Boost (Phase 1)")]
    public float boostDuration = 1.0f;        // រយៈពេល boost ទៅមុខ
    public float boostForce = 80f;            // កម្លាំង boost (N) ឬ addForce Acceleration mode

    [Header("Homing (Phase 2)")]
    public float maxSpeed = 12f;             // ល្បឿនអតិបរមា
    public float acceleration = 60f;          // អត្រាបន្ថែមល្បឿន
    public float turnRateDegPerSec = 180f;    // អត្រាបង្វែរ deg/sec
    public float seekGain = 1.0f;             // Strength for steering

    [Header("Guidance")]
    private float proximityDetonateRadius = 0.1f; // ជិតគម្លាតសំអាត/ចំគោល

    private float startTime;
    private bool homingEnabled = false;
    public GameObject effect;

    private float lifeTime = 3f;
    void Awake()
    {
        lifeTime = 3f;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;                // Missile ធម្មតាមិនធ្លាក់ក្រោមទំនាញ (game-style)
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            // បន្តទៅមុខ តាមទិសបច្ចុប្បន្ន
            rb.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
            ClampSpeed();
            lifeTime -= Time.deltaTime;
            if(lifeTime < 0)
            {
                Destroy(gameObject);
            }
            return;
        }

        float elapsed = Time.time - startTime;

        // Phase 1: Boost Forward Only
        if (!homingEnabled)
        {
            rb.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
            ClampSpeed();

            if (elapsed >= boostDuration)
                homingEnabled = true;

            return;
        }

       
        Vector3 toTarget = (target.position - transform.position);
        float distance = toTarget.magnitude;
        Vector3 desiredDir = toTarget.normalized;

      
        float maxRadiansThisStep = Mathf.Deg2Rad * turnRateDegPerSec * Time.fixedDeltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, desiredDir, maxRadiansThisStep, 0f);

       
        transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);

       
        rb.AddForce(transform.forward * acceleration * seekGain, ForceMode.Acceleration);
        ClampSpeed();

       
        if (distance <= proximityDetonateRadius)
        {
            OnHitTarget();
        }
    }

    private void ClampSpeed()
    {
        if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void OnHitTarget()
    {

        Destroy(gameObject);
    }


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.CompareTag("Land") || other.gameObject.CompareTag("Enemy"))
        { 
            var effectClone =  Instantiate(effect,transform.position,Quaternion.identity);
            effectClone.transform.rotation = transform.rotation;
            Destroy(effectClone,1f);
            Destroy(gameObject, 0.05f);
        }
    }
}
