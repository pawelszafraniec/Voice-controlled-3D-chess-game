using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderMenuManager : MonoBehaviour
{
    public GameObject PanelMenu;

    public void SliderMenu()
    {
        if (PanelMenu != null)
        {
            Animator animator = PanelMenu.GetComponent<Animator>();
            if (animator != null)
            {
                bool ShowSlider = animator.GetBool("Slide");
                animator.SetBool("Slide", !ShowSlider);
            }
        }
    }
}
