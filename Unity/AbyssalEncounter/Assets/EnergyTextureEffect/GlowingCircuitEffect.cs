using System.Collections;
using UnityEngine;

public class GlowingCircuitEffect : MonoBehaviour
{
    [Header("Clock mask animation")]
    [SerializeField] private float animationSmoothing = 1f;
    [SerializeField] private int targetFishCount = 0;
    [SerializeField] private int maxFishCount = 60;
    
    [Header("Circle pulse animation")]
    [SerializeField] private bool addPulseAnimation = false;
    [SerializeField] private float pulsesInterval = 3f;
    [Range(0.01f, 3f)]
    [SerializeField] private float pulsesDuration = 0.5f;
    
    private Renderer thisRenderer;
    private float animatedFishCount;
    private float animationVelocity;
    
    private static readonly int FishCountProperty = Shader.PropertyToID("_FishCount");
    private static readonly int MaxFishCountProperty = Shader.PropertyToID("_MaxFishCount");
    private static readonly int CircleAnimationProperty = Shader.PropertyToID("_CircleAnimation");

    // Public Methods
    public void SetFishCount(int count)
    {
        targetFishCount = count;
    }
    
    // Private Methods
    private void Awake()
    {
        thisRenderer = GetComponent<Renderer>();
        animatedFishCount = targetFishCount;
    }

    private void OnEnable()
    {
        if (addPulseAnimation)
        {
            StartCoroutine(Pulse());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        animatedFishCount = Mathf.SmoothDamp(animatedFishCount, (float)targetFishCount, ref animationVelocity, animationSmoothing);
        thisRenderer.material.SetFloat(FishCountProperty, animatedFishCount);
        thisRenderer.material.SetFloat(MaxFishCountProperty, maxFishCount);
    }

    private IEnumerator Pulse()
    {
        var time = 0f;
        while (time < 1f)
        {
            thisRenderer.material.SetFloat(CircleAnimationProperty, time);
            time += Time.deltaTime / pulsesDuration;
            yield return null;
        }
        thisRenderer.material.SetFloat(CircleAnimationProperty, 1);

        // wait for interval in seconds
        yield return new WaitForSeconds(pulsesInterval);
        
        // restart coroutine
        StartCoroutine(Pulse());
    }
}
