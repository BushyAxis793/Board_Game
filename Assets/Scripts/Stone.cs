using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{

    public int stoneID;
    [Header("ROUTES")]
    public Route commonRoute;//OUTER ROUTE
    public Route finalRoute;

    public List<Node> fullRoute = new List<Node>();

    [Header("NODES")]
    public Node startNode;
    public Node baseNode;//BODE IN HOME BASE

    public Node currentNode;
    public Node goalNode;

    int routePosition;
    int startNodeIndex;
    int steps;//ROLLED DICE AMOUNT
    int doneSteps;

    [Header("BOOLEANS")]
    public bool isOut;
    bool isMoving;
    bool hasTurn;// IS FOR HUMEN INPUT


    [Header("SELECTOR")]
    public GameObject selector;

    //ARC MOVEMENT
    float amplitude = 0.5f;
    float cTime = 0f;

    private void Start()
    {
        startNodeIndex = commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();
    }

    void CreateFullRoute()
    {
        for (int i = 0; i < commonRoute.childNodeList.Count; i++)
        {
            int tempPos = startNodeIndex + i;
            tempPos %= commonRoute.childNodeList.Count;

            fullRoute.Add(commonRoute.childNodeList[tempPos].GetComponent<Node>());
        }

        for (int i = 0; i < finalRoute.childNodeList.Count; i++)
        {
            fullRoute.Add(finalRoute.childNodeList[i].GetComponent<Node>());
        }
    }


    public void LeaveBase()
    {
        steps = 1;
        isOut = true;
        routePosition = 0;
        //START COUROUTINE
        //StartCoroutine(MoveOut());
    }


    bool MoveToNextNode(Vector3 goalPos, float speed)
    {
        return goalPos != (transform.position = Vector3.MoveTowards(transform.position, goalPos, speed * Time.deltaTime));
    }

    bool MoveInArcToNextNode(Vector3 startPos, Vector3 goalPos, float speed)
    {
        cTime += speed * Time.deltaTime;
        Vector3 myPosition = Vector3.Lerp(startPos, goalPos, cTime);

        myPosition.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);

        return goalPos != (transform.position = Vector3.Lerp(transform.position, goalPos, cTime));
    }

    public bool ReturnIsOut()
    {
        return isOut;
    }
    IEnumerator MoveOut()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        while (steps > 0)
        {
            routePosition++;

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startPos = baseNode.gameObject.transform.position;

            //while (MoveToNextNode(nextPos, 8f)) { yield return null; }
            while (MoveInArcToNextNode(startPos, nextPos, 4f)){ yield return null; }

            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;

        }

        //UPDATE NODE
        goalNode = fullRoute[routePosition];
        //ChECK FOR KICKING OTHER STONE
        if (goalNode.isTaken)
        {
            //RETURN TO START BASE NODE
            goalNode.stone.ReturnToBase();
        }

        currentNode.stone = null;
        currentNode.isTaken = false;

        goalNode.stone = this;
        goalNode.isTaken = true;
        currentNode = goalNode;
        goalNode = null;

        //REPORT BACK TO GAMEMANAGER
        //SWITCH THE PLAYER

    }

    IEnumerator Move(int diceNumber)
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        while (steps > 0)
        {
            routePosition++;

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startPos = fullRoute[routePosition-1].gameObject.transform.position;
            //while (MoveToNextNode(nextPos, 8f)) { yield return null; }
            while (MoveInArcToNextNode(startPos, nextPos, 4f))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;

        }


        //UPDATE NODE
        goalNode = fullRoute[routePosition];
        //ChECK FOR KICKING OTHER STONE
        if (goalNode.isTaken)
        {
            //RETURN TO START BASE NODE
            goalNode.stone.ReturnToBase();
        }

        currentNode.stone = null;
        currentNode.isTaken = false;

        goalNode.stone = this;
        goalNode.isTaken = true;
        currentNode = goalNode;
        goalNode = null;

        if (WinCondition())
        {
            GameManager.instance.ReportWinning();
        }

        if (diceNumber < 6)
        {

            GameManager.instance.state = GameManager.States.SWITCH_PLAYER;

        }
        else
        {
            GameManager.instance.state = GameManager.States.ROLL_DICE;
        }
        isMoving = false;
    }

    public bool CheckPossibleMove(int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if (tempPos >= fullRoute.Count)
        {
            return false;
        }
        return !fullRoute[tempPos].isTaken;
    }

    public bool CheckPossibleKick(int stoneID, int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if (tempPos >= fullRoute.Count)
        {
            return false;
        }
        if (fullRoute[tempPos].isTaken)
        {
            if (stoneID == fullRoute[tempPos].stone.stoneID)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public void StartTheMove(int diceNumber)
    {
        steps = diceNumber;
        StartCoroutine(MoveOut());
    }

    public void ReturnToBase()
    {
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        GameManager.instance.ReportTurnPossible(false);
        routePosition = 0;
        currentNode = null;
        goalNode = null;
        isOut = false;
        doneSteps = 0;

        Vector3 baseNodePosition = baseNode.gameObject.transform.position;
        while (MoveToNextNode(baseNodePosition, 100f))
        {
            yield return null;
        }
        GameManager.instance.ReportTurnPossible(true);
    }

    bool WinCondition()
    {
        for (int i = 0; i < finalRoute.childNodeList.Count; i++)
        {
            if (!finalRoute.childNodeList[i].GetComponent<Node>().isTaken) 
            {
                return false;
            }
        }
        return true;
    }

}
