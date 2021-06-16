using UnityEngine;

public class HeadFollow : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] Transform follow;

    // Update is called once per frame
    void LateUpdate() {head.position = new Vector3(follow.position.x, head.position.y, head.position.z);}
}
