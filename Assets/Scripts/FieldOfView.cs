using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private float fov;
    private Vector3 origin;
    private float startingAngle;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fov = 360f;
        origin = Vector3.zero;
    }


    // Late Update
    void LateUpdate()
    { 
        int rayCount = 100;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 5f;

        Vector3[] verticies = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[verticies.Length];
        int[] triangles = new int[rayCount * 3];

        verticies[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int index = 0; index <= rayCount; index++)
        {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if (hit.collider == null)
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                vertex = hit.point;
                vertex.x += .5f;
                vertex.y -= .5f;
                vertex.z -= .5f;
                Debug.Log("Hitting Something");
            }

            verticies[vertexIndex] = vertex;

            if (index > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
        Debug.Log("Updating");
    }

    public void SetPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }
    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) - fov / 2f;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
