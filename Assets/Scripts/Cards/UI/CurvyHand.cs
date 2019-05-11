using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class CurvyHand : MonoBehaviour
{
    public List<Image> items; // List that holds all my ten cards
    public Transform start;  //Location where to start adding my cards
    public Transform HandDeck; //The hand panel reference
    public float howManyAdded; // How many cards I added so far
    float gapFromOneItemToTheNextOne; //the gap I need between each card

    void Start()
    {
        howManyAdded = 0.0f;
        gapFromOneItemToTheNextOne = 1.0f;
    }

    public void FitCards()
    {

        if (items.Count == 0) //if list is null, stop function
            return;

        Image img = items[0]; //Reference to first image in my list

        img.transform.position = start.position; //relocating my card to the Start Position
        img.transform.position += new Vector3((howManyAdded * gapFromOneItemToTheNextOne), 0, 0); // Moving my card 1f to the right
        img.transform.SetParent(HandDeck); //Setting ym card parent to be the Hand Panel

        items.RemoveAt(0);
        howManyAdded++;
    }

    private float testGetPivot(int index, int number)
    {
        float mid = (float)number / 2;
        float sigmaS = (float)number;
        float p = Mathf.Exp(-Mathf.Pow(((float)index - mid), 2) / (2 * sigmaS)) / Mathf.Sqrt(2 * Mathf.PI * sigmaS);
        return p;
        //p(x) = exp(-(x - mu) ^ 2 / (2 * sigma ^ 2)) / sqrt(2 * pi * sigma ^ 2)

    }

    public int testGetRotation(int index, int number)
    {
        float x = (float)index / (float)number;
        return 0;
        
    }

    private float rot(float x, int number)
    {
        return 0f;
    }
}

