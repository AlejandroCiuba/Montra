using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private float speed = .5f;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        this.transform.position = player.transform.position + offset;
    }

    void LateUpdate() 
    {
        if(followX) SetPositionX();
        if(followY) SetPositionY();
    }

    private void SetPositionX()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, 
        new Vector3(player.transform.position.x + offset.x, this.transform.position.y, this.transform.position.z), 
        speed * Time.deltaTime);
    }

    private void SetPositionY()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, 
        new Vector3(this.transform.position.x, player.transform.position.y + offset.y, this.transform.position.z), 
        speed * Time.deltaTime);
    }
}
