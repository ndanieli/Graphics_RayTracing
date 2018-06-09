using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorManagerScript : MonoBehaviour {

    public Color grey;
    public Color blue;
    public Color red;
    public Color green;
    public Color greenish_Blue;
    public Color blueish_red;

    public void ChangeButtonColor(Button button)
    {
        StartCoroutine(waitAndChangeColor(button));
    }

    IEnumerator waitAndChangeColor(Button button)
    {
        yield return new WaitForSeconds(1f);

        ColorBlock cb = button.colors;

        string buttonName = button.name;

        string buttonRow = buttonName.Substring(1, 1);
        string buttonCol = buttonName.Substring(3);

        int row = int.Parse(buttonRow);
        int col = int.Parse(buttonCol);

        int index = 0;

        if (col == 5 && (row == 1 || row == 2))
        {
            index = 1;
        }

        if (col == 7 && (row == 2 || row == 3))
        {
            index = 2;
        }

        if (col == 8 && row != 0)
        {
            index = 3;
        }

        if (col == 5 && row == 3)
        {
            index = 13;
        }

        if (col == 6 && (row == 2 || row == 3))
        {
            index = 21;
        }

        switch (index)
        {
            case 0:
                cb.normalColor = grey;
                break;

            case 1:
                cb.normalColor = blue;
                break;

            case 2:
                cb.normalColor = red;
                break;

            case 3:
                cb.normalColor = green;
                break;

            case 13:
                cb.normalColor = greenish_Blue;
                break;

            case 21:
                cb.normalColor = blueish_red;
                break;

            default:
                break;
        }

        cb.highlightedColor = cb.normalColor;
        button.colors = cb;
    }
}
