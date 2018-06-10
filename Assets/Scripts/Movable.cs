using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*----------------------------------
 * По сути телепортировать объект мы не будет
 * При входе объекта в телепор мы будет создавать его копию за другим телепортов
 * Разумеется физика оригинального объекта и его копии должны быть взаимосвязаны
 * Здесь будет прописана их взаимосвязь
 * --------------------------------------*/
public class Movable : MonoBehaviour, IMovableGameObject, IShadow
{
    [SerializeField]
    private GameObject ShadowGameObject;
    private IShadow Shadow;

    private Collider thisCollider;
    private Rigidbody RBody;
    private Rigidbody ShadowRBoy;

    [SerializeField]
    private bool IsShadow; //Является ли наш объект копией

    private bool isPortate; //Портируется ли в данный момент наш объект
    public bool IsPortate { get { return isPortate; } }

    private GameObject ThisPortal; // Портал через который проходит наш объект
    private GameObject ShadowPortal;

    private Vector3[] StartedVecticles;
    private int[] StartedTriangles;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    // Use this for initialization
    void Start()
    {
        thisCollider = GetComponent<Collider>();
        Shadow = ShadowGameObject.GetComponent<IShadow>();
        RBody = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        StartedTriangles = meshFilter.sharedMesh.triangles;
        StartedVecticles = meshFilter.sharedMesh.vertices;
        ShadowRBoy = ShadowGameObject.GetComponent<Rigidbody>();
        meshCollider = GetComponent<MeshCollider>();

        //Если объект - тень, то деактивируем его
        if (IsShadow)
        {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        //Если объект телепортируется
        if (isPortate)
        {
            DrawVertices();
        }
    }

    //Отлавливаем коллизии с другими объектами и корректируем поведение теневого объекта
    private void OnCollisionEnter(Collision collision)
    {
        //Вычисляем скорость объекта после коллизии
        var Speed = RBody.velocity;
        //Поворачиваем скорость на нужный угол
        var Angles = ShadowPortal.transform.eulerAngles - ThisPortal.transform.eulerAngles;
        var ShadowSpeed = TransformCoord.Rotate(Speed, Angles);
        ShadowSpeed *= -1;
        //Прибавляем скорость к теневому объекту
        ShadowRBoy.velocity = ShadowSpeed;
    }

    #region IMovable
    public void Portate(GameObject StartPortal, GameObject EndPortal)
    {
        if (!IsShadow) //Если наш объект не является "тенью" то разрешаем его телепортацию
        {
            ThisPortal = StartPortal;
            ShadowPortal = StartPortal;
            //Ставим теневой объект за вторым поратолом
            //Для начала определяем координаты текущего объекта относительно портала
            var ObjectCoord = transform.position;
            var PortalCoord = StartPortal.transform.position;
            var PortalAngles = StartPortal.transform.eulerAngles;
            var ShadowCoordAtPortal = TransformCoord.Transform(ObjectCoord, PortalCoord, PortalAngles);

            //Затем ставим теневой объект за вторым порталом точно так же расположен первый объект относительно порала
            //Только инвертируем координату у, чтобы объект был за порталом
            ShadowCoordAtPortal.y *= -1;
            
            //Теперь изменияем вектор скорости для теневого объекта
            var ShadowVelocity = RBody.velocity;
            //Сначала поворачивем наш вектор на нунжый угол
            var Angles = EndPortal.transform.eulerAngles - StartPortal.transform.eulerAngles;
            ShadowVelocity = TransformCoord.Rotate(ShadowVelocity, Angles);
            ShadowVelocity *= -1;

            Shadow.SetShadowPortate(StartPortal, EndPortal,ShadowVelocity, ShadowCoordAtPortal);

            isPortate = true;
        }
    }
  
    public void EndPortate()
    {
        //При окончании телепортации приодим меш к первоначальному виду
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.vertices = StartedVecticles;
        meshFilter.mesh.triangles = StartedTriangles;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();

        //Если при выхоже из портала наш объект поздади него, то объект делаем неактивным
        if (BehingThePortal(transform.position,ThisPortal))
        {
            RBody.Sleep();
            IsShadow = true;
            gameObject.SetActive(false);
        }
        else
        {
            IsShadow = false;
        }

        isPortate = false;
        ThisPortal = null;
    }
    #endregion IMovable

    #region IShadow
    public void SetShadowPortate(GameObject StartPortal, GameObject EndPortal, Vector3 velocity, Vector3 PositionAtPotal)
    {
        isPortate = true;
        gameObject.SetActive(true);
        RBody.WakeUp();

        //Переводим координаты объекта относительно портала в глобальные координаты
        //Сначала поворачиваем вектор обозначающий позицию за порталом на угол поворота портала
        var PortalAngles = EndPortal.transform.eulerAngles;
        //Углы умножаем на минус один, поскольку в формулах поворота положительное направление угла - против часовой стрелки. А в unity - по
        PortalAngles.x *= -1;
        PortalAngles.y *= -1;
        PortalAngles.z *= -1;
        var ShadowCoordAtPortal = TransformCoord.Rotate(PositionAtPotal, PortalAngles);
        //Прибавляем к позиции портала наши координаты
        transform.position = EndPortal.transform.position + PositionAtPotal;

        RBody.velocity = velocity;

        IsShadow = true;
        ThisPortal = EndPortal;
        ShadowPortal = StartPortal;
        isPortate = true;

        //Отрисовываем меш
        DrawVertices();
    }

    public GameObject GetPortal()
    {
        return ThisPortal;
    }
    #endregion IShadow

    //Функция которая определяет находится ли наш объект с указанными координатами за порталом
    private bool BehingThePortal(Vector3 ObjectCoord,GameObject Portal)
    {
        var PortalCoord = Portal.transform.position;
        var PortalAngles = -1 * Portal.transform.eulerAngles;
        var ObjectCoordAtPorlat = TransformCoord.Transform(ObjectCoord, PortalCoord, PortalAngles);
        return ObjectCoordAtPorlat.y < 0;
    }

    //Функция отрисовки вершин портируемого объекта. Вершины за порталом не отрисовываются
    private void DrawVertices()
    {
        var vertices = new List<Vector3>(StartedVecticles);
        var Triangles = new TriangleList(StartedTriangles);

        //Ищем вершины которые находятся за поратлом
        //И удаляем их
        for (int i =0; i < vertices.Count;i++)
        {
            Vector3 GlobalVertexCoord = transform.TransformPoint(vertices[i]);
            if (BehingThePortal(GlobalVertexCoord, ThisPortal))
            {
                vertices.Remove(vertices[i]);
                Triangles.DeleteVertex(i);
                --i;
            }
        }

        //Перестраиваем меш
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = Triangles.ToArray();
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
    }
}
