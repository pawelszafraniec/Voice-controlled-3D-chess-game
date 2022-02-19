using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class supporting main menu slider animation
 */
public class SliderMenuManager : MonoBehaviour
{
    public GameObject PanelMenu;

    /**
     * Method setting bool parameter to slider
     */
    public void SliderMenu()
    {
        if (PanelMenu != null)
        {
            Animator animator = PanelMenu.GetComponent<Animator>(); // get animator
            if (animator != null)
            {
                bool ShowSlider = animator.GetBool("Slide"); // get bool
                animator.SetBool("Slide", !ShowSlider); // set bool
            }
        }
    }
}
