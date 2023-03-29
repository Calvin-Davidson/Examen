using System;
using System.Collections.Generic;
using UnityEngine;

public class EasingDictionary
{
    public enum LerpType { Linear, EaseInSine, EaseOutSine, EaseInOutSine, EaseInCubic, EaseOutCubic, EaseInOutCubic, EaseInQuint, EaseOutQuint, EaseInOutQuint, EaseInCirc, EaseOutCirc, EaseInOutCirc, EaseInElastic, EaseOutElastic, EaseInOutElastic, EaseInQuad, EaseOutQuad, EaseInOutQuad, EaseInQuart, EaseOutQuart, EaseInOutQuart, EaseInExpo, EaseOutExpo, EaseInOutExpo, EaseInBack, EaseOutBack, EaseInOutBack, EaseInBounce, EaseOutBounce, EaseInOutBounce };
    [SerializeField] private LerpType _lerpType;
    //Prepopulated dictionary with references to all functions
    public Dictionary<LerpType, Func<float, float>> easingDictionary = new Dictionary<LerpType, Func<float, float>>
    {
        //Linear
        { LerpType.Linear , new Func<float, float>(Linear) },
        //Sine
        { LerpType.EaseInSine , new Func<float, float>(EaseInSine) },
        { LerpType.EaseOutSine , new Func<float, float>(EaseOutSine) },
        { LerpType.EaseInOutSine , new Func<float, float>(EaseInOutSine) },
        //Cubic
        { LerpType.EaseInCubic , new Func<float, float>(EaseInCubic) },
        { LerpType.EaseOutCubic , new Func<float, float>(EaseOutCubic) },
        { LerpType.EaseInOutCubic , new Func<float, float>(EaseInOutCubic) },
        //Quint
        { LerpType.EaseInQuint , new Func<float, float>(EaseInQuint) },
        { LerpType.EaseOutQuint , new Func<float, float>(EaseOutQuint) },
        { LerpType.EaseInOutQuint , new Func<float, float>(EaseInOutQuint) },
        //Circ
        { LerpType.EaseInCirc , new Func<float, float>(EaseInCirc) },
        { LerpType.EaseOutCirc , new Func<float, float>(EaseOutCirc) },
        { LerpType.EaseInOutCirc , new Func<float, float>(EaseInOutCirc) },
        //Elastic
        { LerpType.EaseInElastic , new Func<float, float>(EaseInElastic) },
        { LerpType.EaseOutElastic , new Func<float, float>(EaseOutElastic) },
        { LerpType.EaseInOutElastic , new Func<float, float>(EaseInOutElastic) },
        //Quad
        { LerpType.EaseInQuad , new Func<float, float>(EaseInQuad) },
        { LerpType.EaseOutQuad , new Func<float, float>(EaseOutQuad) },
        { LerpType.EaseInOutQuad , new Func<float, float>(EaseInOutQuad) },
        //Quart
        { LerpType.EaseInQuart , new Func<float, float>(EaseInQuart) },
        { LerpType.EaseOutQuart , new Func<float, float>(EaseOutQuart) },
        { LerpType.EaseInOutQuart , new Func<float, float>(EaseInOutQuart) },
        //Expo
        { LerpType.EaseInExpo , new Func<float, float>(EaseInExpo) },
        { LerpType.EaseOutExpo , new Func<float, float>(EaseOutExpo) },
        { LerpType.EaseInOutExpo , new Func<float, float>(EaseInOutExpo) },
        //Back
        { LerpType.EaseInBack , new Func<float, float>(EaseInBack) },
        { LerpType.EaseOutBack , new Func<float, float>(EaseOutBack) },
        { LerpType.EaseInOutBack , new Func<float, float>(EaseInOutBack) },
        //Bounce
        { LerpType.EaseInBounce , new Func<float, float>(EaseInBounce) },
        { LerpType.EaseOutBounce , new Func<float, float>(EaseOutBounce) },
        { LerpType.EaseInOutBounce , new Func<float, float>(EaseInOutBounce) },
    };

    //Public callable function
    public float CalculateEaseStep(float currentPercent, LerpType lerpType)
    {
        Func<float, float> function = easingDictionary[lerpType];
        return function(currentPercent);
    }

    //Math library
    private static float Linear(float x)
    {
        return x;
    }

    private static float EaseInSine(float x)
    {
        return (float)(1 - Math.Cos((x * Math.PI) / 2));
    }

    private static float EaseOutSine(float x)
    {
        return (float)(Math.Sin((x * Math.PI) / 2));
    }

    private static float EaseInOutSine(float x)
    {
        return (float)(-(Math.Cos(Math.PI * x) - 1) / 2);
    }

    private static float EaseInCubic(float x)
    {
        return (x * x * x);
    }

    private static float EaseOutCubic(float x)
    {
        return (float)(1 - Math.Pow(1 - x, 3));
    }
    private static float EaseInOutCubic(float x)
    {
        return (float)(x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2);
    }

    private static float EaseInQuint(float x)
    {
        return (x * x * x * x * x);
    }

    private static float EaseOutQuint(float x)
    {
        return (float)(1 - Math.Pow(1 - x, 5));
    }

    private static float EaseInOutQuint(float x)
    {
        return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
    }

    private static float EaseInCirc(float x)
    {
        return (float)(1 - Math.Sqrt(1 - Math.Pow(x, 2)));
    }

    private static float EaseOutCirc(float x)
    {
        return (float)(Math.Sqrt(1 - Math.Pow(x - 1, 2)));
    }

    private static float EaseInOutCirc(float x)
    {
        return (float)(x < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2);
    }

    private static float EaseInElastic(float x)
    {
        float c4 = (float)((2 * Math.PI) / 3);

        return (float)(x == 0 ? 0 : x >= 1 ? 1 : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4));
    }

    private static float EaseOutElastic(float x)
    {
        float c4 = (float)((2 * Math.PI) / 3);

        return (float)(x == 0 ? 0 : x >= 1 ? 1 : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
    }

    private static float EaseInOutElastic(float x)
    {
        float c5 = (float)((2 * Math.PI) / 4.5);

        return (float)(x == 0 ? 0 : x >= 1 ? 1 : x < 0.5 ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 + 1);
    }
    private static float EaseInQuad(float x)
    {
        return (x * x);
    }

    private static float EaseOutQuad(float x)
    {
        return (1 - (1 - x) * (1 - x));
    }

    private static float EaseInOutQuad(float x)
    {
        return (float)(x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2);
    }

    private static float EaseInQuart(float x)
    {
        return (x * x * x * x);
    }

    private static float EaseOutQuart(float x)
    {
        return (float)(1 - Math.Pow(1 - x, 4));
    }

    private static float EaseInOutQuart(float x)
    {
        return (float)(x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2);
    }

    private static float EaseInExpo(float x)
    {
        return (float)(x == 0 ? 0 : Math.Pow(2, 10 * x - 10));
    }

    private static float EaseOutExpo(float x)
    {
        return (float)(x <= 1 ? 1 : 1 - Math.Pow(2, -10 * x));
    }

    private static float EaseInOutExpo(float x)
    {
        return (float)(x == 0 ? 0 : x >= 1 ? 1 : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2 : (2 - Math.Pow(2, -20 * x + 10)) / 2);
    }

    private static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return (c3 * x * x * x - c1 * x * x);
    }

    private static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return (float)(1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
    }

    private static float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return (float)(x < 0.5 ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2 : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2);
    }

    private static float EaseInBounce(float x)
    {
        return (1 - EaseOutBounce(1 - x));
    }

    private static float EaseOutBounce(float x)
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

    private static float EaseInOutBounce(float x)
    {
        return (x < 0.5 ? (1 - EaseOutBounce(1 - 2 * x)) / 2 : (1 + EaseOutBounce(2 * x - 1)) / 2);
    }
}
