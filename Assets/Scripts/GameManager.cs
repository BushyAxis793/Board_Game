using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.Serializable]
    public class Entity
    {
        public string playerName;
        public Stone[] myStones;
        public bool hasTurn;
        public enum PlayerTypes
        {
            HUMAN,
            CPU,
            NO_PLAYER
        }

        public PlayerTypes playerType;
        public bool hasWon;
    }

    public List<Entity> playerList = new List<Entity>();

    //STATEMACHINE

    public enum States
    {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER
    }

    public States state;

    public int activePlayer;
    bool switchingPlayer;

    //HUMAN INPUTS
    //GAMEOBJECT FOR OUR BUTTON
    //int rolledHumanDice;


    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (playerList[activePlayer].playerType == Entity.PlayerTypes.CPU)
        {
            switch (state)
            {
                case States.WAITING:
                    {
                        //INDLE
                    }
                    break;
                case States.ROLL_DICE:
                    {
                        StartCoroutine(RollDiceDelay());
                        state = States.WAITING;
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        StartCoroutine(SwitchPlayer());
                        state = States.WAITING;

                    }
                    break;
            }

        }
    }

    void RollDice()
    {
        int diceNumber = Random.Range(1, 7);
        

        if (diceNumber == 6)
        {
            //check the start node
            CheckStartNode(diceNumber);
        }

        if (diceNumber < 6)
        {
            //checkfor kick
            MoveAStone(diceNumber);
        }
        Debug.Log("dice rolled number " + diceNumber);
    }

    IEnumerator RollDiceDelay()
    {
        yield return new WaitForSeconds(2);
        RollDice();
    }

    void CheckStartNode(int diceNumber)
    {
        //IS ANYONE ON THE START NODE
        bool startNodeFull = false;
        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startNodeFull = true;
                break;//WE ARE DONE HERE WE FOUND A MATCH
            }
        }

        if (startNodeFull)
        {
            //MOVE A STONE
            MoveAStone(diceNumber);
            Debug.Log("Start Node is full");
        }
        else //START NODE IS EMPTY
        {
            //IF AT LEAST IS INSIDE THE BASE

            for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
            {
                if (playerList[activePlayer].myStones[i].ReturnIsOut())
                {

                    //LEAVE THE BASE
                    playerList[activePlayer].myStones[i].LeaveBase();
                    state = States.WAITING;
                    return;
                }


            }
            //MOVE A STONE
            MoveAStone(diceNumber);
        }
    }

    void MoveAStone(int diceNumber)
    {
        List<Stone> movableStones = new List<Stone>();
        List<Stone> movableKickStones = new List<Stone>();

        //FILL THE LISTS
        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                //CHECK FOR POSSIBLE KICK
                if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID, diceNumber))
                {
                    movableKickStones.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }
                //CHECK FOR POSSIBLE MOVE
                if (playerList[activePlayer].myStones[i].CheckPossibleMove(diceNumber))
                {
                    movableStones.Add(playerList[activePlayer].myStones[i]);

                }

            }

        }

        //PERFORM KICK IF POSSIBLE
        if (movableKickStones.Count > 0)
        {
            int num = Random.Range(0, movableKickStones.Count);
            movableKickStones[num].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }
        //PERFORM MOVE IF POSSIBLE
        if (movableStones.Count > 0)
        {
            int num = Random.Range(0, movableStones.Count);
            movableStones[num].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }
        //NONE IS POSSIBLE

        //SWITCHING PLAYER
        state = States.SWITCH_PLAYER;
    }

    IEnumerator SwitchPlayer()
    {
        if (switchingPlayer)
        {
            yield break;
        }

        switchingPlayer = true;

        yield return new WaitForSeconds(2);
        //SET NEXT PLAYER
        SetNextActivePlayer();

        switchingPlayer = false;

    }

    void SetNextActivePlayer()
    {
        activePlayer++;
        activePlayer %= playerList.Count;

        int available = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playerList[i].hasWon)
            {
                available++;
            }
        }

        if (playerList[activePlayer].hasWon && available > 1)
        {
            SetNextActivePlayer();
            return;
        }
        else if (available <2)
        {
            //GAME OVER SCREEN
            state = States.WAITING;
            return;
        }

        state = States.ROLL_DICE;
    }



}
