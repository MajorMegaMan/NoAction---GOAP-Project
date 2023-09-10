using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [SerializeField] List<GameObject> m_eatPositionObjects = new List<GameObject>();
    [SerializeField] List<EatingPosition> m_eatingPositions;
    Queue<EatingPosition> m_openEatingPositions = new Queue<EatingPosition>();
    HashSet<EatingPosition> m_closedEatingPositions = new HashSet<EatingPosition>();

    int m_grabCount = 0;

    [SerializeField] int m_shuffleCount = 16;

    System.Random m_rand = new System.Random();

    public bool tableReady { get { return m_openEatingPositions.Count > 0; } }

    private void Awake()
    {
        foreach (var e in m_eatPositionObjects)
        {
            var eatPositionsArray = e.GetComponentsInChildren<EatingPosition>();
            foreach (var eatPos in eatPositionsArray)
            {
                m_openEatingPositions.Enqueue(eatPos);
                eatPos.table = this;
            }
        }

        foreach (var eatPos in m_eatingPositions)
        {
            m_openEatingPositions.Enqueue(eatPos);
            eatPos.table = this;
        }

        ShuffleEatPositions();
    }

    public EatingPosition PopOpenEatingPosition()
    {
        if (m_openEatingPositions.Count > 0)
        {
            if(m_grabCount++ > m_shuffleCount)
            {
                ShuffleEatPositions();
            }
            var eatPos = m_openEatingPositions.Dequeue();
            m_closedEatingPositions.Add(eatPos);
            return eatPos;
        }
        return null;
    }

    public void RestoreEatingPosition(EatingPosition eatPosition)
    {
        if(m_closedEatingPositions.Remove(eatPosition))
        {
            m_openEatingPositions.Enqueue(eatPosition);
        }
    }

    public EatingPosition PeekTopClosed()
    {
        if(m_closedEatingPositions.Count == 0)
        {
            return null;
        }
        return m_closedEatingPositions.First();
    }

    public void ShuffleEatPositions()
    {
        var shuffledList = m_openEatingPositions.ToList();
        shuffledList = shuffledList.OrderBy(_ => m_rand.Next()).ToList();
        m_openEatingPositions.Clear();
        foreach (var item in shuffledList)
        {
            m_openEatingPositions.Enqueue(item);
        }
    }

    public Vector3 GetRandomTablePosition()
    {
        if (m_closedEatingPositions.Count > 0)
        {
            int rand = Random.Range(0, m_closedEatingPositions.Count);
            int i = 0;
            foreach(var item in m_closedEatingPositions)
            {
                if(rand == i)
                {
                    return item.transform.position;
                }
                i++;
            }    
        }

        if (m_openEatingPositions.Count > 0)
        {
            int rand = Random.Range(0, m_openEatingPositions.Count);
            int i = 0;
            foreach (var item in m_openEatingPositions)
            {
                if (rand == i)
                {
                    return item.transform.position;
                }
                i++;
            }
        }

        return Vector3.zero;
    }
}
