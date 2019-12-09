using UnityEngine;
using UnityEngine.Events;

public class PedestrianController : MonoBehaviour
{
    public UnityEvent OnDestroy;
    public bool stop = false;
    public bool reversed = false;

    [SerializeField]
    private float speed = 3f;
    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool("reversed", reversed);
            animator.SetBool("stopped", stop);
        }

        Translate();
    }

    private void Translate()
    {
        if (!stop)
        {
            transform.Translate(0f, 0f, speed * Time.deltaTime);
        }

        if (reversed)
        {
            transform.Translate(0f, 0f, speed * Time.deltaTime * -1);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
        OnDestroy.Invoke();
        OnDestroy.RemoveAllListeners();
    }
}
