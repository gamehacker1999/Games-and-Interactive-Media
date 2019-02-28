using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author Shubham Sachdeva
//Class used for seperating axis theorem collisions
//Restrictions - Only works with rectangles
public class SATCollision : MonoBehaviour
{
    //Fields for the class
    //corners of the first gameobject
    Vector3 forwardLeft1=Vector3.zero;
    Vector3 forwardRight1 = Vector3.zero;
    Vector3 backLeft1 = Vector3.zero;
    Vector3 backRight1 = Vector3.zero;

    //corners of the second gameobject
    Vector3 forwardLeft2 = Vector3.zero;
    Vector3 forwardRight2 = Vector3.zero;
    Vector3 backLeft2 = Vector3.zero;
    Vector3 backRight2 = Vector3.zero;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Method to detect collision using SAT
    /// </summary>
    /// <param name="g1">First collidable gameobject</param>
    /// <param name="g2">Second collidable gameobject</param>
    /// <returns></returns>
    public bool IsColliding(GameObject g1, GameObject g2)
    {
        bool colliding = false;

        //Getiing the corner positions of the 2 gameobjects
        GetCornerPositions(g1.transform, out forwardLeft1, out forwardRight1, out backRight1, out backLeft1);
        GetCornerPositions(g2.transform, out forwardLeft2, out forwardRight2, out backRight2, out backLeft2);

        //Caliing the method to check for collision
        colliding = SATRectangles();
        return colliding;
    }

    /// <summary>
    /// Method for the 4 corners of the gameobject
    /// </summary>
    /// <param name="transform">The transform component of the gameobject</param>
    /// <param name="forwardLeft">Top left corner</param>
    /// <param name="forwardRight">Top right corner</param>
    /// <param name="backRight">Bottom right corner</param>
    /// <param name="backLeft">Bottom left corner</param>
    private void GetCornerPositions(Transform transform, out Vector3 forwardLeft, out Vector3 forwardRight,
        out Vector3 backRight, out Vector3 backLeft)
    {
        //Getting the corner positions
        forwardLeft = (transform.position + transform.forward * transform.localScale.z/2 ) - transform.right * transform.localScale.x/2 ;
        forwardRight = (transform.position + transform.forward * transform.localScale.z/2) + transform.right * transform.localScale.x/2 ;
        backLeft = (transform.position - transform.forward * transform.localScale.z/2 ) - transform.right * transform.localScale.x/2;
        backRight = (transform.position - transform.forward * transform.localScale.z/2 ) + transform.right * transform.localScale.x/2 ;
    }

    /// <summary>
    /// Method for the SAT collision
    /// </summary>
    /// <returns></returns>
    private bool SATRectangles()
    {
        //Boolean to check if SAT detected collision
        bool isIntersecting = false;

        //Finding the 4 normal vectors 
        //We have just 4 normals because the other 4 normals are the same but in another direction
        Vector3 normal1 = GetNormal(backLeft1, forwardLeft1);
        
        Vector3 normal2 = GetNormal(forwardLeft1, forwardRight1);
        
        Vector3 normal3 = GetNormal(backLeft2, forwardLeft2);
        
        Vector3 normal4 = GetNormal(forwardLeft2, forwardRight2);

        //checking to see if the sides are overlapping using the normals
        if(!IsOverlapping(normal1))
        {
            return isIntersecting;
        }

        if (!IsOverlapping(normal2))
        {
            return isIntersecting;
        }

        if (!IsOverlapping(normal3))
        {
            return isIntersecting;
        }

        if (!IsOverlapping(normal4))
        {
            return isIntersecting;
        }

        //If any of the sides are overlapping, return true
        isIntersecting = true;
        return isIntersecting;
    }

    //Method to find the dotproduct
    private static float DotProduct(Vector3 v1, Vector3 v2)
    {
        //2d space
        //Formula for the dot product of 2 vectors
        float dotProduct = v1.x * v2.x + v1.z * v2.z;
        return dotProduct;
    }

    /// <summary>
    /// Method to find a perpendicular line
    /// </summary>
    /// <param name="startPos">Starting vector</param>
    /// <param name="endPos">Ending vector</param>
    /// <returns></returns>
    private Vector3 GetNormal(Vector3 startPos, Vector3 endPos)
    {
        //The direction
        Vector3 dir = endPos - startPos;
        //The normal, just flip x and y and make one negative (don't need to normalize it)
        Vector3 normal = new Vector3(-dir.z, dir.y, dir.x);
        return normal;
    }

    /// <summary>
    /// Method to check if it is overlapping
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    private bool IsOverlapping(Vector3 normal)
    {
        bool isOverlapping = false;

        //Project the corners of rectangle 1 onto the normal
        float dot1 = DotProduct(normal, forwardLeft1)/Vector3.Magnitude(normal);
        float dot2 = DotProduct(normal, forwardRight1) / Vector3.Magnitude(normal);
        float dot3 = DotProduct(normal, backLeft1) / Vector3.Magnitude(normal);
        float dot4 = DotProduct(normal, backRight1) / Vector3.Magnitude(normal);

        //Find the range
        float min1 = Mathf.Min(dot1, Mathf.Min(dot2, Mathf.Min(dot3, dot4)));
        float max1 = Mathf.Max(dot1, Mathf.Max(dot2, Mathf.Max(dot3, dot4)));


        //Project the corners of rectangle 2 onto the normal
        float dot5 = DotProduct(normal, forwardLeft2) / Vector3.Magnitude(normal);
        float dot6 = DotProduct(normal, forwardRight2) / Vector3.Magnitude(normal);
        float dot7 = DotProduct(normal, backLeft2) / Vector3.Magnitude(normal);
        float dot8 = DotProduct(normal, backRight2)/Vector3.Magnitude(normal);

        //Find the range
        float min2 = Mathf.Min(dot5, Mathf.Min(dot6, Mathf.Min(dot7, dot8)));
        float max2 = Mathf.Max(dot5, Mathf.Max(dot6, Mathf.Max(dot7, dot8)));


        //Are the ranges overlapping?
        if (min2 <= max1 && min1 <= max2)
        {
            isOverlapping = true;
        }

        return isOverlapping;
    }
}
