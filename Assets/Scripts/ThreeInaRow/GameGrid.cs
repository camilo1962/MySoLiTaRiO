using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    const int GRIDSIZE = 6;

    int GEM_VALUE = 10;

    gameTile current;


    [SerializeField]
    gameTile[] gameGrid;

    [SerializeField]
    Score score;

    [SerializeField]
    GameOver gameover;

    [SerializeField]
    GameObject noMoves;

    [SerializeField]
    Timer timer;

    [SerializeField]
    GameObject destroyEffect;

    [SerializeField]
    List<Sprite> backTileSprites;

    [SerializeField]
    AudioSource gemSwap;

    [SerializeField]
    AudioSource noMatch;

    [SerializeField]
    AudioSource gemPop;

    bool canClick = true;
    bool checking = true;

    bool paused = false;
    // Start is called before the first frame update
    void Awake()
    {
        timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<Score>();
        
        var tiles = GetComponentsInChildren<gameTile>();

        gameGrid = tiles;

        for (int i = 0; i < gameGrid.Length; i++)
        {
            gameGrid[i].setIndex(i);
        }

        /*
        bool noMatches = setUp();
        if (!noMatches)
        {
            print("error");
        } 
        */
    }

    void Start()
    {
        setUp();   
    }

    // Update is called once per frame
    void Update()
    {
        if (!timer.getRunning() && !paused)
        {
            endOfGame();
        }
    }

    IEnumerator NoMatchCoroutine(int currIndex, int upIndex)
    {
        canClick = false;
        current = null;
        checking = false;

        Gem currGem = gameGrid[currIndex].getGem();
        Gem upGem = gameGrid[upIndex].getGem();

        Transform currGemTrans = gameGrid[currIndex].getGemTransform();
        Transform upGemTrans = gameGrid[upIndex].getGemTransform();

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.5f);

        currGemTrans.parent = gameGrid[upIndex].gameObject.transform;
        upGemTrans.parent = gameGrid[currIndex].gameObject.transform;

        gameGrid[upIndex].setGem(currGem);
        gameGrid[currIndex].setGem(upGem);

        noMatch.Play();

        canClick = true;
        checking = true;
    }

    IEnumerator MoveDownCoroutine()
    {
        canClick = false;
        checking = false;
        yield return new WaitForSeconds(0.9f);
        gemPop.Play();
        moveDown();

        yield return new WaitForSeconds(0.9f);
        


        checking = true;
        HashSet<int> matches = checkBoard(false);
        while (matches.Count != 0)
        {
            checking = false;
            gemSwap.Play();
            yield return new WaitForSeconds(0.9f);
            gemPop.Play();
            moveDown();
            yield return new WaitForSeconds(0.9f);
            checking = true;
            matches = checkBoard(false);   
        }

        checking = true;
        canClick = true;
        bool moreMoves = checkAnyMoves();
        if (!moreMoves)
            handleNoMoves();
    }

    void endOfGame()
    {
        gameover.setScore(score.getScore());
        gameover.activate(true);
    }

    public void setCurrent(gameTile update)
    {
        if (current != null)
        {
            int currIndex = current.getIndex();
            int upIndex = update.getIndex();

            bool isAdjacent = false;

            if ((upIndex == currIndex + 1 && upIndex % GRIDSIZE != 0) ||  //to right
                 (upIndex == currIndex - 1 && upIndex % GRIDSIZE != 5) ||  //to left
                 (upIndex == currIndex - GRIDSIZE || upIndex == currIndex + GRIDSIZE))  //top or bottom
            {
                isAdjacent = true;
                current.setSelected(false);
                current = null;
            }

            if (!isAdjacent)
            {
                current.setSelected(false);
                current = update;
                current.setSelected(true);
            } else
            {
                Gem currGem = gameGrid[currIndex].getGem();
                Gem upGem = gameGrid[upIndex].getGem();

                Transform currGemTrans = gameGrid[currIndex].getGemTransform();
                Transform upGemTrans = gameGrid[upIndex].getGemTransform();

                currGemTrans.parent = gameGrid[upIndex].gameObject.transform;
                upGemTrans.parent = gameGrid[currIndex].gameObject.transform;

                gemSwap.Play();

                gameGrid[upIndex].setGem(currGem);
                gameGrid[currIndex].setGem(upGem);

                HashSet<int> matches = checkBoard(false);
                if (matches.Count == 0)
                    StartCoroutine(NoMatchCoroutine(currIndex, upIndex));
                else
                {
                    StartCoroutine(MoveDownCoroutine());
                }
              
            }
        } 
        else
        {
            current = update;
            current.setSelected(true);
        }
    }

    HashSet<int> checkBoard(bool setup)
    {
        if (checking)
        {
            HashSet<int> indices = new HashSet<int>();
            for (int i = 0; i < gameGrid.Length; i++)
            {

                if (i - GRIDSIZE * 2 <= GRIDSIZE * GRIDSIZE && i < 24)
                {
                    bool down = checkDown(i);
                    if (down)
                    {
                        gameGrid[i].setBg(backTileSprites[1]);
                        gameGrid[i + GRIDSIZE].setBg(backTileSprites[1]);
                        gameGrid[i + GRIDSIZE * 2].setBg(backTileSprites[1]);

                        indices.Add(i);
                        indices.Add(i + GRIDSIZE);
                        indices.Add(i + GRIDSIZE * 2);
                    }
                }
                if ((i + 1) % GRIDSIZE != 0 &&  (i + 2) % GRIDSIZE != 1 &&
                    (i + 1) % GRIDSIZE != 5 && (i + 2) % GRIDSIZE != 6 &&
                    i < 34)
                {

                    bool right = checkRight(i);
                    if (right)
                    {
                        gameGrid[i].setBg(backTileSprites[1]);
                        gameGrid[i + 1].setBg(backTileSprites[1]);
                        gameGrid[i + 2].setBg(backTileSprites[1]);

                        indices.Add(i);
                        indices.Add(i + 1);
                        indices.Add(i + 2);
                    }
                }

            }

            if (!setup)
            {
                removeBlocks(indices);
            }
            return indices;
        }

        return new HashSet<int>();
    }

    bool checkDown(int index)
    {
        int val1 = gameGrid[index].getGemVal();
        int val2 = gameGrid[index + GRIDSIZE].getGemVal();
        int val3 = gameGrid[index + GRIDSIZE * 2].getGemVal();

        return val1 == val2 && val1 == val3;
    }

    bool checkRight(int index)
    {
        int val1 = gameGrid[index].getGemVal();
        int val2 = gameGrid[index + 1].getGemVal();
        int val3 = gameGrid[index + 2].getGemVal();

        return val1 == val2 && val1 == val3;
    }

    void removeBlocks(HashSet<int> indices)
    {

        HashSet<int>.Enumerator indexEnum = indices.GetEnumerator();

        while (indexEnum.MoveNext())
        {
            int current = indexEnum.Current;
            var pos = gameGrid[current].transform.position;
            var newPos = new Vector3(pos.x, pos.y, 13);
            GameObject particle = Instantiate(destroyEffect, newPos, Quaternion.identity);
            Destroy(particle, 0.5f);
            score.addToScore(GEM_VALUE);
            timer.addToTime(1);
            gameGrid[current].deleteGem();
            
        }
    }

    void moveDown()
    {
        int[] heights = new int[6] { 0, 0, 0, 0, 0, 0 };

        for (int i = 35; i >= 0; i--)
        {
            int height = 0;
            int index = i % GRIDSIZE;

            if (gameGrid[i].getGem() == null)
            {
                gameGrid[i].setBg(backTileSprites[0]);
                int findUp = i - GRIDSIZE;
                while (findUp >= 0)
                {
                    height++;
                    if (gameGrid[findUp].getGem() != null && gameGrid[findUp].getGemTransform() != null)
                    {

                        Gem foundGem = gameGrid[findUp].getGem();
                        Transform foundTrans = gameGrid[findUp].getGemTransform();
                        //Transform foundGem 

                        foundTrans.parent = gameGrid[i].gameObject.transform;
                        gameGrid[i].setGem(foundGem);

                        //gameGrid[i].getGemTransform().position = ;
                        gameGrid[findUp].setGem(null);
                        break;
                    }
                    findUp -= GRIDSIZE;
                }

                if (gameGrid[i].getGem() == null) //no gems available above
                {
                    if (heights[index] == 0 && height != 0)
                        heights[index] = height;
                    gameGrid[i].randomGem(false, heights[index]);
                }
            }
        }

    }

    bool setUp() //prevents initiating with matches
    {
        try
        {
            HashSet<int> indices = checkBoard(true);
            bool hasMatches = indices.Count != 0;

            while (hasMatches) //has matches
            {
                HashSet<int>.Enumerator indexEnum = indices.GetEnumerator();

                while (indexEnum.MoveNext())
                {
                    int current = indexEnum.Current;
                    if (gameGrid[current] != null)
                    {
                        gameGrid[current].setBg(backTileSprites[0]);
                        gameGrid[current].randomGem(true, 0);
                    }
                }
                hasMatches = checkBoard(true).Count != 0;
            }
        } catch (Exception e)
        {
            print("error");
            return false;
        }
        if (paused)
            paused = false;
        return true;
    }

    public bool getCanClick()
    {
        return canClick;
    }

    public void resetGame()
    {
        timer.resetTime();
        score.resetScore();
        resetBoard();
    }

    public void resetBoard()
    {
        for (int i = 0; i < gameGrid.Length; i++)
        {
            gameGrid[i].setBg(backTileSprites[0]);
            gameGrid[i].randomGem(true, 0);
        }

        setUp();
    }

    bool checkAnyMoves()
    {
        bool noMoves = false;
        gameTile[] checkGrid = new gameTile[36];

        //CHECK HORIZONTAL SWAPS
        Array.Copy(gameGrid, 0, checkGrid, 0, checkGrid.Length);
        for (int i = 35; i >= 0; i--)
        {
            if (i % GRIDSIZE >= 5)
                continue;

            gameTile temp = checkGrid[i];
            checkGrid[i] = checkGrid[i+1];
            checkGrid[i+1] = temp;

            //only need to check this row if swapping horizontally
            int rowNum = getRowNum(i);
            bool horiMove = checkHori(rowNum, checkGrid);
            if (horiMove) return true;

            //for i column
            bool vertMove = checkVert(i%6, checkGrid);

            //for i+1 column
            bool vertMove2 = checkVert((i+1) % 6, checkGrid);

            if (vertMove || vertMove2) return true;

            //SWAP BACK AFTER
            gameTile temp2 = checkGrid[i];
            checkGrid[i] = checkGrid[i+1];
            checkGrid[i+1] = temp2;
        }

        //CHECK VERTICAL SWAPS
        Array.Copy(gameGrid, 0, checkGrid, 0, checkGrid.Length);
        for (int i = 35; i >= 0; i--)
        {
            if (i > 29)
                continue;

            gameTile temp = checkGrid[i];
            checkGrid[i] = checkGrid[i + GRIDSIZE];
            checkGrid[i+GRIDSIZE] = temp;

            //only need to check this column if swapping vertically
            bool vertMove = checkVert(i % 6, checkGrid);

            if (vertMove) return true;
            
            int rowNum = getRowNum(i);

            if (i % GRIDSIZE < 5)
            {
                bool horiMove = checkHori(rowNum, checkGrid);
                if (rowNum < 30)
                {
                    bool horiMove2 = checkHori(rowNum+6, checkGrid);
                    if (horiMove || horiMove2) return true;
                }
                if (horiMove) return true;
            }

            //SWAP BACK AFTER
            gameTile temp2 = checkGrid[i];
            checkGrid[i] = checkGrid[i + GRIDSIZE];
            checkGrid[i + GRIDSIZE] = temp2;
        }

        return noMoves;
    }

    int getRowNum(int i)
    {
        int rowNum = 0;
        int[] rowStarts = new int[GRIDSIZE] { 0, 6, 12, 18, 24, 30 };

        if (i < 6)
        {
            rowNum = rowStarts[0];
        }
        else if (i < 12)
        {
            rowNum = rowStarts[1];
        }
        else if (i < 18)
        {
            rowNum = rowStarts[2];
        }
        else if (i < 24)
        {
            rowNum = rowStarts[3];
        }
        else if (i < 30)
        {
            rowNum = rowStarts[4];
        }
        else
        {
            rowNum = rowStarts[5];
        }

        return rowNum;
    }

    bool checkVert(int column, gameTile[] board)
    {
        int count = 1;
        int x = 0;
        int y = 1;

        int[] indices = new int[GRIDSIZE];
        for (int i = 0; i < GRIDSIZE; i++)
        {
            indices[i] = column + (i * GRIDSIZE);
        }

        while (x < indices.Length)
        {
            if (board[indices[x]].getGemVal() == board[indices[y]].getGemVal())
            {
                y++;
                count++;
                if (count == 3) return true;
            } else
            {
                count = 1;
                x++;
                y = x + 1;
                if (!(y < indices.Length - 1))
                    break;
            }
        }

        return false;
    }

    bool checkHori(int row, gameTile[] board)
    {
        int count = 1;
        int x = row;
        int y = row+1;

        while (x < (row + GRIDSIZE))
        {
            if (board[x].getGemVal() == board[y].getGemVal())
            {
                y++;
                if (y >= row + GRIDSIZE)
                    break;
                count++;
                if (count == 3) return true;
            } else
            {
                count = 1;
                x++;
                y = x + 1;
                if (y >= row + GRIDSIZE)
                    break;
            }
        }

        return false;
    }

    void handleNoMoves()
    {
        paused = true;
        timer.pause();
        noMoves.SetActive(true);
    }

}
