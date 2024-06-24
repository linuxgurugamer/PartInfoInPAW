namespace PartInfoInPAW
{
	internal readonly struct ModWithComplexFoldersStruct
	{
		private readonly string ModFolder;
		private readonly int NestingLevel;

		public ModWithComplexFoldersStruct(string folder, int nestLevel = 2)
		{
			ModFolder = folder;
			NestingLevel = nestLevel;
		}

		public bool URLMatches(string url)
		{
			return (ModFolder == url.Split('/')[0]);
		}

		public string BuildModName(string url)
		{
			int nestLevel = NestingLevel;
			string[] folders = url.Split('/');
			string result = "";
			if (folders.Length < nestLevel)
			{
				nestLevel = folders.Length;
			}
			for (int i = 0; i < nestLevel; i++)
			{
				if (i > 0)
				{
					result += "/" + folders[i];
				}
				else
				{
					result += folders[i];
				}
			}
			return result;
		}
	}
}
