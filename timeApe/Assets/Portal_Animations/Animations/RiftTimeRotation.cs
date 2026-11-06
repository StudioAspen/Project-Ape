using UnityEngine;

public class RiftTimeRotation : MonoBehaviour
{
    [Header("Skybox Rotation")]
    [SerializeField] private float rotationSpeed = 5f;

    private Material skyboxMaterial;
    private float rotation;

    private void Start()
    {
        skyboxMaterial = RenderSettings.skybox;

        if (skyboxMaterial == null ||
            !skyboxMaterial.HasProperty("_Rotation"))
        {
            Debug.LogError(
                "The current skybox material does not support rotation."
            );

            enabled = false;
            return;
        }

        rotation = skyboxMaterial.GetFloat("_Rotation");
    }

    private void Update()
    {
        // Rotate the skybox clockwise.
        rotation += rotationSpeed * Time.deltaTime;

        if (rotation >= 360f)
            rotation -= 360f;

        skyboxMaterial.SetFloat("_Rotation", rotation);
    }
}

