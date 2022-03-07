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

    public GameObject musicObject;
    public List<Encounter> encountersList;
    public List<AudioClip> availableTracks;
    private List<string> currentlyPlaying = new List<string>();
    private List<musicController> currentMusic = new List<musicController>();
    private  List<string> trackers = new List<string> { };

    public Gradient[] lineGrad;

    private int journeyCount = 0;
    private int journeyLength = 10;
    private int waterStart = 5;
    public List<MapNode> options;
    private NodeType currentType;
    private int choice = 0;
    public Transform currentNode;
    private bool doneGenerating = true;

    private List<bool> directions = new List<bool>{false, false, false};
    private List<bool> blockDirections = new List<bool> { false, false, false };

    public List<GameObject> inventoryItems;
    private List<GameObject> currentInventory = new List<GameObject> { };
    public Transform inventoryContainer;

    private Dictionary<NodeType, int> amountPerNode;

    public GameObject textPanel;
    public GameObject imagePanel;
    public GameObject statsPanel;
    public GameObject pointer;
    public TextMeshPro pointerText;
    public TextMeshPro underPointerText;

    private Encounter chosenEncounter;

    public SpriteRenderer cardImage;
    public TextMeshPro cardTitle;
    public TextMeshPro cardText;
    public List<TextMeshPro> cardChoices;
    public GameObject selector;
    public SpriteRenderer selectorSprite;

    private bool panelChange;
    private bool panelShow;
    private float panelMove;
    private float panelPos;

    private bool forward = true;
    private bool chooseMode = false;
    private int selectedOption = 0;
    private int holdOption;
    private bool colorMode = false;

    public int health;
    public int coins;
    public TextMeshPro healthCounter;
    public TextMeshPro coinsCounter;
    private bool fade;
    private bool secFade;
    private float alpha = 1;
    private float secAlpha = 1;
    private Color colorHold;
    private Color secColorHold;
    public Color selectedColor;
    public Color chosenColor;
    private bool changeComplete;
    private int shiftIndex = 0;

    private List<choice> availableChoices = new List<choice> { };
    private choice currentChoice;
    public riseAndDisappear healthRise;
    public riseAndDisappear wealthRise;

    public bool checkInv;

    void Start()
    {
        for (int i = 0; i < availableTracks.Count; i++)
        {
            GameObject newTrack = GameObject.Instantiate(musicObject, transform);
            musicController newMusic = newTrack.GetComponent<musicController>();
            newMusic.playClip(availableTracks[i]);
            newMusic.setVolume(0, 0.1f);
            newTrack.name = availableTracks[i].name.ToLower();
            currentMusic.Add(newMusic);
            currentlyPlaying.Add(availableTracks[i].name.ToLower());
        }

        Vector3 textPos = textPanel.transform.position;
        textPanel.transform.position = new Vector3(250, textPos.y, textPos.z);

        Vector3 imagePos = imagePanel.transform.position;
        imagePanel.transform.position = new Vector3(-250, imagePos.y, imagePos.z);

        Vector3 statsPos = statsPanel.transform.position;
        statsPanel.transform.position = new Vector3(statsPos.x, -150, statsPos.z);

        amountPerNode = new Dictionary<NodeType, int>()
        {
            { NodeType.CLEARING, 3},
            { NodeType.STRUCTURE, 2},
            { NodeType.FOREST, 3},
            { NodeType.WETLAND, 1}
        };

        selectorSprite = selector.GetComponent<SpriteRenderer>();

        colorHold = cardText.color;
        secColorHold = selectorSprite.color;

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
                        alpha -= Time.deltaTime * 3f;

                        if (alpha < 0.4f)
                        {
                            secFade = true;
                        }
                    }
                    else
                    {
                        alpha = 0;
                        showText();
                        fade = false;
                    }
                }
                else
                {
                    if (alpha < 1)
                    {
                        alpha += Time.deltaTime * 1.5f;
                    }
                    else
                    {
                        alpha = 1;
                    }
                }
                colorHold.a = alpha;
                cardText.color = colorHold;

                if (secFade)
                {
                    if (secAlpha > 0)
                    {
                        secAlpha -= Time.deltaTime * 3f;
                    }
                    else
                    {
                        secAlpha = 0;
                        showChoices();
                        secFade = false;
                    }
                }
                else if (!secFade && !fade)
                {
                    if (secAlpha < 1)
                    {
                        secAlpha += Time.deltaTime * 1.5f;
                    }
                    else
                    {
                        secAlpha = 1;
                        changeComplete = true;
                    }
                }

                secColorHold.a = secAlpha;
                chosenColor.a = secAlpha;
                selectedColor.a = secAlpha;

                for (int ind = 0; ind < 4; ind++)
                {
                    if (ind != holdOption)
                    {
                        cardChoices[ind].color = secColorHold;
                    }
                }

                if (secFade || fade)
                {
                    cardChoices[holdOption].color = chosenColor;
                    selectorSprite.color = chosenColor;
                }
                else
                {
                    cardChoices[holdOption].color = selectedColor;
                    selectorSprite.color = selectedColor;
                }

                if (changeComplete)
                {
                    if (!currentChoice.subChoice)
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            healthRise.completeRise();
                            wealthRise.completeRise();

                            colorMode = false;
                            chooseMode = false;
                            Progress();
                            forward = true;
                        }
                    }
                    else
                    {
                        colorMode = false;

                    }
                }
            }
            else if (!chooseMode)
            {
                if (!checkInv)
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

                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    checkInv = !checkInv;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    selectedOption++;
                    if (selectedOption >= 4)
                    {
                        selectedOption = 3;
                    }
                    moveSelect(selectedOption);
                }
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    selectedOption--;
                    if (selectedOption < shiftIndex)
                    {
                        selectedOption = shiftIndex;
                    }
                    moveSelect(selectedOption);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //Debug.Log("Selected Option : " + (selectedOption - shiftIndex));
                    cardChoices[selectedOption - shiftIndex].color = chosenColor;
                    selectChoice(availableChoices[selectedOption - shiftIndex]);
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
            textPos.x = 100 + (panelPos * 140);
            textPanel.transform.localPosition = textPos;

            Vector3 imagePos = imagePanel.transform.localPosition;
            imagePos.x = -100 - (panelPos * 140);
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
        holdOption = selected;

        foreach(var choiceCard in cardChoices)
        {
            choiceCard.color = secColorHold;
        }

        cardChoices[selected].color = selectedColor;
        selectorSprite.color = selectedColor;

        selector.transform.position = cardChoices[selected].transform.position;
        float width = (cardChoices[selected].text.Length + 6) *5f;
        if (width > 175) 
        {
            width = 175;
        }

        selector.GetComponent<SpriteRenderer>().size = new Vector2(width, 18);
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

        if(journeyCount == (journeyLength - 2))
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
        if (journeyCount > (journeyLength - 1))
            return null; // TODO

        GameObject newNode = GameObject.Instantiate(newMapNodeObject);
        newNode.transform.position = newMapNodePosition;
        newNode.transform.parent = scrollable;
        MapNode newMapNode = newNode.GetComponent<MapNode>();
        options.Add(newMapNode);
        newMapNode.direction = dirCount;

        float RATIO_CHANCE_REG = 0.50f;
        NodeType nodeType = NodeType.CLEARING;

        if(journeyCount == 0)
        {
            nodeType = NodeType.CLEARING;
        }
        else if(journeyCount == (journeyLength - 2))
        {
            nodeType = NodeType.CASTLE;
        }
        else
        {
            do
            {
                //4 is exclusive, so doesnt include castle
                float chance = Random.Range(0.0f, 1.0f);
                int lastOption = (journeyCount <= (waterStart - 1)) ?  3 : 4;
                nodeType = chance <= RATIO_CHANCE_REG ? nodeType = NodeType.CLEARING : (NodeType)Random.Range(1, lastOption);
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
            case NodeType.CLEARING:
                typeToSet = "Clearing";
                break;
            case NodeType.STRUCTURE:
                typeToSet = "Structure";
                break;
            case NodeType.FOREST:
                typeToSet = "Forest";
                break;
            case NodeType.WETLAND:
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

        bool breakCondition = true;

        Encounter checkEncounter;

        do 
        {
            bool satisfiesPreTrack = true;

            indexEncounter = Random.Range(0, encountersList.Count);
            checkEncounter = encountersList[indexEncounter];

            if (checkEncounter.preTrackers.Count > 0)
            {
                foreach (string track in checkEncounter.preTrackers)
                {
                    if (track.ToLower().StartsWith("not"))
                    {
                        if (trackers.Contains(track.ToLower().Substring(3)))
                        {
                            satisfiesPreTrack = false;
                        }
                    }
                    else if (!trackers.Contains(track.ToLower()))
                    {
                        satisfiesPreTrack = false;
                    }
                }
            }

            if (checkEncounter.type == currentType && satisfiesPreTrack)
            {
                chosenEncounter = checkEncounter;
                //Debug.Log("Found Encounter : " + chosenEncounter);
                breakCondition = false;
                break;
            }

            counter++;

            if (counter > 500)
            {
                Debug.Log("Failed to find encounter!");
                chosenEncounter = encountersList[0];
                break;
            }
        }
        while(breakCondition);

        //Debug.Log(encountersList[indexEncounter]);
        //Debug.Log(indexEncounter);
        if (!breakCondition)
        {
            encountersList.RemoveAt(indexEncounter);
        }
        //encountersList[indexEncounter].seen = true;

        cardImage.sprite = chosenEncounter.cardSprite;
        cardTitle.text = chosenEncounter.cardTitle;
        cardText.text = chosenEncounter.cardText.text;

        generateSelect(chosenEncounter.choices);
    }

    void generateSelect(List<choice> choiceListInput)
    {
        List<string> choicesText = new List<string> { };

        clearSelect();
        availableChoices.Clear();

        for (int choiceIndex = 0; choiceIndex < 4; choiceIndex++)
        {
            if (choiceIndex < choiceListInput.Count)
            {
                bool missingTrackers = false;

                foreach (string track in choiceListInput[choiceIndex].preTrackers)
                {
                    if (!trackers.Contains(track.ToLower()))
                    {
                        //Debug.Log("Missing Tracker : " + track);
                        missingTrackers = true;
                    }
                }

                if (!missingTrackers)
                {
                    availableChoices.Add(choiceListInput[choiceIndex]);
                    choicesText.Add(choiceListInput[choiceIndex].choiceText);
                }
            }
        }

        shiftIndex = 4 - choicesText.Count;

        for (int textIndex = 0; textIndex < choicesText.Count; textIndex++)
        {
            cardChoices[textIndex + shiftIndex].text = choicesText[textIndex];
        }

        chooseMode = true;

        selectedOption = shiftIndex;
        holdOption = shiftIndex;
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
        currentChoice = selectedChoice;

        int healVal = selectedChoice.healthChange;
        int wealVal = selectedChoice.wealthChange;
        changeStats(healVal, wealVal);

        healthRise.initiateRise(selectedChoice.healthChange, false);
        wealthRise.initiateRise(selectedChoice.wealthChange, false);

        /*
        if (healVal == 0 || wealVal == 0)
        {
            //Debug.Log("health/wealth value are : " + healVal + wealVal);

            healthRise.initiateRise(selectedChoice.healthChange, wealVal == 0);
            wealthRise.initiateRise(selectedChoice.wealthChange, healVal == 0);
        }
        else
        {
            //Debug.Log("health/wealth value are : " + healVal + wealVal);

            healthRise.initiateRise(selectedChoice.healthChange, false);
            wealthRise.initiateRise(selectedChoice.wealthChange, false);
        }
        */

        foreach (var stringToAdd in selectedChoice.trackers)
        {
            checkTracker(stringToAdd.ToLower());
        }

        //clearSelect();
        
        changeComplete = false;

        colorMode = true;
        fade = true;
    }

    void showText()
    {
        cardText.text = availableChoices[selectedOption - shiftIndex].resultText.text;

        //availableChoices.Clear();
    }
        
    void showChoices()
    {
        clearSelect();

        if (!currentChoice.subChoice)
        {
            cardChoices[3].text = "Accept.";
            moveSelect(3);
        }
        else
        {
            generateSelect(currentChoice.subChoices);
        }
    }

    void checkTracker(string trackerToCheck)
    {
        if (trackerToCheck.StartsWith("getinv"))
        {
            GameObject invItem = null;
            foreach (var invInList in inventoryItems)
            {
                if (invInList.name.ToLower() == trackerToCheck.Substring(3))
                {
                    invItem = invInList;
                    //Debug.Log("Inventory item was found : " + trackerToCheck.Substring(3));
                    break;
                }
            }
            if (invItem == null)
            {
                Debug.Log("Inventory item was not found : " + trackerToCheck.Substring(3));
            }
            else
            {
                GameObject newInv = GameObject.Instantiate(invItem, inventoryContainer);
                newInv.name = trackerToCheck.Substring(3);
                currentInventory.Add(newInv);
                trackers.Add(trackerToCheck.Substring(3));
            }
        }
        else if (trackerToCheck.StartsWith("reminv"))
        {
            bool foundInv = false;

            foreach (var invToCheck in currentInventory)
            {
                if (invToCheck.name == trackerToCheck.Substring(3))
                {
                    currentInventory.Remove(invToCheck);

                    trackers.Remove(invToCheck.name);

                    Destroy(invToCheck);
                    foundInv = true;

                    break;
                }
            }
            if (!foundInv)
            {
                Debug.Log("Inventory item was not found : " + trackerToCheck.Substring(3));
            }
        }
        else if (trackerToCheck.StartsWith("addmus"))
        {
            playMusic(trackerToCheck.Substring(6), 0.1f);
        }
        else if (trackerToCheck.StartsWith("remmus"))
        {
            playMusic(trackerToCheck.Substring(6), 0);
        }
        else
        {
            trackers.Add(trackerToCheck);
        }
    }

    void playMusic(string audioName, float volume)
    {
        bool musicFound = false;

        for (int i = 0; i < currentMusic.Count; i++)
        {
            if (currentMusic[i].name.ToLower() == audioName)
            {
                Debug.Log("Set track " + currentMusic[i] + " to Volume " + volume);

                musicFound = true;

                if (volume == 0)
                {
                    currentMusic[i].setVolume(0, 0.1f);
                    //currentMusic[i].terminate();
                    //currentMusic.RemoveAt(i);
                    //currentlyPlaying.RemoveAt(i);
                }
                else
                {
                    currentMusic[i].setVolume(volume, 0.1f);
                }
                break;
            }
        }

        if (!musicFound)
        {
            AudioClip newClip = null;

            for (int i = 0; i < availableTracks.Count; i++)
            {
                if (availableTracks[i].name.ToLower() == audioName)
                {
                    newClip = availableTracks[i];
                    break;
                }
            }

            if (newClip != null)
            {
                GameObject newTrack = GameObject.Instantiate(musicObject, transform);
                musicController newMusic = newTrack.GetComponent<musicController>();
                newMusic.playClip(newClip);
                newMusic.setVolume(0.6f, 0.1f);
                newTrack.name = audioName;
                currentMusic.Add(newMusic);
                currentlyPlaying.Add(audioName);
            }
            else
            {
                Debug.Log("Could not find track : " + audioName);
            }
        }
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
