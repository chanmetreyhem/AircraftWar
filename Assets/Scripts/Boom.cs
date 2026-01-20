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

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.CompareTag("Land") || other.gameObject.CompareTag("Enemy"))
        {
            audioSource.PlayOneShot(clip);
            Instantiate(m_Effect, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            Destroy(gameObject, 0.05f);
        }
    }
  
}
