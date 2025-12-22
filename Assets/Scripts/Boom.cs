using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Boom : MonoBehaviour
{
    [SerializeField] private GameObject m_Effect;
    [SerializeField] private AudioClip clip;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource  = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Land") || collision.gameObject.CompareTag("Enemy")) {
            audioSource.PlayOneShot(clip);
            Instantiate(m_Effect,transform.position,Quaternion.identity);
            Destroy(gameObject,0.05f);
        }
    }
}
