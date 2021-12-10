using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject newMapNodeObject;
    public GameObject splatter;
    public GameObject mapLine;
    public Transform scrollable;

    public List<Encounter> encounters;
    List<string> trackers;

    public Gradient[] lineGrad;

    private int journeyCount = 0;
    public List<MapNode> options;
    private NodeType currentType;
    private int choice = 0;
    public Transform currentNode;

    private List<bool> directions = new List<bool>{false, false, false};
    private List<bool> blockDirections = new List<bool> { false, false, false };

    private Dictionary<NodeType, int> amountPerNode;

    void Start()
    {
        amountPerNode = new Dictionary<NodeType, int>()
        {
            { NodeType.REGULAR, 3},
            { NodeType.STRUCTURE, 2},
            { NodeType.FOREST, 3},
            { NodeType.POND, 1}
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Progress();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            choice++;
            if (choice >= options.Count)
            {
                choice = 0;
            }
            ChooseDirection(choice);
        }

        if (Mathf.Abs(transform.position.x - currentNode.position.x) >= 0 || Mathf.Abs(transform.position.y - currentNode.position.y) >= 0)
        {
            Vector3 driftPosition = new Vector3();
            driftPosition.x = Mathf.Lerp(transform.position.x, currentNode.position.x, Time.deltaTime);
            driftPosition.y = Mathf.Lerp(transform.position.y, currentNode.position.y, Time.deltaTime);
            transform.position = driftPosition;
        }
    }

    void Progress()
    {
        GenDirection();

        int dirCount = 0;

        options.Clear();

        --amountPerNode[currentNode.GetComponent<MapNode>().selfType];
        //amountPerNode.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Debug.Log);
        //Debug.LogError("hi");

        if(journeyCount == 8)
        {
            directions = new List<bool> { false, true, false };
        }

        foreach (bool generate in directions) 
        {
            if (generate)
            {
                Vector3 newPoint1 = new Vector3(1, 1, 1);

                Vector3 newMapNodePosition = currentNode.position;
                newMapNodePosition.y += 50 + Random.Range(-10, 10);
                newMapNodePosition.x += (dirCount * 50) - 50 + Random.Range(-10, 10);

                GameObject newNode = SpawnNode(newMapNodePosition, dirCount);
                SpawnLines(newNode, newMapNodePosition, dirCount);
            }

            dirCount++;
        }

        journeyCount++;
    }

    GameObject SpawnNode(Vector3 newMapNodePosition, int dirCount) 
    {
        if (journeyCount > 9)
            return null; // TODO

        GameObject newNode = GameObject.Instantiate(newMapNodeObject);
        newNode.transform.position = newMapNodePosition;
        newNode.transform.parent = scrollable;
        MapNode newMapNode = newNode.GetComponent<MapNode>();
        options.Add(newMapNode);
        newMapNode.direction = dirCount;

        float RATIO_CHANCE_REG = 0.50f;
        NodeType nodeType = NodeType.REGULAR;
        if(journeyCount == 0)
        {
            nodeType = NodeType.REGULAR;
        }
        else if(journeyCount == 8)
        {
            nodeType = NodeType.CASTLE;
        }
        else
        {
            do
            {
                //4 is exclusive, so doesnt include castle
                float chance = Random.Range(0.0f, 1.0f);
                int lastOption = (journeyCount <= 4) ?  3 : 4;
                nodeType = chance <= RATIO_CHANCE_REG ? nodeType = NodeType.REGULAR : (NodeType)Random.Range(1, lastOption);
            }
            while (amountPerNode[nodeType] <= 0);

        }
        newMapNode.SetNodeType(nodeType);

        return newNode;
    }

    void SpawnLines(GameObject newNode, Vector3 newMapNodePosition, int dirCount)
    {
        GameObject newLine = GameObject.Instantiate(mapLine);
        newLine.transform.position = newMapNodePosition;
        newLine.transform.parent = newNode.transform;
        LineRenderer line = newLine.GetComponent<LineRenderer>();

        Vector3[] LinePoints = new Vector3[5];

        if (dirCount != 1)
        {
            LinePoints = (new Vector3[5]{

            new Vector3(currentNode.position.x, currentNode.position.y, 0),
            new Vector3(currentNode.position.x + (20 * (dirCount - 1)) , currentNode.position.y + Random.Range(-3, 3), 0),
            new Vector3(newMapNodePosition.x - (10 * (dirCount - 1)), currentNode.position.y + 10, 0),
            new Vector3(newMapNodePosition.x - Random.Range(0, 3) * (dirCount - 1), newMapNodePosition.y - 20 , 0),
            new Vector3(newMapNodePosition.x, newMapNodePosition.y, 0)});
        }
        else
        {
            LinePoints = (new Vector3[5]{

            new Vector3(currentNode.position.x, currentNode.position.y, 0),
            new Vector3(newMapNodePosition.x + Random.Range(0, 3), currentNode.position.y + 15, 0),
            new Vector3(newMapNodePosition.x, currentNode.position.y + 30, 0),
            new Vector3(newMapNodePosition.x + Random.Range(-3, 3), currentNode.position.y + 45, 0),
            new Vector3(newMapNodePosition.x, newMapNodePosition.y, 0)});
        }

        line.SetPositions(LinePoints);
        StartCoroutine(SpawnSplatter(newNode, LinePoints, dirCount, newMapNodePosition, line));
    }

    IEnumerator SpawnSplatter(GameObject newNode, Vector3[] splatPositions, int dirCount, Vector3 finalPosition, LineRenderer line)
    {
        Vector3 newSplatX = currentNode.position;

        for (int splatIndex = 0; splatIndex < splatPositions.Length; splatIndex++)
        {
            GameObject newSplat = GameObject.Instantiate(splatter);
            newSplat.transform.position = splatPositions[splatIndex];
            newSplat.transform.parent = newNode.transform;

            line.colorGradient = lineGrad[splatIndex];
            yield return new WaitForSeconds(0.1f);
        }

        /*
        for (int splatIndex = 0; splatIndex < splatPositions.Length; splatIndex++)
        {
            newSplatX.x += (dirCount - 1) * 15;

            GameObject newSplat = GameObject.Instantiate(splatter);
            newSplat.transform.position = newSplatX;
            newSplat.transform.parent = newNode.transform;
        }

        Vector3 newSplatY = newSplatX;
        Vector3 newSplatMid = newSplatX;
        newSplatMid.x -= (dirCount - 1) * 15;
        newSplatMid.y += 15;

        GameObject newSplatMidObj = GameObject.Instantiate(splatter);
        newSplatMidObj.transform.position = newSplatMid;
        newSplatMidObj.transform.parent = newNode.transform;

        for (int splatCount = 0; splatCount < 4; splatCount++)
        {
            newSplatY.y += 15;

            GameObject newSplat = GameObject.Instantiate(splatter);
            newSplat.transform.position = newSplatY;
            newSplat.transform.parent = newNode.transform;
        }
        */

        GameObject newSplatFinal = GameObject.Instantiate(splatter);
        newSplatFinal.transform.position = finalPosition;
        newSplatFinal.transform.parent = newNode.transform;
    }

    void GenDirection()
    {
        directions = new List<bool> { false, false, false };

        int guaranteed = Random.Range(0, 2);
        directions[guaranteed] = true;

        for(int count = 0; count < directions.Count; count++)
        {
            if (count != guaranteed)
            {
                int chance = Random.Range(0, 10);
                if (chance >= 6)
                {
                    directions[count] = true;
                }
            }
        }

        for (int blockCount = 0; blockCount < blockDirections.Count; blockCount++)
        {
            if (blockDirections[blockCount])
            {
                directions[blockCount] = false;

                if (blockCount == guaranteed)
                {
                    directions[1] = true;
                }
            }
        }
    }

    void ChooseDirection(int optionChoice)
    {
        blockDirections = new List<bool> { false, false, false };

        currentNode = options[optionChoice].transform;
        currentType = options[optionChoice].selfType;

        switch (options[optionChoice].direction)
        {
            case 0:
                if (directions[1])
                {
                    blockDirections[2] = true;
                }
                break;

            case 1:
                if (directions[0])
                {
                    blockDirections[0] = true;
                }
                if (directions[2])
                {
                    blockDirections[2] = true;
                }
                break;

            case 2:
                if (directions[1])
                {
                    blockDirections[0] = true;
                }
                break;
        }
    }

    void ChooseCard()
    {
        Encounter chosenEncounter = new Encounter();
        do
        {
            chosenEncounter = encounters[Random.Range(0, encounters.Count)];
        }
        while (chosenEncounter.type != currentType);


    }
}
