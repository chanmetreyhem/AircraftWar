using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(AudioSource))]
public class Aircraft : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public InputAction launch;
    private InputAction moveAction;
    private InputAction attackAction;
    private float moveSpeed = 10f;
    float xRange = 11f;
    float yRange = 6f;
    int boomAmount = 10;
    float currentHealth = 1000f;
    float maxHealth = 1000f;
    float revealedTime = 1f;

    float shootInterval = 0.2f;

    [SerializeField] private GameObject boomPre;
    public AudioClip drop;

    private AudioSource sfx;

    private Tank targetTank;

    public GameObject weapon;
    public Transform weaponPos;
    public void PlaySfxBoom()
    {
        
    }
    private void Awake()
    {
        launch.Enable();
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    private void Start()
    {
        sfx = GetComponent<AudioSource>();
        sfx.playOnAwake = false;
        gameManager.UpdateBoomAmountUI(boomAmount);
    }

    private void OnEnable()
    {
        attackAction.Enable();
        moveAction.Enable();
        launch.Enable();
        attackAction.performed += AttackAction_performed;
    }

    private void OnDisable()
    {
        attackAction.Disable();
        moveAction.Disable();
        attackAction.performed -= AttackAction_performed;
    }

    private void AttackAction_performed(InputAction.CallbackContext obj)
    {
        if (gameManager.isEndGame) return;
        if(boomAmount > 0)
        {
            sfx.PlayOneShot(drop);
            boomAmount -= 1;
            gameManager.UpdateBoomAmountUI(boomAmount);
            Instantiate(boomPre, transform.position, Quaternion.Euler(90f, 0f, 0f));
            if(boomAmount <= 0) StartCoroutine(RevealBoomCoroutine());
        }
        
       
    }

    IEnumerator RevealBoomCoroutine()
    {
        yield return new WaitForSeconds(revealedTime);
        yield return gameManager.RevealBoomAmount(revealedTime);
        boomAmount = 10;
        gameManager.UpdateBoomAmountUI(boomAmount);
    }



    float zEngel = -60;
    private bool isShoot = false;   
    // Update is called once per frame
    void Update()
    {
        if (gameManager.isEndGame) return;
        if(isShoot) return;
        var moveVector  = moveAction.ReadValue<Vector2>();

        Vector3 direction = Vector3.forward * moveVector.y + Vector3.right * moveVector.x;

        //Vector2 direction = new Vector3(moveVector.x,0, moveVector.y).normalized;

        if (transform.position.x < -xRange) transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        if (transform.position.x > xRange) transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        if (transform.position.z < -yRange) transform.position = new Vector3(transform.position.x, transform.position.y, -yRange);
        if (transform.position.z > yRange) transform.position = new Vector3(transform.position.x, transform.position.y, yRange);
        if(moveVector !=  Vector2.zero)
            transform.Translate(Vector3.forward  * moveSpeed * Time.deltaTime);

        //if (moveVector.y != 0) return;
        //zEngel += moveVector.x * 100 * Time.deltaTime;
        //transform.localRotation = Quaternion.Euler(0,zEngel, 0);
        
        if(moveVector.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(moveVector.x , moveVector.y ) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0,targetAngle,0);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.deltaTime);
        }
        
        if (currentHealth <= 0)
        {
            gameManager.PopupGameOver();
        }

        if (launch.WasPressedThisFrame())
        {
            isShoot = true;
            GameObject newWeapon = Instantiate(weapon,weaponPos.position,transform.rotation);
           // Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 20,ForceMode.Impulse);           
            StartCoroutine(LaunchCurve(newWeapon.GetComponent<Weapon>()));

        }
        Vector4 a = new Vector4 (0,0,0,0);
    }

    IEnumerator LaunchCurve(Weapon w)
    {
        yield return new WaitForSeconds(0.3f);
        isShoot = false;
        targetTank = FindFirstObjectByType<Tank>();
        if (targetTank != null) 
            w.SetTarget(targetTank.transform);
    }



    public void OnAttack()
    {
        currentHealth -= Random.Range(10, 100);
        gameManager.UpdateHealthSlider(currentHealth/maxHealth);
       
    }
}
