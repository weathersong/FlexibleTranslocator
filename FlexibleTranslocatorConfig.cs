using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleTranslocator
{
	public class FlexibleTranslocatorConfig
	{
		public int MinTeleportRangeInBlocks = 400;
		public int MaxTeleportRangeInBlocks = 8000;

		public bool DebugLogging;

		internal void ResetToDefaults()
		{
			MinTeleportRangeInBlocks = 400;
			MaxTeleportRangeInBlocks = 8000;

			DebugLogging = false;
		}
	}
}
