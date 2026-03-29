using System;
using UnityEngine;

namespace Save_System
{
    [Serializable]
    public class PlayerData
    {
        public int[] xps;
        
        public PlayerData(PlayerStats playerStats)
        {
            xps = new int[PlayerSkills.Instance.skillsAmount];
            for (int i = 0; i <= xps.Length; i++)
            {
                //xps[i] = PlayerSkills.Instance.GetXP(, i);
            }
        }
    }
}
