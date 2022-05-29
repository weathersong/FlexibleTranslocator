using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace FlexibleTranslocator
{
	[HarmonyPatch(typeof(BlockEntityStaticTranslocator))]
	public class TranslocatorPatch
	{

		[HarmonyPatch("Initialize")]
		[HarmonyPrefix]
		private static void SetDistances(ICoreAPI api, BlockEntityStaticTranslocator __instance)
		{
			FlexibleTranslocatorMod myMod = api?.ModLoader.GetModSystem<FlexibleTranslocatorMod>();
			if (myMod == null) return;

			BlockPos pos = __instance.Pos;
			myMod.LogDebug($"Harmony patching the static translocator at (X {pos.X} Y {pos.Y} Z {pos.Z}) : Initialize.");

			int min = myMod.Config.MinTeleportRangeInBlocks;
			int max = myMod.Config.MaxTeleportRangeInBlocks;
			myMod.LogDebug($"Setting MinTeleportRangeInBlocks to {min}. Setting MaxTeleportRangeInBlocks to {max}.");
			__instance.MinTeleporterRangeInBlocks = min;
			__instance.MaxTeleporterRangeInBlocks = max;
		}

	}
}
