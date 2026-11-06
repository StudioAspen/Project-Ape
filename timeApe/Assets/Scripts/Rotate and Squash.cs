
using UnityEngine;

public class RotateandSquash : MonoBehaviour
{
    [Header("GameObjects to Disappear")]
    public GameObject[] objectsToHide;

    [Header("7 GameObjects with Animations")]
    public GameObject[] animatedObjects = new GameObject[7];

    private bool hasSwitched = false;

    void Update()
    {
        // Press K to switch the GameObjects
        if (Input.GetKeyDown(KeyCode.K) && !hasSwitched)
        {
            SwitchObjects();
        }
    }

    void SwitchObjects()
    {
        // Permanently turn off the original GameObjects
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // Activate all 7 animated GameObjects
        foreach (GameObject obj in animatedObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);

                // Restart the attached Animator animation
                Animator animator = obj.GetComponent<Animator>();

                if (animator != null)
                {
                    animator.enabled = true;
                    animator.Rebind();
                    animator.Update(0f);
                }
            }
        }

        // Prevent the switch from happening again
        hasSwitched = true;
    }
}
