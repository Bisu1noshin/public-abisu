using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerObjectState 
{
    private int playerHP;
    private int playerATP;
    private int playerMp;
    private int maxHP;
    private int maxMP;

    public PlayerObjectState(int hp,int atp,int mp)
    {
        playerHP = hp;
        playerATP = atp;
        playerHP = mp;
        maxHP = hp;
        maxMP = mp;
    }

    public int GetPlayerHP() { return this.playerHP; }
    public void SubPlayerHP(int atp)
    { 
        if (this.playerHP - atp <= 0) { this.playerHP = 0; return; }

        this.playerHP -= atp;
    }
    public void AddPlayerHP(int hp) 
    {
        if (this.playerHP + hp >= maxHP) { playerHP = maxHP;return; }
        this.playerHP += hp;
    }
    public int GetPlayerAtp() { return this.playerATP; }
    public void SetPlayerAtp(int atp) { this.playerATP = atp; }
    public int GetPlayerMP() { return this.playerMp; }
    public void SubPlayerMP(int mp) 
    {
        if (playerMp - mp <= 0) { mp = 0; return; }
        playerMp -= mp;
    }
    public void AddPlayerMP(int mp) 
    {
        if (playerMp + mp >= maxMP) { playerMp = maxMP; }

        playerMp += mp;
    }
}
