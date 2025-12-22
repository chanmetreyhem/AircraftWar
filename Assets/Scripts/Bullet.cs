using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    private Rigidbody rb;
    private float force = 15f;
    public Vector3 target;
    private float lifeTime = 1f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject,lifeTime);
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    public void FixedUpdate()
    {
        rb.AddForce(target * force * Time.fixedDeltaTime, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(effect, transform.position, Quaternion.identity);
            other.GetComponent<Aircraft>().OnAttack();
        }
        
    }
}
