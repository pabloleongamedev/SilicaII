using UnityEngine;

public class GroundingProbe : MonoBehaviour
{
    private const string SecretValidationToken = "LIAS_READ_TEST_7391";

    private void Awake()
    {
        Debug.Log($"Grounding probe active: {SecretValidationToken}");
    }
}
