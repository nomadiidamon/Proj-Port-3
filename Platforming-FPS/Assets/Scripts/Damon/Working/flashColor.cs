using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class flashColor : MonoBehaviour
{
    [Header("-----Flashing Object Properties-----")]
    [SerializeField] float flashSpeed;
    [SerializeField] float maxIntensity;
    [SerializeField] float minIntensity;
    [SerializeField] float baseIntensity;
    [SerializeField] float lightRange;
    [SerializeField] float maxRange;
    [SerializeField] float minRange;
    float currentRange;
    [SerializeField] bool flashRange;
    [SerializeField] Light myLight;
    [SerializeField] enum numberOfColors { one, two, three, four, five };
    [SerializeField] numberOfColors chosenNumber;

    [Header("-----Colors-----")]
    [SerializeField] firstColor first;
    [SerializeField] enum firstColor { empty, black, blue, clear, cyan, gray, green, magenta, red, white, yellow, custom1, custom2, custom3, custom4, custom5 };
    Color color1;

    [Header("-----Currently only one color supported-----")]
    [SerializeField] secondColor second;
    [SerializeField] enum secondColor { empty, black, blue, clear, cyan, gray, green, magenta, red, white, yellow, custom1, custom2, custom3, custom4, custom5 };
    Color color2;

    [SerializeField] thirdColor third;
    [SerializeField] enum thirdColor { empty, black, blue, clear, cyan, gray, green, magenta, red, white, yellow, custom1, custom2, custom3, custom4, custom5 };
    Color color3;

    [SerializeField] fourthColor fourth;
    [SerializeField] enum fourthColor { empty, black, blue, clear, cyan, gray, green, magenta, red, white, yellow, custom1, custom2, custom3, custom4, custom5 };
    Color color4;

    [SerializeField] fifthColor fifth;
    enum fifthColor { empty, black, blue, clear, cyan, gray, green, magenta, red, white, yellow, custom1, custom2, custom3, custom4, custom5 };
    Color color5;

    [Header("-----Custom Color 1-----")]
    [SerializeField] float Custom1_R;
    [SerializeField] float Custom1_G;
    [SerializeField] float Custom1_B;
    [SerializeField] float Custom1_A;
    [SerializeField] Color customColor1;

    [Header("-----Custom Color 2-----")]
    [SerializeField] float Custom2_R;
    [SerializeField] float Custom2_G;
    [SerializeField] float Custom2_B;
    [SerializeField] float Custom2_A;
    [SerializeField] Color customColor2;

    [Header("-----Custom Color 3-----")]
    [SerializeField] float Custom3_R;
    [SerializeField] float Custom3_G;
    [SerializeField] float Custom3_B;
    [SerializeField] float Custom3_A;
    [SerializeField] Color customColor3;

    [Header("-----Custom Color 4-----")]
    [SerializeField] float Custom4_R;
    [SerializeField] float Custom4_G;
    [SerializeField] float Custom4_B;
    [SerializeField] float Custom4_A;
    [SerializeField] Color customColor4;

    [Header("-----Custom Color 3-----")]
    [SerializeField] float Custom5_R;
    [SerializeField] float Custom5_G;
    [SerializeField] float Custom5_B;
    [SerializeField] float Custom5_A;
    [SerializeField] Color customColor5;

    private Renderer rend;
    private Color originalColor;
    private float time;

    void Start()
    {
        AssignCustomColors();
        AssignLightValues();

        switch (chosenNumber)
        {
            case numberOfColors.one:
                {
                    OneColor();
                    break;
                }
            case numberOfColors.two:
                {
                    TwoColors();
                    break;
                }
            case numberOfColors.three:
                {
                    ThreeColors();
                    break;
                }
            case numberOfColors.four:
                {
                    FourColors();
                    break;
                }
            case numberOfColors.five:
                {
                    FiveColors();
                    break;
                }
            default:
                {
                    break;
                }
        }
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        time = 0.0f;
    }

    void Update()
    {
        //time += Time.deltaTime * flashSpeed;
        //float lerpFactor = Mathf.PingPong(time, 3f);

        //Color modelColor = Color.Lerp(color1, color2, lerpFactor);
        //rend.material.color = modelColor;
        //Color lightColor = Color.Lerp(color1, color2, lerpFactor);
        //myLight.color = lightColor;

        switch (chosenNumber)
        {
            case numberOfColors.one:
                {
                    flashOne();
                    break;
                }
            case numberOfColors.two:
                {
                    flashTwo();
                    break;
                }
            case numberOfColors.three:
                {
                    flashThree();
                    break;
                }
            case numberOfColors.four:
                {
                    flashFour();
                    break;
                }
            case numberOfColors.five:
                {
                    flashFive();
                    break;
                }
            default:
                {
                    break;
                }



        }

    }

    public void AssignCustomColors()
    {
        if (Custom1_R != 0 && Custom1_G != 0 && Custom1_B != 0 && Custom1_A != 0)
        {
            customColor1 = new Color(Custom1_R, Custom1_G, Custom1_B, Custom1_A);
        }
        if (Custom2_R != 0 && Custom2_G != 0 && Custom2_B != 0 && Custom2_A != 0)
        {
            customColor2 = new Color(Custom2_R, Custom2_G, Custom2_B, Custom2_A);
        }
        if (Custom3_R != 0 && Custom3_G != 0 && Custom3_B != 0 && Custom3_A != 0)
        {
            customColor3 = new Color(Custom3_R, Custom3_G, Custom3_B, Custom3_A);
        }
        if (Custom4_R != 0 && Custom4_G != 0 && Custom4_B != 0 && Custom4_A != 0)
        {
            customColor4 = new Color(Custom4_R, Custom4_G, Custom4_B, Custom4_A);
        }
        if (Custom5_R != 0 && Custom5_G != 0 && Custom5_B != 0 && Custom5_A != 0)
        {
            customColor5 = new Color(Custom5_R, Custom5_G, Custom5_B, Custom5_A);
        }
    }

    public void AssignLightValues()
    {

        myLight.intensity = baseIntensity;
        myLight.range = lightRange;
    }

    public void OneColor()
    {
        if (first == firstColor.custom1)
        {
            color1 = customColor1;
        }
        else if (first == firstColor.black)
        {
            color1 = Color.black;
        }
        else if (first == firstColor.blue)
        {
            color1 = Color.blue;
        }
        else if (first == firstColor.clear)
        {
            color1 = Color.clear;
        }
        else if (first == firstColor.cyan)
        {
            color1 = Color.cyan;
        }
        else if (first == firstColor.gray)
        {
            color1 = Color.gray;
        }
        else if (first == firstColor.magenta)
        {
            color1 = Color.magenta;
        }
        else if (first == firstColor.red)
        {
            color1 = Color.red;
        }
        else if (first == firstColor.white)
        {
            color1 = Color.white;
        }
        else if (first == firstColor.yellow)
        {
            color1 = Color.yellow;
        }
        else if (first == firstColor.custom2)
        {
            color1 = customColor2;
        }
        else if (first == firstColor.custom3)
        {
            color1 = customColor3;
        }
        else if (first == firstColor.custom4)
        {
            color1 = customColor4;
        }
        else if (first == firstColor.custom5)
        {
            color1 = customColor5;
        }

    }

    public void flashOne()
    {
        time += Time.deltaTime * flashSpeed;
        float colorLerp = Mathf.PingPong(time, flashSpeed);
        float lightLerp = Mathf.PingPong(time, maxIntensity);
        float rangeLerp = Mathf.PingPong(time, maxRange);

        Color modelColor = Color.Lerp(color1, originalColor, colorLerp);
        rend.material.color = modelColor;
        Color lightColor = Color.Lerp(color1, modelColor, lightLerp);


        myLight.color = lightColor;
        myLight.intensity = lightLerp;
        rend.sharedMaterial.SetColor("_EmissionColor", lightColor * lightLerp);
        if (flashRange)
        {
            currentRange = Mathf.Lerp(minRange, maxRange, rangeLerp);
            myLight.range = currentRange;
        }

    }

    public void TwoColors()
    {
        /// first color
        if (first == firstColor.custom1)
        {
            color1 = customColor1;
        }
        else if (first == firstColor.black)
        {
            color1 = Color.black;
        }
        else if (first == firstColor.blue)
        {
            color1 = Color.blue;
        }
        else if (first == firstColor.clear)
        {
            color1 = Color.clear;
        }
        else if (first == firstColor.cyan)
        {
            color1 = Color.cyan;
        }
        else if (first == firstColor.gray)
        {
            color1 = Color.gray;
        }
        else if (first == firstColor.magenta)
        {
            color1 = Color.magenta;
        }
        else if (first == firstColor.red)
        {
            color1 = Color.red;
        }
        else if (first == firstColor.white)
        {
            color1 = Color.white;
        }
        else if (first == firstColor.yellow)
        {
            color1 = Color.yellow;
        }
        else if (first == firstColor.custom2)
        {
            color1 = customColor2;
        }
        else if (first == firstColor.custom3)
        {
            color1 = customColor3;
        }
        else if (first == firstColor.custom4)
        {
            color1 = customColor4;
        }
        else if (first == firstColor.custom5)
        {
            color1 = customColor5;
        }

        /// second color
        if (second == secondColor.custom2)
        {
            color2 = customColor2;
        }
        else if (second == secondColor.black)
        {
            color2 = Color.black;
        }
        else if (second == secondColor.blue)
        {
            color2 = Color.blue;
        }
        else if (second == secondColor.clear)
        {
            color2 = Color.clear;
        }
        else if (second == secondColor.cyan)
        {
            color2 = Color.cyan;
        }
        else if (second == secondColor.gray)
        {
            color2 = Color.gray;
        }
        else if (second == secondColor.magenta)
        {
            color2 = Color.magenta;
        }
        else if (second == secondColor.red)
        {
            color2 = Color.red;
        }
        else if (second == secondColor.white)
        {
            color2 = Color.white;
        }
        else if (second == secondColor.yellow)
        {
            color2 = Color.yellow;
        }
        else if (second == secondColor.custom1)
        {
            color2 = customColor1;
        }
        else if (second == secondColor.custom3)
        {
            color2 = customColor3;
        }
        else if (second == secondColor.custom4)
        {
            color2 = customColor4;
        }
        else if (second == secondColor.custom5)
        {
            color2 = customColor5;
        }
    }

    public void flashTwo()
    {

    }

    public void ThreeColors()
    {
        /// first color
        if (first == firstColor.custom1)
        {
            color1 = customColor1;
        }
        else if (first == firstColor.black)
        {
            color1 = Color.black;
        }
        else if (first == firstColor.blue)
        {
            color1 = Color.blue;
        }
        else if (first == firstColor.clear)
        {
            color1 = Color.clear;
        }
        else if (first == firstColor.cyan)
        {
            color1 = Color.cyan;
        }
        else if (first == firstColor.gray)
        {
            color1 = Color.gray;
        }
        else if (first == firstColor.magenta)
        {
            color1 = Color.magenta;
        }
        else if (first == firstColor.red)
        {
            color1 = Color.red;
        }
        else if (first == firstColor.white)
        {
            color1 = Color.white;
        }
        else if (first == firstColor.yellow)
        {
            color1 = Color.yellow;
        }
        else if (first == firstColor.custom2)
        {
            color1 = customColor2;
        }
        else if (first == firstColor.custom3)
        {
            color1 = customColor3;
        }
        else if (first == firstColor.custom4)
        {
            color1 = customColor4;
        }
        else if (first == firstColor.custom5)
        {
            color1 = customColor5;
        }

        /// second color
        if (second == secondColor.custom2)
        {
            color2 = customColor2;
        }
        else if (second == secondColor.black)
        {
            color2 = Color.black;
        }
        else if (second == secondColor.blue)
        {
            color2 = Color.blue;
        }
        else if (second == secondColor.clear)
        {
            color2 = Color.clear;
        }
        else if (second == secondColor.cyan)
        {
            color2 = Color.cyan;
        }
        else if (second == secondColor.gray)
        {
            color2 = Color.gray;
        }
        else if (second == secondColor.magenta)
        {
            color2 = Color.magenta;
        }
        else if (second == secondColor.red)
        {
            color2 = Color.red;
        }
        else if (second == secondColor.white)
        {
            color2 = Color.white;
        }
        else if (second == secondColor.yellow)
        {
            color2 = Color.yellow;
        }
        else if (second == secondColor.custom1)
        {
            color2 = customColor1;
        }
        else if (second == secondColor.custom3)
        {
            color2 = customColor3;
        }
        else if (second == secondColor.custom4)
        {
            color2 = customColor4;
        }
        else if (second == secondColor.custom5)
        {
            color2 = customColor5;
        }

        /// third color
        if (third == thirdColor.custom3)
        {
            color3 = customColor3;
        }
        else if (third == thirdColor.black)
        {
            color3 = Color.black;
        }
        else if (third == thirdColor.blue)
        {
            color3 = Color.blue;
        }
        else if (third == thirdColor.clear)
        {
            color3 = Color.clear;
        }
        else if (third == thirdColor.cyan)
        {
            color3 = Color.cyan;
        }
        else if (third == thirdColor.gray)
        {
            color3 = Color.gray;
        }
        else if (third == thirdColor.magenta)
        {
            color3 = Color.magenta;
        }
        else if (third == thirdColor.red)
        {
            color3 = Color.red;
        }
        else if (third == thirdColor.white)
        {
            color3 = Color.white;
        }
        else if (third == thirdColor.yellow)
        {
            color3 = Color.yellow;
        }
        else if (third == thirdColor.custom1)
        {
            color3 = customColor1;
        }
        else if (third == thirdColor.custom2)
        {
            color3 = customColor2;
        }
        else if (third == thirdColor.custom4)
        {
            color3 = customColor4;
        }
        else if (third == thirdColor.custom5)
        {
            color3 = customColor5;
        }
    }

    public void flashThree()
    {

    }

    public void FourColors()
    {
        /// first color
        if (first == firstColor.custom1)
        {
            color1 = customColor1;
        }
        else if (first == firstColor.black)
        {
            color1 = Color.black;
        }
        else if (first == firstColor.blue)
        {
            color1 = Color.blue;
        }
        else if (first == firstColor.clear)
        {
            color1 = Color.clear;
        }
        else if (first == firstColor.cyan)
        {
            color1 = Color.cyan;
        }
        else if (first == firstColor.gray)
        {
            color1 = Color.gray;
        }
        else if (first == firstColor.magenta)
        {
            color1 = Color.magenta;
        }
        else if (first == firstColor.red)
        {
            color1 = Color.red;
        }
        else if (first == firstColor.white)
        {
            color1 = Color.white;
        }
        else if (first == firstColor.yellow)
        {
            color1 = Color.yellow;
        }
        else if (first == firstColor.custom2)
        {
            color1 = customColor2;
        }
        else if (first == firstColor.custom3)
        {
            color1 = customColor3;
        }
        else if (first == firstColor.custom4)
        {
            color1 = customColor4;
        }
        else if (first == firstColor.custom5)
        {
            color1 = customColor5;
        }

        /// second color
        if (second == secondColor.custom2)
        {
            color2 = customColor2;
        }
        else if (second == secondColor.black)
        {
            color2 = Color.black;
        }
        else if (second == secondColor.blue)
        {
            color2 = Color.blue;
        }
        else if (second == secondColor.clear)
        {
            color2 = Color.clear;
        }
        else if (second == secondColor.cyan)
        {
            color2 = Color.cyan;
        }
        else if (second == secondColor.gray)
        {
            color2 = Color.gray;
        }
        else if (second == secondColor.magenta)
        {
            color2 = Color.magenta;
        }
        else if (second == secondColor.red)
        {
            color2 = Color.red;
        }
        else if (second == secondColor.white)
        {
            color2 = Color.white;
        }
        else if (second == secondColor.yellow)
        {
            color2 = Color.yellow;
        }
        else if (second == secondColor.custom1)
        {
            color2 = customColor1;
        }
        else if (second == secondColor.custom3)
        {
            color2 = customColor3;
        }
        else if (second == secondColor.custom4)
        {
            color2 = customColor4;
        }
        else if (second == secondColor.custom5)
        {
            color2 = customColor5;
        }

        /// third color
        if (third == thirdColor.custom3)
        {
            color3 = customColor3;
        }
        else if (third == thirdColor.black)
        {
            color3 = Color.black;
        }
        else if (third == thirdColor.blue)
        {
            color3 = Color.blue;
        }
        else if (third == thirdColor.clear)
        {
            color3 = Color.clear;
        }
        else if (third == thirdColor.cyan)
        {
            color3 = Color.cyan;
        }
        else if (third == thirdColor.gray)
        {
            color3 = Color.gray;
        }
        else if (third == thirdColor.magenta)
        {
            color3 = Color.magenta;
        }
        else if (third == thirdColor.red)
        {
            color3 = Color.red;
        }
        else if (third == thirdColor.white)
        {
            color3 = Color.white;
        }
        else if (third == thirdColor.yellow)
        {
            color3 = Color.yellow;
        }
        else if (third == thirdColor.custom1)
        {
            color3 = customColor1;
        }
        else if (third == thirdColor.custom2)
        {
            color3 = customColor2;
        }
        else if (third == thirdColor.custom4)
        {
            color3 = customColor4;
        }
        else if (third == thirdColor.custom5)
        {
            color3 = customColor5;
        }


        /// fourth color
        if (fourth == fourthColor.custom4)
        {
            color4 = customColor4;
        }
        else if (fourth == fourthColor.black)
        {
            color4 = Color.black;
        }
        else if (fourth == fourthColor.blue)
        {
            color4 = Color.blue;
        }
        else if (fourth == fourthColor.clear)
        {
            color4 = Color.clear;
        }
        else if (fourth == fourthColor.cyan)
        {
            color4 = Color.cyan;
        }
        else if (fourth == fourthColor.gray)
        {
            color4 = Color.gray;
        }
        else if (fourth == fourthColor.magenta)
        {
            color4 = Color.magenta;
        }
        else if (fourth == fourthColor.red)
        {
            color4 = Color.red;
        }
        else if (fourth == fourthColor.white)
        {
            color4 = Color.white;
        }
        else if (fourth == fourthColor.yellow)
        {
            color4 = Color.yellow;
        }
        else if (fourth == fourthColor.custom1)
        {
            color4 = customColor1;
        }
        else if (fourth == fourthColor.custom2)
        {
            color4 = customColor2;
        }
        else if (fourth == fourthColor.custom3)
        {
            color4 = customColor3;
        }
        else if (fourth == fourthColor.custom5)
        {
            color4 = customColor5;
        }
    }

    public void flashFour()
    {

    }

    public void FiveColors()
    {
        /// first color
        if (first == firstColor.custom1)
        {
            color1 = customColor1;
        }
        else if (first == firstColor.black)
        {
            color1 = Color.black;
        }
        else if (first == firstColor.blue)
        {
            color1 = Color.blue;
        }
        else if (first == firstColor.clear)
        {
            color1 = Color.clear;
        }
        else if (first == firstColor.cyan)
        {
            color1 = Color.cyan;
        }
        else if (first == firstColor.gray)
        {
            color1 = Color.gray;
        }
        else if (first == firstColor.magenta)
        {
            color1 = Color.magenta;
        }
        else if (first == firstColor.red)
        {
            color1 = Color.red;
        }
        else if (first == firstColor.white)
        {
            color1 = Color.white;
        }
        else if (first == firstColor.yellow)
        {
            color1 = Color.yellow;
        }
        else if (first == firstColor.custom2)
        {
            color1 = customColor2;
        }
        else if (first == firstColor.custom3)
        {
            color1 = customColor3;
        }
        else if (first == firstColor.custom4)
        {
            color1 = customColor4;
        }
        else if (first == firstColor.custom5)
        {
            color1 = customColor5;
        }

        /// second color
        if (second == secondColor.custom2)
        {
            color2 = customColor2;
        }
        else if (second == secondColor.black)
        {
            color2 = Color.black;
        }
        else if (second == secondColor.blue)
        {
            color2 = Color.blue;
        }
        else if (second == secondColor.clear)
        {
            color2 = Color.clear;
        }
        else if (second == secondColor.cyan)
        {
            color2 = Color.cyan;
        }
        else if (second == secondColor.gray)
        {
            color2 = Color.gray;
        }
        else if (second == secondColor.magenta)
        {
            color2 = Color.magenta;
        }
        else if (second == secondColor.red)
        {
            color2 = Color.red;
        }
        else if (second == secondColor.white)
        {
            color2 = Color.white;
        }
        else if (second == secondColor.yellow)
        {
            color2 = Color.yellow;
        }
        else if (second == secondColor.custom1)
        {
            color2 = customColor1;
        }
        else if (second == secondColor.custom3)
        {
            color2 = customColor3;
        }
        else if (second == secondColor.custom4)
        {
            color2 = customColor4;
        }
        else if (second == secondColor.custom5)
        {
            color2 = customColor5;
        }

        /// third color
        if (third == thirdColor.custom3)
        {
            color3 = customColor3;
        }
        else if (third == thirdColor.black)
        {
            color3 = Color.black;
        }
        else if (third == thirdColor.blue)
        {
            color3 = Color.blue;
        }
        else if (third == thirdColor.clear)
        {
            color3 = Color.clear;
        }
        else if (third == thirdColor.cyan)
        {
            color3 = Color.cyan;
        }
        else if (third == thirdColor.gray)
        {
            color3 = Color.gray;
        }
        else if (third == thirdColor.magenta)
        {
            color3 = Color.magenta;
        }
        else if (third == thirdColor.red)
        {
            color3 = Color.red;
        }
        else if (third == thirdColor.white)
        {
            color3 = Color.white;
        }
        else if (third == thirdColor.yellow)
        {
            color3 = Color.yellow;
        }
        else if (third == thirdColor.custom1)
        {
            color3 = customColor1;
        }
        else if (third == thirdColor.custom2)
        {
            color3 = customColor2;
        }
        else if (third == thirdColor.custom4)
        {
            color3 = customColor4;
        }
        else if (third == thirdColor.custom5)
        {
            color3 = customColor5;
        }


        /// fourth color
        if (fourth == fourthColor.custom4)
        {
            color4 = customColor4;
        }
        else if (fourth == fourthColor.black)
        {
            color4 = Color.black;
        }
        else if (fourth == fourthColor.blue)
        {
            color4 = Color.blue;
        }
        else if (fourth == fourthColor.clear)
        {
            color4 = Color.clear;
        }
        else if (fourth == fourthColor.cyan)
        {
            color4 = Color.cyan;
        }
        else if (fourth == fourthColor.gray)
        {
            color4 = Color.gray;
        }
        else if (fourth == fourthColor.magenta)
        {
            color4 = Color.magenta;
        }
        else if (fourth == fourthColor.red)
        {
            color4 = Color.red;
        }
        else if (fourth == fourthColor.white)
        {
            color4 = Color.white;
        }
        else if (fourth == fourthColor.yellow)
        {
            color4 = Color.yellow;
        }
        else if (fourth == fourthColor.custom1)
        {
            color4 = customColor1;
        }
        else if (fourth == fourthColor.custom2)
        {
            color4 = customColor2;
        }
        else if (fourth == fourthColor.custom3)
        {
            color4 = customColor3;
        }
        else if (fourth == fourthColor.custom5)
        {
            color4 = customColor5;
        }


        /// fifth color
        if (fifth == fifthColor.custom5)
        {
            color5 = customColor5;
        }
        else if (fifth == fifthColor.black)
        {
            color5 = Color.black;
        }
        else if (fifth == fifthColor.blue)
        {
            color5 = Color.blue;
        }
        else if (fifth == fifthColor.clear)
        {
            color5 = Color.clear;
        }
        else if (fifth == fifthColor.cyan)
        {
            color5 = Color.cyan;
        }
        else if (fifth == fifthColor.gray)
        {
            color5 = Color.gray;
        }
        else if (fifth == fifthColor.magenta)
        {
            color5 = Color.magenta;
        }
        else if (fifth == fifthColor.red)
        {
            color5 = Color.red;
        }
        else if (fifth == fifthColor.white)
        {
            color5 = Color.white;
        }
        else if (fifth == fifthColor.yellow)
        {
            color5 = Color.yellow;
        }
        else if (fifth == fifthColor.custom1)
        {
            color5 = customColor1;
        }
        else if (fifth == fifthColor.custom2)
        {
            color5 = customColor2;
        }
        else if (fifth == fifthColor.custom3)
        {
            color5 = customColor3;
        }
        else if (fifth == fifthColor.custom4)
        {
            color5 = customColor4;
        }
    }

    public void flashFive()
    {

    }

}
