using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(AudioSource))]
public class Aircraft : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private InputAction moveAction;
    private InputAction attackAction;
    private float moveSpeed = 10f;
    float xRange = 11f;
    int boomAmount = 10;
    float currentHealth = 1000f;
    float maxHealth = 1000f;
    float revealedTime = 1f;

    [SerializeField] private GameObject boomPre;
    public AudioClip drop;

    private AudioSource sfx;
    public void PlaySfxBoom()
    {
        
    }
    private void Awake()
    {
        
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
            Instantiate(boomPre, transform.position, Quaternion.identity);
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


  

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isEndGame) return;
        var moveVector  = moveAction.ReadValue<Vector2>();
        if(transform.position.x < -xRange)  transform.position = new Vector3(-xRange,transform.position.y,transform.position.z);
        if(transform.position.x  > xRange) transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        transform.Translate(new Vector3(0,0, moveVector.x * moveSpeed * Time.deltaTime));
        if (currentHealth <= 0)
        {
            gameManager.PopupGameOver();
        }



    }

    public void OnAttack()
    {
        currentHealth -= Random.Range(10, 100);
        gameManager.UpdateHealthSlider(currentHealth/maxHealth);
       
    }
}
