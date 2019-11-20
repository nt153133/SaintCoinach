﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaintCoinach.Cmd.DeepDungeonExporter;
//using SaintCoinach.Cmd.DeepDungeonExporter;
using SaintCoinach.Graphics.TerritoryParts;
using SaintCoinach.Graphics.Viewer;
using SaintCoinach.Graphics.Viewer.Content;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;
using SaintCoinach.Graphics;

namespace SaintCoinach.Cmd.Commands
{
	using SharpDX;

	public class CollisionCommand : ActionCommandBase
	{

		private ARealmReversed _Realm;

		public CollisionCommand(ARealmReversed realm)
		    : base("collision", "Export deep dungeon nav mesh.")
		{
			_Realm = realm;
		}

		public override async Task<bool> InvokeAsync(string paramList)
		{
			const string CsvFileFormat = "collision/{0}.pcb";
			var c = 0;
			var allMaps = _Realm.GameData.GetSheet<SaintCoinach.Xiv.TerritoryType>().Where(i => i.PlaceName.ToString().Contains("The Palace of the Dead")).GroupBy(i => i.Bg).Select(i => i.First());
			foreach (var map in allMaps)
			{
				var territory = new Graphics.Territory(map);
				//if (territory.Collision == null) continue;

				OutputInformation(territory.Name);
				var target = new FileInfo(Path.Combine(_Realm.GameVersion, string.Format(CsvFileFormat, map.Key)));

				if (!target.Directory.Exists)
					target.Directory.Create();

				//System.IO.File.WriteAllBytes(target.FullName, territory.Collision.GetData());
				c++;
			}
			OutputInformation("{0} collisions saved", c);

			return true;
		}
		static string ToPathSafeString(string input, char invalidReplacement = '_')
		{
			var sb = new StringBuilder(input);
			var invalid = Path.GetInvalidFileNameChars();
			foreach (var c in invalid)
				sb.Replace(c, invalidReplacement);
			return sb.ToString();
		}
	}
}
