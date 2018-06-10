using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Интерфейс описывающий подвижный объект
public interface IMovableGameObject
{
    //Портация объекта. На вход принимает входной и выходной портал
    void Portate(GameObject PortalOne, GameObject PortalTwo);

    //Заканчиваем поратцию нашего объекта
    void EndPortate();

    //Портируется ли сейчас наш объект
    bool IsPortate { get;}
}
