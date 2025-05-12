using UnityEngine;
using System.Collections;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segment;
    [SerializeField] GameObject buildings;
    [SerializeField] int zPos = 60;
    [SerializeField] bool creatingSegment = false;
    [SerializeField] int segmentNum;
    [SerializeField] int segmentLength = 50; // Constant segment length

    void Start()
    {
        for(int i = 0; i < 3; i++)
        {
            GenerateSegment();
        }
    }

    void GenerateSegment()
    {
        segmentNum = Random.Range(0, segment.Length);
        Instantiate(segment[segmentNum], new Vector3(0, 0, zPos), Quaternion.Euler(0, 90, 0));
        Instantiate(buildings, new Vector3(7.5f, 10, zPos), Quaternion.identity);
        zPos += segmentLength;
    }

    void Update()
    {
        if (!creatingSegment)
        {
            creatingSegment = true;
            StartCoroutine(SegmentGen());
        }
    }

    IEnumerator SegmentGen()
    {
        GenerateSegment();
        yield return new WaitForSeconds(3);
        creatingSegment = false;
    }
}