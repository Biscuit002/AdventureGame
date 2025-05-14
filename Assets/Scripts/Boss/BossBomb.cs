using UnityEngine; 
using System.Collections; 
 
public class BossBomb : MonoBehaviour 
{ 
    public CircleCollider2D triggerArea; // Assign the trigger area in the Inspector 
    public Collider2D bombCollider; // Assign the non-trigger collider 
    public string targetTag; // The tag that triggers the explosion 
    public float destructionDelay = 1f; // Time delay before explosion 
    public AudioClip explosionSound; // Sound effect for explosion 
 
    private AudioSource cameraAudioSource; 
 
    private void Awake() 
    { 
        // Find the main camera's AudioSource 
        Camera mainCamera = Camera.main; 
        if (mainCamera != null) 
        { 
            cameraAudioSource = mainCamera.GetComponent<AudioSource>(); 
            if (cameraAudioSource == null) 
            { 
                Debug.LogError("No AudioSource found on the main camera. Please add one."); 
            } 
        } 
        else 
        { 
            Debug.LogError("Main camera not found in the scene."); 
        } 
    } 
 
    void OnCollisionEnter2D(Collision2D collision) 
    { 
        // Only trigger explosion if collided object has the specified tag 
        if (collision.gameObject.CompareTag(targetTag)) 
        { 
            StartCoroutine(StartDestructionTimer()); 
        } 
    } 
 
    IEnumerator StartDestructionTimer() 
    { 
        yield return new WaitForSeconds(destructionDelay); // Wait before explosion 
 
        // Play the explosion sound from the camera's AudioSource 
        if (cameraAudioSource != null && explosionSound != null) 
        { 
            cameraAudioSource.pitch = 1f + Random.Range(-0.1f, 0.1f); // Add random pitch variation 
            cameraAudioSource.PlayOneShot(explosionSound); 
        } 
 
        // Destroy all blocks with the tag "Block" within the trigger radius 
        Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(transform.position, triggerArea.radius); 
        foreach (Collider2D obj in objectsInRadius) 
        { 
            if (obj.CompareTag("Block")) 
            { 
                Destroy(obj.gameObject); 
            } 
        } 
 
        // Destroy the bomb itself 
        Destroy(gameObject); 
    } 
 
    private void OnDrawGizmosSelected() 
    { 
        // Visualize the trigger radius in the editor 
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, triggerArea.radius); 
    } 
} 