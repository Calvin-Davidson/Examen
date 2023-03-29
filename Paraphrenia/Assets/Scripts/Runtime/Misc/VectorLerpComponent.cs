using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorLerpComponent : MasterVectorLerpComponent
{
    enum LerpType { Linear, EaseInSine, EaseOutSine, EaseInOutSine, EaseInCubic, EaseOutCubic, EaseInOutCubic, EaseInQuint, EaseOutQuint, EaseInOutQuint, EaseInCirc, EaseOutCirc, EaseInOutCirc, EaseInElastic, EaseOutElastic, EaseInOutElastic, EaseInQuad, EaseOutQuad, EaseInOutQuad, EaseInQuart, EaseOutQuart, EaseInOutQuart, EaseInExpo, EaseOutExpo, EaseInOutExpo, EaseInBack, EaseOutBack, EaseInOutBack, EaseInBounce, EaseOutBounce, EaseInOutBounce };
    [SerializeField] private LerpType _lerpType;

    protected override float CalculateEaseStep(float currentPercent)
    {
        switch(_lerpType)
        {
            //Sine
            case LerpType.EaseInSine:
                return EaseInSine(currentPercent);
            case LerpType.EaseOutSine:
                return EaseOutSine(currentPercent);
            case LerpType.EaseInOutSine:
                return EaseInOutSine(currentPercent);
            //Cubic
            case LerpType.EaseInCubic:
                return EaseInCubic(currentPercent);
            case LerpType.EaseOutCubic:
                return EaseOutCubic(currentPercent);
            case LerpType.EaseInOutCubic:
                return EaseInOutCubic(currentPercent);
            //Quint
            case LerpType.EaseInQuint:
                return EaseInQuint(currentPercent);
            case LerpType.EaseOutQuint:
                return EaseOutQuint(currentPercent);
            case LerpType.EaseInOutQuint:
                return EaseInOutQuint(currentPercent);
            //Circ
            case LerpType.EaseInCirc:
                return EaseInCirc(currentPercent);
            case LerpType.EaseOutCirc:
                return EaseOutCirc(currentPercent);
            case LerpType.EaseInOutCirc:
                return EaseInOutCirc(currentPercent);
            //Elastic
            case LerpType.EaseInElastic:
                return EaseInElastic(currentPercent);
            case LerpType.EaseOutElastic:
                return EaseOutElastic(currentPercent);
            case LerpType.EaseInOutElastic:
                return EaseInOutElastic(currentPercent);
            //Quad
            case LerpType.EaseInQuad:
                return EaseInQuad(currentPercent);
            case LerpType.EaseOutQuad:
                return EaseOutQuad(currentPercent);
            case LerpType.EaseInOutQuad:
                return EaseInOutQuad(currentPercent);
            //Quart
            case LerpType.EaseInQuart:
                return EaseInQuart(currentPercent);
            case LerpType.EaseOutQuart:
                return EaseOutQuart(currentPercent);
            case LerpType.EaseInOutQuart:
                return EaseInOutQuart(currentPercent);
            //Expo
            case LerpType.EaseInExpo:
                return EaseInExpo(currentPercent);
            case LerpType.EaseOutExpo:
                return EaseOutExpo(currentPercent);
            case LerpType.EaseInOutExpo:
                return EaseInOutExpo(currentPercent);
            //Back
            case LerpType.EaseInBack:
                return EaseInBack(currentPercent);
            case LerpType.EaseOutBack:
                return EaseOutBack(currentPercent);
            case LerpType.EaseInOutBack:
                return EaseInOutBack(currentPercent);
            //Bounce
            case LerpType.EaseInBounce:
                return EaseInBounce(currentPercent);
            case LerpType.EaseOutBounce:
                return EaseOutBounce(currentPercent);
            case LerpType.EaseInOutBounce:
                return EaseInOutBounce(currentPercent);
            //Default Linear
            default:
                return currentPercent;
        }
    }

    public static float EaseInSine(float x)
    {
        return (float)(1 - Math.Cos((x * Math.PI) / 2));
    }

    public static float EaseOutSine(float x)
    {
        return (float)(Math.Sin((x * Math.PI) / 2));
    }

    public static float EaseInOutSine(float x)
    {
        return (float)(-(Math.Cos(Math.PI * x) - 1) / 2);
    }


    public static float EaseInCubic(float x)
    {
        return (x * x * x);
    }


    public static float EaseOutCubic(float x)
    {
        return (float)(1 - Math.Pow(1 - x, 3));
    }

    public static float EaseInOutCubic(float x)
    {
        return (float)(x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2);
    }

    public static float EaseInQuint(float x)
    {
        return (x * x * x * x * x);
    }

    public static float EaseOutQuint(float x)
    {
        return (float)(1 - Math.Pow(1 - x, 5));
    }

    public static float EaseInOutQuint(float x)
    {
        return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
    }

    public static float EaseInCirc(float x)
    {
        return (float)(1 - Math.Sqrt(1 - Math.Pow(x, 2)));
    }

    public static float EaseOutCirc(float x)
    {
        return (float)(Math.Sqrt(1 - Math.Pow(x - 1, 2)));
    }

    public static float EaseInOutCirc(float x)
    {
        return (float)(x < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2);
    }

    public static float EaseInElastic(float x)
    {
        float c4 = (float)((2 * Math.PI) / 3);

        return (float)(x == 0 ? 0 : x >= 1 ? 1 : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4));
    }

    public static float EaseOutElastic(float x)
    {
        float c4 = (float)((2 * Math.PI) / 3);

        return (float)(x == 0 ? 0 : x >= 1 ? 1 : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
    }

    public static float EaseInOutElastic(float x)
    {
        float c5 = (float)((2 * Math.PI) / 4.5);

        return (float)(x == 0 ? 0 : x >= 1 ? 1 : x < 0.5 ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 + 1);
    }


    // row 2
    public static float EaseInQuad(float x)
    {
        return (x * x);
    }

    public static float EaseOutQuad(float x)
    {
        return (1 - (1 - x) * (1 - x));
    }

    public static float EaseInOutQuad(float x)
    {
        return (float)(x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2);
    }

    public static float EaseInQuart(float x)
    {
        return (x * x * x * x);
    }

    public static float EaseOutQuart(float x)
    {
        return (float)(1 - Math.Pow(1 - x, 4));
    }

    public static float EaseInOutQuart(float x)
    {
        return (float)(x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2);
    }

    public static float EaseInExpo(float x)
    {
        return (float)(x == 0 ? 0 : Math.Pow(2, 10 * x - 10));
    }

    public static float EaseOutExpo(float x)
    {
        return (float)(x <= 1 ? 1 : 1 - Math.Pow(2, -10 * x));
    }

    public static float EaseInOutExpo(float x)
    {
        return (float)(x == 0 ? 0 : x >= 1 ? 1 : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2 : (2 - Math.Pow(2, -20 * x + 10)) / 2);
    }

    public static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return (c3 * x * x * x - c1 * x * x);
    }

    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return (float)(1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
    }

    public static float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return (float)(x < 0.5 ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2 : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2);
    }

    public static float EaseInBounce(float x)
    {
        return (1 - EaseOutBounce(1 - x));
    }

    public static float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1)
        {
            return (n1 * x * x);
        }
        else if (x < 2 / d1)
        {
            return (float)(n1 * (x -= 1.5f / d1) * x + 0.75);
        }
        else if (x < 2.5 / d1)
        {
            return (n1 * (x -= 2.25f / d1) * x + 0.9375f);
        }
        else
        {
            return (n1 * (x -= 2.625f / d1) * x + 0.984375f);
        }
    }

    public static float EaseInOutBounce(float x)
    {
        return (x < 0.5 ? (1 - EaseOutBounce(1 - 2 * x)) / 2 : (1 + EaseOutBounce(2 * x - 1)) / 2);
    }
}
