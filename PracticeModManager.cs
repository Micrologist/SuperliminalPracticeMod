﻿using FMOD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MelonLoader;

namespace SuperliminalPracticeMod
{
	class PracticeModManager : MonoBehaviour
	{
		public static PracticeModManager Instance;
		public bool noClip;
		public float noClipSpeed;
		public float defaultFarClipPlane;
		public GameObject player;
		public GameObject flashLight;
		public CharacterMotor playerMotor;
		public Camera playerCamera;
		public ResizeScript resizeScript;
		public Text playerText;
		public Text grabbedObejctText;
		public PauseMenu pauseMenu;

		Vector3 storedPosition;
		Quaternion storedRotation;
		float storedScale;
		int storedMap;
		float storeTime;
		float teleportTime;
		bool unlimitedRenderDistance;
		bool debugFunctions;
		bool triggersVisible;
		List<GameObject> triggerGameObjects;


		void Awake()
		{
			PracticeModManager.Instance = this;
			noClip = false;
			unlimitedRenderDistance = false;
			noClipSpeed = 10.0f;
			defaultFarClipPlane = 999f;

			storedPosition = Vector3.zero;
			storedRotation = Quaternion.identity;
			storedScale = 1.0f;
			storedMap = -1;
			GameManager.GM.enableDebugFunctions = true;
			debugFunctions = false;
			GameManager.GM.GetComponent<LevelInformation>().LevelInfo.RandomLoadingScreens = new SceneReference[1] { GameManager.GM.GetComponent<LevelInformation>().LevelInfo.NormalLoadingScreen };
			base.gameObject.AddComponent<SLPMod_Console>();
		}

		private void OnLevelWasLoaded(int level)
		{
			triggerGameObjects = new List<GameObject>();
			triggersVisible = false;
		}

		public void AddTriggerGO(GameObject go)
		{
			triggerGameObjects.Add(go);
		}

		public void ToggleTriggerVisibility()
		{
			if (GameManager.GM.player == null)
				return;

			triggersVisible = !triggersVisible;

			foreach (GameObject gameObject in triggerGameObjects)
			{
				gameObject.SetActive(triggersVisible);
			}

		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F11))
				SLPMod_Console.instance.Toggle();

			if (Input.GetKeyDown(KeyCode.F12))
			{
				Transform performanceGraph = GameManager.GM.transform.Find("[Graphy]");
				if (performanceGraph != null)
					performanceGraph.gameObject.SetActive(!performanceGraph.gameObject.activeSelf);
			}

			GameManager.GM.enableDebugFunctions = debugFunctions;

			if (GameManager.GM.player == null)
				return;

			if (player != GameManager.GM.player)
			{
				player = GameManager.GM.player;
				playerMotor = player.GetComponent<CharacterMotor>();
				playerCamera = player.GetComponentInChildren<Camera>();
				resizeScript = playerCamera.GetComponent<ResizeScript>();
				pauseMenu = GameObject.Find("UI_PAUSE_MENU").GetComponentInChildren<PauseMenu>(true);
				defaultFarClipPlane = playerCamera.farClipPlane;
				if(player.transform.Find("Flashlight") == null)
				{
					flashLight = new GameObject("Flashlight");
					flashLight.SetActive(false);
					this.flashLight.transform.parent = player.transform;
					this.flashLight.transform.localPosition = new Vector3(0f, playerCamera.transform.localPosition.y, 0f);
					Light light = this.flashLight.AddComponent<Light>();
					light.range = 10000f;
					light.intensity = 0.5f;
				}
				else
				{
					flashLight = player.transform.Find("Flashlight").gameObject;
				}

				if (GameObject.Find("PlayerText") == null && GameObject.Find("UI_PAUSE_MENU") != null)
				{
					playerText = NewPlayerText();
				}

				if (GameObject.Find("GrabbedObjectText") == null && GameObject.Find("UI_PAUSE_MENU") != null)
				{
					grabbedObejctText = NewGrabbedObjectText();
				}

				SLPMod_Console.instance.active = false;
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
				noClip = !noClip;
				noClipSpeed = 10.0f;
			}

			playerMotor.enabled = !noClip;

			if (Input.GetKeyDown(KeyCode.F))
			{
				flashLight.gameObject.SetActive(!flashLight.gameObject.activeSelf);
			}

			if (noClip)
			{
				this.noClipSpeed += Input.mouseScrollDelta.y;
				this.noClipSpeed = Mathf.Max(0f, this.noClipSpeed);
				Vector3 directionVector = new Vector3(GameManager.GM.playerInput.GetAxisRaw("Move Horizontal"), 0f, GameManager.GM.playerInput.GetAxisRaw("Move Vertical"));
				if (Input.GetKey(KeyCode.Space))
				{
					directionVector.y += 1f;
				}
				if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl))
				{
					directionVector.y -= 1f;
				}
				playerMotor.transform.Translate(directionVector.normalized * Time.deltaTime * this.noClipSpeed);
				playerCamera.cullingMatrix = new Matrix4x4(Vector4.positiveInfinity, Vector4.positiveInfinity, Vector4.positiveInfinity, Vector4.positiveInfinity);
			}
			
			if(noClip || unlimitedRenderDistance)
			{
				playerCamera.cullingMatrix = new Matrix4x4(Vector4.positiveInfinity, Vector4.positiveInfinity, Vector4.positiveInfinity, Vector4.positiveInfinity);
				playerCamera.GetComponent<CameraSettingsLayer>().enabled = false;
				playerCamera.farClipPlane = 10000f;
			}
			else
			{

				playerCamera.GetComponent<CameraSettingsLayer>().enabled = true;
				playerCamera.ResetCullingMatrix();
			}

			if(playerText != null)
			{
				playerText.text = GetPlayerTextString();
			}

			if(grabbedObejctText != null)
			{
				grabbedObejctText.text = GetGrabbedObjectTextString();
			}

			if (Input.GetKey(KeyCode.F1))
			{
				float newScale = playerMotor.transform.localScale.x + Time.deltaTime * playerMotor.transform.localScale.x;
				playerMotor.transform.localScale = new Vector3(newScale, newScale, newScale);
				GameManager.GM.player.GetComponent<PlayerResizer>().Poke();
			}
			
			if (Input.GetKey(KeyCode.F2))
			{
				float newScale = playerMotor.transform.localScale.x - Time.deltaTime * playerMotor.transform.localScale.x;
				playerMotor.transform.localScale = new Vector3(newScale, newScale, newScale);
				GameManager.GM.player.GetComponent<PlayerResizer>().Poke();
			}

			if (Input.GetKeyDown(KeyCode.F3))
			{
				playerMotor.transform.localScale = Vector3.one;
				GameManager.GM.player.GetComponent<PlayerResizer>().Poke();
			}

			if (Input.GetKeyDown(KeyCode.F4) && !noClip)
				unlimitedRenderDistance = !unlimitedRenderDistance;

			if (Input.GetKeyDown(KeyCode.F5))
				StorePosition();

			if (Input.GetKeyDown(KeyCode.F6))
				TeleportPosition();

			if (Input.GetKeyDown(KeyCode.F7))
				ReloadCheckpoint();

			if (Input.GetKeyDown(KeyCode.F8))
				RestartMap();

			if(Input.GetKeyDown(KeyCode.F9))
			{
				debugFunctions = !debugFunctions;
				GameManager.GM.enableDebugFunctions = debugFunctions;
			}
		}

		public void SetMouseMinY(float mouseMinY)
		{
			if (GameManager.GM.player != null && GameManager.GM.playerCamera.GetComponent<MouseLook>() != null)
				GameManager.GM.playerCamera.GetComponent<MouseLook>().minimumY = (mouseMinY);
		}

		Text NewPlayerText()
		{
			Text newText;
			GameObject gameObject = new GameObject("PlayerText");
			gameObject.transform.parent = GameObject.Find("UI_PAUSE_MENU").transform.Find("Canvas");
			CanvasGroup cg = gameObject.AddComponent<CanvasGroup>();
			cg.interactable = false;
			cg.blocksRaycasts = false;
			newText = gameObject.AddComponent<Text>();
			RectTransform component = newText.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2((float)(Screen.currentResolution.width / 3), (float)(Screen.currentResolution.height / 3));
			component.pivot = new Vector2(0f, 1f);
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(0f, 1f);
			component.anchoredPosition = new Vector2(25f, -25f);
			foreach (Font font in Resources.FindObjectsOfTypeAll<Font>())
			{
				if (font.name == "BebasNeue Bold")
				{
					newText.font = font;
				}
			}
			newText.text = "hello world";
			newText.fontSize = 30;

			return newText;
		}

		Text NewGrabbedObjectText()
		{
			Text newText;
			GameObject gameObject = new GameObject("GrabbedObjectText");
			gameObject.transform.parent = GameObject.Find("UI_PAUSE_MENU").transform.Find("Canvas");
			CanvasGroup cg = gameObject.AddComponent<CanvasGroup>();
			cg.interactable = false;
			cg.blocksRaycasts = false;
			newText = gameObject.AddComponent<Text>();
			RectTransform component = newText.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2((float)(Screen.currentResolution.width / 3), (float)(Screen.currentResolution.height / 3));
			component.pivot = new Vector2(0f, .5f);
			component.anchorMin = new Vector2(0f, .5f);
			component.anchorMax = new Vector2(0f, .5f);
			component.anchoredPosition = new Vector2(25f, -25f);
			foreach (Font font in Resources.FindObjectsOfTypeAll<Font>())
			{
				if (font.name == "BebasNeue Bold")
				{
					newText.font = font;
				}
			}
			newText.text = "hello world";
			newText.fontSize = 30;

			return newText;
		}

		string GetPlayerTextString()
		{
			Vector3 position = playerMotor.transform.localPosition;
			Vector3 velocity = playerMotor.GetComponent<CharacterController>().velocity;
			Vector3 rotation = playerMotor.transform.localRotation.eulerAngles;
			float scale = playerMotor.transform.localScale.x;
			string dynamicInfo = "";

			if (debugFunctions)
				dynamicInfo += "\nDebug Functions";

			if (triggersVisible)
				dynamicInfo += "\nTriggers Visible";

			if (noClip)
				dynamicInfo += "\nNoClip";

			if (!noClip && unlimitedRenderDistance)
				dynamicInfo += "\nUnlimited Render Distance";

			if (flashLight.activeSelf)
				dynamicInfo += "\nFlashlight";

			if (Time.time - this.storeTime <= 1f)
				dynamicInfo += "\nPosition Stored";

			if (Time.time - this.teleportTime <= 1f)
				dynamicInfo += "\nTeleport";

			return string.Concat(new object[]
			{
				"Position: ",
				position.x.ToString("0.000"),
				", ",
				position.y.ToString("0.000"),
				", ",
				position.z.ToString("0.000"),
				"\n",
				"Rotation: ",
				playerCamera.transform.rotation.eulerAngles.x.ToString("0.000"),
				", ",
				rotation.y.ToString("0.000"),
				"\n",
				"Scale: ",
				scale.ToString("0.0000")+"x",
				"\n",
				"Horizontal Velocity: ",
				Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z).ToString("0.000")+" m/s",
				"\n",
				"Vertical Velocity: ",
				velocity.y.ToString("0.000")+" m/s",
				"\n",
				dynamicInfo
			});
		}

		string GetGrabbedObjectTextString()
		{
			if(resizeScript.isGrabbing && resizeScript.GetGrabbedObject() != null)
			{
				GameObject grabbedObject = resizeScript.GetGrabbedObject();
				return string.Concat(new object[]{
					grabbedObject.name+"\n",
					"Position: "+grabbedObject.transform.position.x.ToString("0.000")+", "+grabbedObject.transform.position.y.ToString("0.000")+", "+grabbedObject.transform.position.z.ToString("0.000")+"\n",
					"Scale: "+grabbedObject.transform.localScale.x.ToString("0.0000")+"x"
				});
			}
			else
			{
				return "";
			}
		}

		void StorePosition()
		{
			storedPosition = playerMotor.transform.position;
			storedRotation = playerMotor.transform.rotation;
			storedScale = playerMotor.transform.localScale.x;
			storedMap = SceneManager.GetActiveScene().buildIndex;
			storeTime = Time.time;
		}

		void TeleportPosition()
		{
			if(storedMap == SceneManager.GetActiveScene().buildIndex)
			{
				playerMotor.transform.position = storedPosition;
				playerMotor.transform.rotation = storedRotation;
				playerMotor.transform.localScale = new Vector3(storedScale, storedScale, storedScale);
				teleportTime = Time.time;
			}
		}

		void ReloadCheckpoint()
		{
			GameManager.GM.TriggerScenePreUnload();
			GameManager.GM.GetComponent<SaveAndCheckpointManager>().ResetToLastCheckpoint();
		}

		void RestartMap()
		{
			GameManager.GM.TriggerScenePreUnload();
			GameManager.GM.GetComponent<SaveAndCheckpointManager>().RestartLevel();
		}

		public void Teleport(Vector3 position)
		{
			if (GameManager.GM.player == null)
				return;

			playerMotor.transform.position = position;
		}

		public void Scale(float newScale)
		{
			if (GameManager.GM.player != null && newScale > 0.0001f)
			{
				playerMotor.transform.localScale = new Vector3(newScale, newScale, newScale);
				GameManager.GM.player.GetComponent<PlayerResizer>().Poke();
			}
		}

	}
}
