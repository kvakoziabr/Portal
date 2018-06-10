using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Интерфейс для теневого объекта
public interface IShadow
{
    //Через эту функцию мы будет сообщать теневому объекту, о начале телепоратции.
    void SetShadowPortate(GameObject StartPortal,GameObject EndPortal, Vector3 velocity, Vector3 PositionAtPotal);

    //Получить портал за котором находится объект
    GameObject GetPortal();
}
