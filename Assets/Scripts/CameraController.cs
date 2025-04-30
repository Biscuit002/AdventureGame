using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerStateMachine playerStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateMachine = FindObjectOfType<PlayerStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        //lerp camera position to player position
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerStateMachine.transform.position.x, playerStateMachine.transform.position.y + 2, playerStateMachine.transform.position.z - 5), Time.deltaTime * 5);
    }
}
