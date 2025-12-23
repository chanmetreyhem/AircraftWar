using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Pool;
public class Bullet : MonoBehaviour
{
    private IObjectPool<Bullet> objectPool;
    public IObjectPool<Bullet> ObjectPool { get { return objectPool; } set => objectPool = value; }

    [SerializeField] private GameObject effect;
    private Rigidbody rb;
    private float force = 15f;
    public Vector3 target;
    private float lifeTime = 2f;
    private Tank _tank;
    public Tank Tank { get => _tank;  set => _tank = value; }
    private bool isTriger = false;
    public bool IsTriger { get => isTriger ; set => isTriger = value; }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Destroy(gameObject,lifeTime);
       StartCoroutine(ReturnToPoolCoroutine());
    }

    public IEnumerator SetupRb()
    {
        print(nameof(SetupRb));
        lifeTime = 2;
        isTriger = false;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; 
        rb.Sleep();
        yield return new WaitForFixedUpdate();
        rb.WakeUp();
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    public void FixedUpdate()
    {
        if (isTriger) return;
        rb.AddForce(target * force * Time.fixedDeltaTime, ForceMode.Impulse);
        lifeTime -= Time.deltaTime;
        if (!isTriger && lifeTime <=0) {

           objectPool.Release(this);
           isTriger = true;
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(effect, transform.position, Quaternion.identity);
            other.GetComponent<Aircraft>().OnAttack();
            isTriger = true;
           
         
           objectPool.Release(this);
        }
        
        
    }
    IEnumerator ReturnToPoolCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
       
    }
}
