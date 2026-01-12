using System.Collections;
using System.Data;
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

    [SerializeField] private GameObject boomPre;
    public AudioClip drop;

    private AudioSource sfx;

    private Tank targetTank;

    public GameObject weapon;
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
    // Update is called once per frame
    void Update()
    {
        if (gameManager.isEndGame) return;
        var moveVector  = moveAction.ReadValue<Vector2>();
        if(transform.position.x < -xRange)  transform.position = new Vector3(-xRange,transform.position.y,transform.position.z);
        if(transform.position.x  > xRange) transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        if (transform.position.z < -yRange) transform.position = new Vector3(transform.position.x, transform.position.y, -yRange);
        if (transform.position.z > yRange) transform.position = new Vector3(transform.position.x, transform.position.y, yRange);

        transform.Translate(new Vector3(0,0,  moveVector.y * moveSpeed * Time.deltaTime));

        if (moveVector.y != 0) return;
        zEngel += moveVector.x * 100 * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0,zEngel, 0);
        if (currentHealth <= 0)
        {
            gameManager.PopupGameOver();
        }

        if (launch.WasPressedThisFrame())
        {
            GameObject newWeapon = Instantiate(weapon,transform.position,transform.rotation);
           // Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 20,ForceMode.Impulse);

            

            StartCoroutine(LaunchCurve(newWeapon.GetComponent<Weapon>()));

        }

    }

    IEnumerator LaunchCurve(Weapon w)
    {
        yield return new WaitForSeconds(0.3f);
        targetTank = FindFirstObjectByType<Tank>();
        w.SetTarget(targetTank.transform);


    }



    public void OnAttack()
    {
        currentHealth -= Random.Range(10, 100);
        gameManager.UpdateHealthSlider(currentHealth/maxHealth);
       
    }
}
