using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public Vector3 rotateOffet;
    public float smoothTime = 0.3f;
    Vector3 velocity = Vector3.zero;

    private void Start()
    { 
      transform.rotation = Quaternion.identity;
    }
    private void LateUpdate()
    {
       if(player == null) return;
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        //print($"{player.rotation.eulerAngles.y}||  {player.localRotation.eulerAngles.y}");
        transform.localRotation = Quaternion.Euler(new Vector3(rotateOffet.x, player.localRotation.eulerAngles.y, rotateOffet.z));
        transform.LookAt(transform.position);
    }
}
