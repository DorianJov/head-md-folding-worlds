using UnityEngine;

public class BlackEnvironment : MonoBehaviour
{
    [SerializeField] private Flock watchedFlock;
    private bool animationStarted;
    
    void Update()
    {
        if (watchedFlock.howManyAreFollowing >= 5 && !animationStarted)
        {
            animationStarted = true;
            GetComponent<Animator>().SetTrigger("fadeout");
        }
    }
}
