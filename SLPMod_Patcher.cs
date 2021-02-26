using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace SuperliminalPracticeMod
{
	public class SLPMod_Patcher
	{
		public static void Patch()
		{
			var harmony = HarmonyInstance.Create("com.harmonypatch.test");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	[HarmonyPatch(typeof(VendingMachine))]
	[HarmonyPatch("Start")]
	class VendingMachinePatch
	{
		static void Prefix(VendingMachine __instance)
		{
			MelonLogger.Log("VendingMachine.Start()");
			__instance.HasInfinite = true;
		}
	}

	[HarmonyPatch(typeof(MOSTTriggerEnterCollider))]
	[HarmonyPatch("Start")]
	class TriggerEnterColliderPatch
	{
		static void Prefix(MOSTTriggerEnterCollider __instance)
		{
			Bounds bounds = __instance.gameObject.GetComponent<Collider>().bounds;
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.transform.position = bounds.center;
			gameObject.transform.localScale = bounds.extents * 2f;
			gameObject.GetComponent<BoxCollider>().enabled = false;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.material.shader = Shader.Find("Transparent/Diffuse");
			component.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.3f);
			gameObject.SetActive(false);
			PracticeModManager.Instance.AddTriggerGO(gameObject);
		}
	}

}
