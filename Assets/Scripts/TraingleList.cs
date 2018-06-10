using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс описывающий список всех треугольников меша
public class TriangleList
{
    public List<Trianlge> ListOfTriangle; //Собсвенно сам список треугольников

    public TriangleList(int[] TrianglesMass)
    {
        ListOfTriangle = new List<Trianlge>();
        for (var i = 0; i < TrianglesMass.Length; i = i + 3)
        {
            ListOfTriangle.Add(new Trianlge(TrianglesMass[i], TrianglesMass[i + 1], TrianglesMass[i + 2]));
        }
    }

    //Функция удаления всех треугольников которые включают данную вершину
    public void DeleteVertex(int vertex)
    {
        var i = 0;
        while (i < ListOfTriangle.Count)
        {
            var one = ListOfTriangle[i].One;
            var two = ListOfTriangle[i].Two;
            var three = ListOfTriangle[i].Three;

            if (one > vertex)
                ListOfTriangle[i].One--;
            if (two > vertex)
                ListOfTriangle[i].Two--;
            if (three > vertex)
                ListOfTriangle[i].Three--;

            if (one == vertex || two == vertex || three == vertex)
            {
                ListOfTriangle.Remove(ListOfTriangle[i]);
            }
            else
            {
                i++;
            }
        }
    }

    //Функция перевода списка вершин в массив
    public int[] ToArray()
    {
        var array = new int[3 * ListOfTriangle.Count];
        for (var i = 0; i < ListOfTriangle.Count; i++)
        {
            array[3 * i] = ListOfTriangle[i].One;
            array[3 * i + 1] = ListOfTriangle[i].Two;
            array[3 * i + 2] = ListOfTriangle[i].Three;
        }

        return array;
    }
}
