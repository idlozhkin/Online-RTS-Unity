using UnityEngine;

public class Fireball : MonoBehaviour
{
    [HideInInspector] public GameObject target = null;
    [SerializeField] private float speed;
    void Update()
    {
        if (target != null)
        {
            Debug.Log(target.name);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            if (Mathf.Abs((transform.position - target.transform.position).sqrMagnitude) < 1)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
