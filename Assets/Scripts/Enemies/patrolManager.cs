using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class patrolManager : MonoBehaviour
{
    [SerializeField]
    private float patrolPointDist;
    [SerializeField]
    private int maxPoints;
    private Transform[] patrolPoints;
    private int pointNum;

    //this sets up the patrol points for an enemy by creating a path from the nearby points
    public void WakeEnemy(PatrolPoint[] points)
    {
        //add nearby all points to the list
        List<Transform> closePoints = new List<Transform>();
        foreach (PatrolPoint pp in points)
        {
            //check if within the range
            if (Vector3.SqrMagnitude(pp.transform.position - transform.position) <= patrolPointDist * patrolPointDist)
            {
                closePoints.Add(pp.transform);
            }
        }

        //sorts the list
        closePoints = closePoints.OrderBy(p => Vector3.Distance(p.position, transform.position)).ToList();

        //remove extra points
        for (int i = closePoints.Count - 1; i >= maxPoints; i--)
        {
            closePoints.RemoveAt(i);
        }

        //order the points by TSP
        patrolPoints = sortTSP(closePoints);
    }

    private Transform[] sortTSP(List<Transform> transforms)
    {
        List<Transform> shortestPath = new List<Transform>();

        if (transforms == null || transforms.Count == 0)
        {
            return shortestPath.ToArray();
        }

        int numTransforms = transforms.Count;
        float[,] distanceMatrix = new float[numTransforms, numTransforms];

        // Build a distance matrix that contains the distance between each pair of transforms
        for (int i = 0; i < numTransforms; i++)
        {
            for (int j = 0; j < numTransforms; j++)
            {
                distanceMatrix[i, j] = Vector3.Distance(transforms[i].position, transforms[j].position);
            }
        }

        // Initialize an array to store the order in which the transforms will be visited
        int[] visitOrder = new int[numTransforms];
        for (int i = 0; i < numTransforms; i++)
        {
            visitOrder[i] = i;
        }

        // Use a simple greedy algorithm to find the shortest path
        float shortestDistance = float.MaxValue;
        int[] shortestVisitOrder = new int[numTransforms];
        do
        {
            float distance = 0f;
            for (int i = 0; i < numTransforms - 1; i++)
            {
                int fromIndex = visitOrder[i];
                int toIndex = visitOrder[i + 1];
                distance += distanceMatrix[fromIndex, toIndex];
            }
            int lastIndex = visitOrder[numTransforms - 1];
            int firstIndex = visitOrder[0];
            distance += distanceMatrix[lastIndex, firstIndex];

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestVisitOrder = (int[])visitOrder.Clone();
            }
        } while (NextPermutation(visitOrder));

        // Convert the visit order to a list of transforms in the correct order
        for (int i = 0; i < numTransforms; i++)
        {
            shortestPath.Add(transforms[shortestVisitOrder[i]]);
        }

        return shortestPath.ToArray();
    }

    private bool NextPermutation(int[] array)
    {
        int i = array.Length - 2;
        while (i >= 0 && array[i] >= array[i + 1])
        {
            i--;
        }
        if (i < 0)
        {
            return false;
        }
        int j = array.Length - 1;
        while (array[j] <= array[i])
        {
            j--;
        }
        int temp = array[i];
        array[i] = array[j];
        array[j] = temp;
        int start = i + 1;
        int end = array.Length - 1;
        while (start < end)
        {
            temp = array[start];
            array[start] = array[end];
            array[end] = temp;
            start++;
            end--;
        }
        return true;
    }

    public Vector3 getNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.Log("No patrol points assigned to " + transform.name + ": This will cause slowdown");
            return transform.position;
        }

        Vector3 ret = patrolPoints[pointNum].position;
        pointNum = (pointNum + 1) % patrolPoints.Length;
        return ret;
    }

    //currently just gets random one from ones on origional path, could change to simply be random in the surrounding area
    public Vector3 getRandomPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.Log("No patrol points assigned to " + transform.name + ": This will cause slowdown");
            return transform.position;
        }
        return patrolPoints[Random.Range(0, patrolPoints.Length)].position;
    }
}
