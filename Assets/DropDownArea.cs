using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownArea : MonoBehaviour
{
    public GameObject Panel;
    public void TogglePanel()
    {
        if( Panel != null) 
        {
            Panel.SetActive(!Panel.activeSelf);
        }

    }

}
