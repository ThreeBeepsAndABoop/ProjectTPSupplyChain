using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.left * 5 * Time.deltaTime);
    }
}
