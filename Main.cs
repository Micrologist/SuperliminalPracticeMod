using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
[assembly: MelonInfo(typeof(SuperliminalPracticeMod.Main), "Superliminal Practice Mod", "0.1.0", "Micrologist#2351")]
[assembly: MelonGame("PillowCastle", "Superliminal")]

namespace SuperliminalPracticeMod
{
    public class Main : MelonMod
    {
        public override void OnLevelWasLoaded(int level)
        {
            if(GameManager.GM != null && GameManager.GM.gameObject.GetComponent<PracticeModManager>() == null)
                GameManager.GM.gameObject.AddComponent<PracticeModManager>();
        }
    }
}
