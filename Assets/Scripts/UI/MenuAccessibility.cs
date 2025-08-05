using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Keep keyboard navigatable elements in structured order
public class MenuAccessibility : MonoBehaviour
{
    public List<Selectable> UIElements;
    private int currentTabIndex = -1;

    private void Update()
    {
        //Move backwards
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
        {
            currentTabIndex--;

            if (currentTabIndex < 0)
                currentTabIndex = UIElements.Count - 1;

            SelectElement(currentTabIndex);
        }
        //Move forwards
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentTabIndex++;

            if (currentTabIndex > UIElements.Count - 1)
                currentTabIndex = 0;

            SelectElement(currentTabIndex);
        }
    }
   
    //Change element to selected mode
    private void SelectElement(int index)
    {
        UIElements[index].Select();
    }
}
