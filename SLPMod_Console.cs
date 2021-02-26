using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using UnityEngine.SceneManagement;

namespace SuperliminalPracticeMod
{
	class SLPMod_Console : MonoBehaviour
	{
		public static SLPMod_Console instance;
		public bool active;
		string input;
		

		private void Awake()
		{
			instance = this;
			active = false;
			input = "";
		}
		
		private void OnGUI()
		{
			if (GameManager.GM.player != false && PracticeModManager.Instance.pauseMenu.isInMenu == false)
				GameManager.GM.PM.canControl = !active;

			if (!active)
				return;

			float y = 0f;

			if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
			{
				ParseCommand(input);
				input = "";
				active = false;
				return;
			}

			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F11 || Event.current.keyCode == KeyCode.Escape)
			{
				active = false;
				return;
			}

			GUI.Box(new Rect(0, y, Screen.width, 30), "");
			GUI.backgroundColor = new Color(0, 0, 0, 0);
			GUI.SetNextControlName("SLP_Console");
			input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
			GUI.FocusControl("SLP_Console");
		}

		private void ParseCommand(string command)
		{
			MelonLogger.Log("Trying to parse \"" + command + "\"");
			string[] commandArray = command.Split(' ');
			if(commandArray[0].ToLower() == "teleport" && commandArray.Length >= 4)
			{
				float x, y, z;
				if (!float.TryParse(commandArray[1], out x) || !float.TryParse(commandArray[2], out y) || !float.TryParse(commandArray[3], out z))
					return;
				MelonLogger.Log("Trying to teleport to "+x+", "+y+", "+z);
				PracticeModManager.Instance.Teleport(new Vector3(x, y, z));
			}
			else if(commandArray[0].ToLower() == "scale" && commandArray.Length >= 2)
			{
				float newScale;
				if (float.TryParse(commandArray[1], out newScale))
					PracticeModManager.Instance.Scale(Math.Abs(newScale));
			}
			else if(commandArray[0].ToLower() == "load" && commandArray.Length >= 2)
			{
				int sceneIndex;
				if (!int.TryParse(commandArray[1], out sceneIndex))
					return;
				if(sceneIndex >= 0)
				{
					SceneManager.LoadScene(sceneIndex % SceneManager.sceneCountInBuildSettings);
				}
			}
			else if(commandArray[0].ToLower() == "noclip")
			{
				PracticeModManager.Instance.noClip = !PracticeModManager.Instance.noClip;
			}
			else if (commandArray[0].ToLower() == "showtriggers")
			{
				PracticeModManager.Instance.ToggleTriggerVisibility();
			}

		}

		public void Toggle()
		{
			active = !active;
		}

	}
}
