using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using Harmony;

[assembly: MelonInfo(typeof(SuperliminalPracticeMod.Main), "Superliminal Practice Mod", "0.3.5", "Micrologist#2351")]
[assembly: MelonGame("PillowCastle", "Superliminal")]
[assembly: MelonGame("PillowCastle", "SuperliminalSteam")]
[assembly: MelonGame("PillowCastle", "SuperliminalGOG")]

namespace SuperliminalPracticeMod
{
    public class Main : MelonMod
    {

        public override void OnApplicationStart()
        {
            MelonLogger.Log("Trying to patch");
            SLPMod_Patcher.Patch();
        }
        public override void OnLevelWasLoaded(int level)
        {
            if(GameManager.GM != null && GameManager.GM.gameObject.GetComponent<PracticeModManager>() == null)
                GameManager.GM.gameObject.AddComponent<PracticeModManager>();
        }
    }
}
