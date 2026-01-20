using UnityEngine;
using UnityEngine.InputSystem;

public class TestManager : MonoBehaviour
{
    public Transform target;
    public float speed = 50f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distant = Vector3.Distance(transform.position, target.position);
       
        if (Keyboard.current.enterKey.wasReleasedThisFrame)
        {
            print(distant);
        }
        
            Debug.DrawLine(transform.position, target.position);

            transform.RotateAround(target.position,Vector3.up, speed * Time.deltaTime);

            transform.Rotate(Vector3.up, speed * Time.deltaTime);
          //  transform.position = Vector3.Slerp(transform.position, target.position, 2f * Time.deltaTime);
        
            
        
    }
}
