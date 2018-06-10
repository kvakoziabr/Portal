using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public LayerMask PortableObjectLayer;
    [SerializeField]
    private GameObject OtherPortal;
    private List<Collider> ColliderListToIgnoreCollision = new List<Collider>();
    //Размеры коллайдера, при попадании в который объект будет начинать портироваться.
    //Необходимы потому что мы начинаем портировать объект не по событию OnTriggerEnter (почему смотри в FixedUpdate)
    private Vector3 HalfColliderSize;
    //Цент нашего коллайдера
    private Vector3 ColliderCenter;


    private void Start()
    {
        var collider = this.GetComponent<BoxCollider>();

        var HalfColliderSizeX = collider.size.x * transform.lossyScale.x/2;
        var HalfColliderSizeY = collider.size.y * transform.lossyScale.y/2;
        var HalfColliderSizeZ = collider.size.z * transform.lossyScale.z/2;
        HalfColliderSize = new Vector3(HalfColliderSizeX, HalfColliderSizeY, HalfColliderSizeZ);

        ColliderCenter = transform.TransformPoint(collider.center);
    }
    /*----------------------------------
      Портация портируемых объектов происходит в FixedUpdate, т.к OnTriggerEnter вызывается после просчёта физики,
      А на больший скоростях портируемого объекта касание данного объекта с препятсвием позади портала может происходить одновременно (тобишь в один кадр) со своход в портал.
      Из-за этого будет просчитываться нежелательная коллизия объекта с порталом
      ---------------------------------*/
    private void FixedUpdate()
    {
        //Ищем коллайдеры всех портируемых обхектов
        var Colliders = Physics.OverlapBox(ColliderCenter, HalfColliderSize, transform.rotation,PortableObjectLayer);
        foreach (var collider in Colliders)
        {
            //Каждый найденный, добавлем в игнор с коллайдерами за порталом
            var Movable = collider.gameObject.GetComponent<IMovableGameObject>();
            if (!Movable.IsPortate)
            {
                foreach (var col in ColliderListToIgnoreCollision)
                {
                    Physics.IgnoreCollision(collider, col);
                }
                Movable.Portate(this.gameObject, OtherPortal);
            }
        }     
    }

    private void OnTriggerEnter(Collider other)
    {
        //Если объект не портируемый портируемый
        if (other.gameObject.tag != "Portable")
        {
            //То добавляем его в список объектов для игорирования коллизий с портируемым объектом
            ColliderListToIgnoreCollision.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Portable")
        {
            //Заканчиваем телепортацию объекта если он портируемый
            var Movable = other.gameObject.GetComponent<IMovableGameObject>();
            Movable.EndPortate();

            //И отключаем игронирование коллизий
            foreach (var col in ColliderListToIgnoreCollision)
            {
                Physics.IgnoreCollision(other, col, false);
            }
        }
        else
        {
            ColliderListToIgnoreCollision.Remove(other);    
        }
    }
}
