using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace FlexibleTranslocator
{
    public class FlexibleTranslocatorMod : ModSystem
    {

		const string LogHeader = "FLEXIBLE_TRANSLOCATOR_1_0";
		const string ConfigFilename = "FlexibleTranslocatorConfig.json";

		ICoreAPI CoreApi;
		ICoreServerAPI ServerApi;
		ICoreClientAPI ClientApi;

		private Harmony Harmony;

		public FlexibleTranslocatorConfig Config;

		#region STARTUP

		public override void StartPre(ICoreAPI api)
		{
			base.StartPre(api);

			CoreApi = api;
			Config = new FlexibleTranslocatorConfig();

			LoadConfig();
		}

		public override void Start(ICoreAPI api)
		{
			base.Start(api);

			Harmony = new Harmony("net.weathersong.flexibletranslocator.changes");
			Harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		public override void StartServerSide(ICoreServerAPI api)
		{
			base.StartServerSide(api);

			ServerApi = api;

			ServerApi.RegisterCommand("flextrans", "Flexible Translocator mod utility command.", "[setmin #|setmax #]", Cmd_FlexTrans);
		}

		#endregion

		#region UTIL_FUNCTIONS

		private void LoadConfig()
		{
			try
			{
				Config = CoreApi.LoadModConfig<FlexibleTranslocatorConfig>(ConfigFilename);
				if (Config == null)
				{
					LogNotif("No config file found. Using defaults, and creating a default config file.");
					Config = DefaultConfig();
					CoreApi.StoreModConfig(Config, ConfigFilename);
				}
				else
				{
					// Extra sanity checks / warnings on particular values:
					// ...

					LogNotif("Config loaded.");

					// In case this was an old version of the config, store again anyway so that it's updated.
					CoreApi.StoreModConfig(Config, ConfigFilename);
				}
			}
			catch (Exception ex)
			{
				LogError($"Problem loading the mod's config file, using defaults. Check the config file for typos! Error details: {ex.Message}");
				Config = DefaultConfig();
			}
		}

		private FlexibleTranslocatorConfig DefaultConfig()
		{
			FlexibleTranslocatorConfig defaultConfig = new FlexibleTranslocatorConfig();
			defaultConfig.ResetToDefaults();

			return defaultConfig;
		}

		private void Cmd_FlexTrans(IServerPlayer player, int groupId, CmdArgs args)
		{
			string response = $"Current configuration for Flexible Translocator mod:\n" +
				$"MinTeleportRangeInBlocks: {Config.MinTeleportRangeInBlocks}\n" +
				$"MaxTeleportRangeInBlocks: {Config.MaxTeleportRangeInBlocks}";

			string verb = args.PopWord() ?? "";

			switch (verb.ToUpperInvariant())
			{
				case "SETMIN":
					int? setmin = args.PopInt();
					if (!setmin.HasValue)
					{
						break;
					}
					else if (setmin > Config.MaxTeleportRangeInBlocks)
					{
						player.SendMessage(groupId, $"SetMin must be < MaxTeleportRange.", EnumChatType.CommandError);
						return;
					}
					else if (setmin < 0 || setmin > 999999)
					{
						player.SendMessage(groupId, $"SetMin must be > 0 and < 1 million.", EnumChatType.CommandError);
						return;
					}
					Config.MinTeleportRangeInBlocks = setmin.Value;
					CoreApi.StoreModConfig(Config, ConfigFilename);
					response = $"Okay. MinTeleportRangeInBlocks: {Config.MinTeleportRangeInBlocks}";
					break;

				case "SETMAX":
					int? setmax = args.PopInt();
					if (!setmax.HasValue)
					{
						break;
					}
					else if (setmax < Config.MinTeleportRangeInBlocks)
					{
						player.SendMessage(groupId, $"SetMax must be > MinTeleportRange.", EnumChatType.CommandError);
						return;
					}
					else if (setmax < 0 || setmax > 999999)
					{
						player.SendMessage(groupId, $"SetMax must be > 0 and < 1 million.", EnumChatType.CommandError);
						return;
					}
					Config.MaxTeleportRangeInBlocks = setmax.Value;
					CoreApi.StoreModConfig(Config, ConfigFilename);
					response = $"Okay. MaxTeleportRangeInBlocks: {Config.MaxTeleportRangeInBlocks}";
					break;

				case null:
				default:
					break;
			}

			player.SendMessage(groupId, response, EnumChatType.CommandSuccess);
		}

		public void LogNotif(string msg)
		{
			CoreApi?.Logger.Notification($"[{LogHeader}] {msg}");
		}

		public void LogWarn(string msg)
		{
			CoreApi?.Logger.Warning($"[{LogHeader}] {msg}");
		}

		public void LogError(string msg)
		{
			CoreApi?.Logger.Error($"[{LogHeader}] {msg}");
		}

		public void LogDebug(string msg)
		{
			if (Config.DebugLogging)
				CoreApi?.Logger.Debug($"[{LogHeader}] {msg}");
		}

		public void MessagePlayer(IPlayer toPlayer, string msg)
		{
			ServerApi?.SendMessage(toPlayer, GlobalConstants.GeneralChatGroup, msg, EnumChatType.OwnMessage);
		}

		public string BlockPosString(BlockPos pos)
		{
			return $"{pos.X} {pos.Y} {pos.Z}";
		}

		public string EntityPosString(SyncedEntityPos pos)
		{
			return $"{pos.X:#.00} {pos.Y:#.00} {pos.Z:#.00}";
		}

		#endregion


	}
}
