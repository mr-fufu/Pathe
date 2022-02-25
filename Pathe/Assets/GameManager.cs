using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject newMapNodeObject;
    public GameObject splatter;
    public GameObject mapLine;
    public Transform scrollable;

    public List<Encounter> encountersList;
    List<string> trackers = new List<string> { };

    public Gradient[] lineGrad;

    private int journeyCount = 0;
    public List<MapNode> options;
    private NodeType currentType;
    private int choice = 0;
    public Transform currentNode;
    private bool doneGenerating = true;

    private List<bool> directions = new List<bool>{false, false, false};
    private List<bool> blockDirections = new List<bool> { false, false, false };

    private Dictionary<NodeType, int> amountPerNode;

    public GameObject textPanel;
    public GameObject imagePanel;
    public GameObject statsPanel;
    public GameObject pointer;
    public TextMeshPro pointerText;

    private Encounter chosenEncounter;

    public SpriteRenderer cardImage;
    public TextMeshPro cardTitle;
    public TextMeshPro cardText;
    public List<TextMeshPro> cardChoices;
    public GameObject selector;

    private bool panelChange;
    private bool panelShow;
    private float panelMove;
    private float panelPos;

    private bool forward = true;
    private bool chooseMode = false;
    private int selectedOption = 0;
    private bool colorMode = false;

    public int health;
    public int coins;
    public TextMeshPro healthCounter;
    public TextMeshPro coinsCounter;
    private bool fade;
    private float alpha = 1;
    private Color colorHold;
    private bool changeComplete;

    void Start()
    {
        Vector3 textPos = textPanel.transform.position;
        textPanel.transform.position = new Vector3(250, textPos.y, textPos.z);

        Vector3 imagePos = imagePanel.transform.position;
        imagePanel.transform.position = new Vector3(-250, imagePos.y, imagePos.z);

        Vector3 statsPos = statsPanel.transform.position;
        statsPanel.transform.position = new Vector3(statsPos.x, -150, statsPos.z);

        amountPerNode = new Dictionary<NodeType, int>()
        {
            { NodeType.REGULAR, 3},
            { NodeType.STRUCTURE, 2},
            { NodeType.FOREST, 3},
            { NodeType.POND, 1}
        };

        colorHold = cardText.color;

        changeStats(5, 15);
        Progress();
    }

    void Update()
    {
        if (doneGenerating)
        {
            if (colorMode)
            {
                if (fade)
                {
                    if (alpha > 0)
                    {
                        alpha -= Time.deltaTime * 2f;
                    }
                    else
                    {
                        alpha = 0;
                        showText();
                        fade = false;
                        changeComplete = false;
                    }
                }
                else
                {
                    if (alpha < 1)
                    {
                        alpha += Time.deltaTime * 2f;
                    }
                    else
                    {
                        alpha = 1;
                        changeComplete = true;
                    }
                }
                colorHold.a = alpha;
                cardText.color = colorHold;

                if (changeComplete)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        colorMode = false;
                        chooseMode = false;
                        Progress();
                        forward = true;
                    }
                }
            }
            else if (!chooseMode)
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (forward)
                    {
                        if (directions[2])
                        {
                            forward = false;

                            if (directions[0])
                            {
                                if (directions[1])
                                {
                                    ChooseDirection(2);
                                }
                            }
                            else if (directions[1])
                            {
                                ChooseDirection(1);
                            }
                            else
                            {
                                ChooseDirection(0);
                            }
                        }
                    }
                    else
                    {
                        choice++;
                        if (choice >= options.Count)
                        {
                            choice = options.Count - 1;
                        }
                        ChooseDirection(choice);
                    }
                }

                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (forward)
                    {
                        if (directions[1])
                        {
                            forward = false;

                            if (directions[0])
                            {
                                ChooseDirection(1);
                            }
                            else
                            {
                                ChooseDirection(0);
                            }
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (forward)
                    {
                        if (directions[0])
                        {
                            forward = false;
                            ChooseDirection(0);
                        }
                    }
                    else
                    {
                        choice--;
                        if (choice < 0)
                        {
                            choice = 0;
                        }
                        ChooseDirection(choice);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!forward)
                    {
                        ChooseCard();
                        ShowPanels(true);
                    }
                }


            }
            else
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    selectedOption++;
                    if (selectedOption >= chosenEncounter.choices.Count)
                    {
                        selectedOption = chosenEncounter.choices.Count - 1;
                    }
                    moveSelect(selectedOption);
                }
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    selectedOption--;
                    if (selectedOption < 0)
                    {
                        selectedOption = 0;
                    }
                    moveSelect(selectedOption);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    selectChoice(chosenEncounter.choices[selectedOption]);
                }
            }
        }

        if (Mathf.Abs(transform.position.x - currentNode.position.x) >= 0 || Mathf.Abs(transform.position.y - currentNode.position.y) >= 0)
        {
            Vector3 driftPosition = new Vector3();
            driftPosition.x = Mathf.Lerp(transform.position.x, currentNode.position.x, Time.deltaTime);
            driftPosition.y = Mathf.Lerp(transform.position.y, currentNode.position.y, Time.deltaTime);
            transform.position = driftPosition;
        }

        if (panelChange)
        {
            float speed = 1f + panelMove;
            if (panelShow)
            {
                speed = 2f - panelMove;
            }

            panelMove += Time.deltaTime * Mathf.Pow(speed, 3f);

            if (panelMove > 1)
            {
                panelMove = 1;
                panelChange = false;
            }

            panelPos = panelMove;

            if (panelShow)
            {
                panelPos = 1 - panelMove;
            }

            Vector3 textPos = imagePanel.transform.localPosition;
            textPos.x = 110 + (panelPos * 140);
            textPanel.transform.localPosition = textPos;

            Vector3 imagePos = imagePanel.transform.localPosition;
            imagePos.x = -110 - (panelPos * 140);
            imagePanel.transform.localPosition = imagePos;

            Vector3 statsPos = statsPanel.transform.localPosition;
            statsPos.y = -60 - (panelPos * 90);
            statsPanel.transform.localPosition = statsPos;
        }
    }

    void changeStats(int healthVal, int coinsVal)
    {
        health += healthVal;
        coins += coinsVal;
        healthCounter.text = "" + health;
        coinsCounter.text = "" + coins;
    }

    void moveSelect(int selected)
    {
        selector.transform.position = cardChoices[selected].transform.position;
        selector.GetComponent<SpriteRenderer>().size = new Vector2((cardChoices[selected].text.Length + 5) * 6f, 18);
    }

    void Progress()
    {
        GenDirection();
        doneGenerating = false;

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
        doneGenerating = true;
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
            new Vector3(newMapNodePosition.x + Random.Range(-3, 3), currentNode.position.y + 40, 0),
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

        ShowPanels(false);
    }

    void ChooseDirection(int optionChoice)
    {
        blockDirections = new List<bool> { false, false, false };

        currentNode = options[optionChoice].transform;
        pointer.transform.position = currentNode.transform.position;
        currentType = options[optionChoice].selfType;
        string typeToSet = "???";
        switch (currentType)
        {
            case NodeType.REGULAR:
                typeToSet = "Clearing";
                break;
            case NodeType.STRUCTURE:
                typeToSet = "Structure";
                break;
            case NodeType.FOREST:
                typeToSet = "Forest";
                break;
            case NodeType.POND:
                typeToSet = "Wetland";
                break;
            case NodeType.CASTLE:
                typeToSet = "The Keep";
                break;
        }
        pointerText.text = typeToSet;

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
        int indexEncounter = 0;
        int counter = 0;

        do
        {
            indexEncounter = Random.Range(0, encountersList.Count);
            chosenEncounter = encountersList[indexEncounter];
            counter++;

            if (counter > 100)
            {
                Debug.Log("Failed to find encounter!");
                break;
            }
        }
        while (!(chosenEncounter.type == currentType && chosenEncounter.seen == false));

        Debug.Log(encountersList[indexEncounter]);
        Debug.Log(indexEncounter);

        encountersList[indexEncounter].seen = true;

        cardImage.sprite = chosenEncounter.cardSprite;
        cardTitle.text = chosenEncounter.cardTitle;
        cardText.text = chosenEncounter.cardText.text;

        for (int choiceIndex = 0; choiceIndex < 4; choiceIndex++)
        {
            if (choiceIndex < chosenEncounter.choices.Count)
            {
                cardChoices[choiceIndex].text = chosenEncounter.choices[choiceIndex].choiceText;
            }
            else
            {
                cardChoices[choiceIndex].text = " ";
            }
        }

        chooseMode = true;
        if (chosenEncounter.choices.Count > 0)
        {
            selectedOption = chosenEncounter.choices.Count - 1;
        }
        moveSelect(selectedOption);
    }

    void clearSelect()
    {
        for (int selectIndex = 0; selectIndex < 4; selectIndex++)
        {
            cardChoices[selectIndex].text = " ";
        }
    }
    
    void selectChoice(choice selectedChoice)
    {
        changeStats(selectedChoice.healthChange, selectedChoice.wealthChange);

        foreach (var stringToAdd in selectedChoice.trackers)
        {
            trackers.Add(stringToAdd);
            checkTracker(stringToAdd);
        }

        clearSelect();
        cardChoices[0].text = "Accept.";
        moveSelect(0);

        colorMode = true;
        fade = true;
    }

    void showText()
    {
        cardText.text = chosenEncounter.choices[selectedOption].resultText.text;
    }

    void checkTracker(string trackerToCheck)
    {

    }

    void ShowPanels(bool show)
    {
        if (show != panelShow)
        {
            panelMove = 0;
            panelShow = show;
            panelChange = true;
        }
    }
}
