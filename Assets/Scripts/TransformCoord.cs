using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformCoord {

	public static Vector3 Transform(Vector3 StartVector, Vector3 EndVector, Vector3 Angles)
    {
        var NewVector = StartVector - EndVector;
        return Rotate(NewVector, Angles);
    }

    public static Vector3 Rotate(Vector3 vector, Vector3 Angles)
    {
        var CosX = Mathf.Cos(Angles.x / 57.2958f);
        var SinX = Mathf.Sin(Angles.x / 57.2958f);

        var CosY = Mathf.Cos(Angles.y / 57.2958f);
        var SinY = Mathf.Sin(Angles.y / 57.2958f);

        var CosZ = Mathf.Cos(Angles.z / 57.2958f);
        var SinZ = Mathf.Sin(Angles.z / 57.2958f);

        var MatrixX = new float[3, 3] { { 1,0,0}, {0,CosX, -SinX }, {0,SinX,CosX } };
        var MatrixY = new float[3, 3] { { CosY, 0, SinY }, { 0, 1, 0 }, { -SinY,0,CosY} };
        var MatrixZ = new float[3, 3] { { CosZ, -SinZ,0},{ SinZ,CosZ,0},{ 0,0,1} };

        var NewVector = new Vector3();

        NewVector = Matrix.Mul(vector, MatrixX);
        NewVector = Matrix.Mul(NewVector, MatrixY);
        NewVector = Matrix.Mul(NewVector, MatrixZ);

        return NewVector;
    }
}
