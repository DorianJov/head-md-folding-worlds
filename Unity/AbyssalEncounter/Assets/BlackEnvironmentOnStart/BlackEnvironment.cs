using UnityEngine;

public class BlackEnvironment : MonoBehaviour
{
    [SerializeField] private Flock watchedFlock;
    private bool fadedOut;  // we start with a black screen, and fade it out after some shrimps
    private bool fadedBackIn;   // fade the black screen at the end for the title
    
    void Update()
    {
        if (!fadedOut && watchedFlock.howManyAreFollowing >= 5)
        {
            fadedOut = true;
            GetComponent<Animator>().SetTrigger("fadeout");
        }

        if (!fadedBackIn && watchedFlock.titleScreenIsOn)
        {
            fadedBackIn = true;
            GetComponent<Animator>().SetTrigger("fadein");
        }
    }
}
