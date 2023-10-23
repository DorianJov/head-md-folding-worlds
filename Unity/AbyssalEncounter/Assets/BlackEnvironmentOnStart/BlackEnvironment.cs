using UnityEngine;

public class BlackEnvironment : MonoBehaviour
{
    [SerializeField] private Flock watchedFlock;
    private bool animationStarted;
    
    void Update()
    {
        if (watchedFlock.howManyAreFollowing >= 2 && !animationStarted)
        {
            animationStarted = true;
            GetComponent<Animator>().SetTrigger("fadeout");
        }
    }
}
