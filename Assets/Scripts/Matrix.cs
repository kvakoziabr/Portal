using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Matrix {

	public static Vector3 Mul(Vector3 vector, float[,] Matrix)
    {
        var NewVector = new Vector3();
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                NewVector[i] += Matrix[i, j] * vector[j];
            }
        }

        return NewVector;
    }
}
