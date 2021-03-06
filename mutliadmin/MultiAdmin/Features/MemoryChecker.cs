﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
	class MemoryChecker : Feature, IEventTick
	{
		private int lowMb;
		private int maxMb;
		private int tickCount;
		public MemoryChecker(Server server) : base(server)
		{
		}

		public override void Init()
		{
			tickCount = 0;
		}

		public override string GetFeatureDescription()
		{
			return "Restarts the server if the working memory becomes too low";
		}

		public override string GetFeatureName()
		{
			return "Restart On Low Memory";
		}

		public void OnTick()
		{
			Server.GetGameProccess().Refresh();
			long workingMemory = Server.GetGameProccess().WorkingSet64 / 1048576L; // process memory in MB
			long memoryLeft = maxMb - workingMemory; // 32 bit limited to 2GB

			if (memoryLeft < lowMb)
			{
				Server.Write("Warning: program is running low on memory (" + memoryLeft + " MB left)", ConsoleColor.Red);
				tickCount++;
			}
			else
			{
				tickCount = 0;
			}

			if (tickCount == 10)
			{
				Server.Write("Restarting due to lower memory", ConsoleColor.Red);
				Server.SoftRestartServer();
			}

		}

		public override void OnConfigReload()
		{
			lowMb = Server.ServerConfig.GetIntValue("RESTART_LOW_MEMORY", 400);
			lowMb = (lowMb > 0 ? lowMb : 400); // Prevent negative values

			maxMb = Server.ServerConfig.GetIntValue("MAX_MEMORY", 2048); // 32 bit limited to 2GB
			maxMb = (maxMb > 0 ? maxMb : 2048); // Prevent negative values
		}
	}
}
