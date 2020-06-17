using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float cubeSize = 0.5f;
    public int cubesInRow = 3;
    public float explosionRadius = 5f;
    public float explosionForce = 20f;
    public float explosionUpward = 1f;
    float cubesPivotDistance;
    Vector3 cubesPivot;
    Collider m_Collider;

    // Start is called before the first frame update
    void Start()
    {
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
        m_Collider = GetComponent<Collider>();
    }

    public void Explode()
    {
        gameObject.SetActive(false);

        //createPiece();
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    CreatePiece(x, y, z);
                }
            }
        }

        //get explosion position
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody RoundBounce = hit.GetComponent<Rigidbody>();
            if (RoundBounce != null)
            {
                RoundBounce.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }
        Destroy(gameObject);
    }

    void CreatePiece(int x, int y, int z)
    {
        //create piece
        GameObject pieceObj;
        pieceObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //set piece position and scale
        pieceObj.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        pieceObj.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //add rigidbody and set mass
        pieceObj.AddComponent<Rigidbody>().mass = cubeSize;

        // Add material
        pieceObj.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;

        pieceObj.GetComponent<Collider>().isTrigger = false;

        Destroy(pieceObj, 3f);
    }
}