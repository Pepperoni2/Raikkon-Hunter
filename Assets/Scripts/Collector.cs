using UnityEngine;
using UnityEngine.Events;

public class Collector : MonoBehaviour
{
    [SerializeField] private string collectedText;
    public UnityEvent<string> OnCollected;
    private int collected;

    void Awake()
    {
        PlayerDetectAndCollect.onCollected += Collect;
    }

    public void Collect()
    {
        collected++;
        OnCollected.Invoke($"{collectedText} {collected}");
        Debug.Log($"Collected total: {collected}");
    }
}
