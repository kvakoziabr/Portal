using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс описывающий один треугольник меша (в unity они просто забиты массивами)
public class Trianlge
{
    public int One;
    public int Two;
    public int Three;

    public Trianlge(int one, int two, int three)
    {
        One = one;
        Two = two;
        Three = three;
    }
}
