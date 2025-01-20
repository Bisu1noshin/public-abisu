using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectState 
{
    private int Hp;
    private int Atp;
    private int MoveCnt;

    public GameObjectState(int hp, int atp)
    {
        Hp = hp;
        Atp = atp;
        MoveCnt = 0;
    }

    public int GetMoveCnt()
    {
        return this.MoveCnt;
    }
    public void SetATP(int atp)
    {
        this.Atp = atp;
    }
    public int GetAtp()
    {
        return this.Atp;
    }
    public void SetHP(int hp)
    {
        this.Hp = hp;
    }
    public void SubHP(int atp)
    {
        if (this.Hp - atp <= 0) { this.Hp = 0; return; }

        this.Hp -= atp;

    }
    public int GetHP()
    {
        return this.Hp;
    }
    public void AddMoveCnt()
    {
        MoveCnt++;
    }
}
