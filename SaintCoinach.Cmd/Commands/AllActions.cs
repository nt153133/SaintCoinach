﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

using SaintCoinach;
using SaintCoinach.Ex;
using SaintCoinach.Xiv;

using ProtoBuf;
using ProtoBuf.Meta;

namespace SaintCoinach.Cmd.Commands {
    public class AllActionsCommand : ActionCommandBase {
        private ARealmReversed _Realm;

		public AllActionsCommand(ARealmReversed realm)
            : base("actions", "Export all actions to a protobuff file") {
            _Realm = realm;
        }

        public override async Task<bool> InvokeAsync(string paramList) {
            const string CsvFileFormat = "spell.bin";

			var sheet = _Realm.GameData.GetSheet<Xiv.Action>("Action");

			var result = new Actions();
			result.Version = 2473375;
			var last = sheet.Keys.Last();
			result.data = new Action[last+1];
			try
			{
				foreach (var x in sheet)
				{
					//if (x.Key == 6490)
					//{
					//	OutputInformation("6490");
					//}
					result.data[x.Key] = new Action
					{
						//EnglishName = x.Name,
						Id = x.Key,
						Range = (sbyte)x.Range,
						TargetArea = x.TargetArea,
						//CastType = x.CastType,
						EffectRange = x.EffectRange,
						//XAxisModifier = x.XAxisModifier,
						//Omen = x.Omen.Key,
						//Interupt = x.Interupt,
					};
				}
			} catch(Exception ex)
			{
				OutputInformation(ex.ToString());
				throw ex;
			}
 
			if (result.data[6334].Interupt != true)
			{
				OutputError("6334 did not validate. found {0} || {1}", sheet[6334].Name, result.data[6334].Interupt);
				return false;
			}
			if (result.data[6336].Interupt != true)
			{
				OutputError("6336 did not validate. found {0} || {1}", sheet[6336].Name, result.data[6336].Interupt);
				return false;
			}
			if (result.data[6417].Interupt != false)
			{
				OutputError("6417 did not validate. found {0} || {1}", sheet[6417].Name, result.data[6417].Interupt);
				return false;
			}

			RuntimeTypeModel.Default[typeof(Actions)][2].SupportNull = true;


			File.WriteAllBytes(CsvFileFormat, ToBytes(result));
            
			OutputInformation("{0} actions exported", result.data.Count());

			var test = FromBytes<Actions>(File.ReadAllBytes(CsvFileFormat));

			if (test.data[6490].Id != 6490)
			{
				OutputInformation("Test failed");
			}
			else
			{
				OutputInformation("Test OK");
			}

            return true;
        }

		private static T FromBytes<T>(byte[] data)
		{
			var obj = default(T);
			using (var stream = new MemoryStream(data))
			{
				try { obj = Serializer.Deserialize<T>(stream); }
				catch (Exception e) { Console.WriteLine(e); }
			}

			return obj;
		}

		private static byte[] ToBytes<T>(T obj)
		{
			byte[] data;
			using (var stream = new MemoryStream())
			{
				try { Serializer.Serialize(stream, obj); }
				catch (Exception e) { Console.WriteLine(e); }

				data = stream.ToArray();
			}

			return data;
		}

	}

	[ProtoContract]
	public class Actions
	{
		[ProtoMember(1)]
		public int Version;

		[ProtoMember(2)]
		public Action[] data;
	}

	[ProtoContract]
	public class Action
	{
		//[ProtoMember(1)]
		//public string EnglishName;

		[ProtoMember(1)]
		public int Id;

		[ProtoMember(2)]
		public sbyte Range;

		[ProtoMember(3)]
		public bool TargetArea;

		[ProtoMember(4)]
		public int CastType;

		[ProtoMember(5)]
		public int EffectRange;

		[ProtoMember(6)]
		public int XAxisModifier;

		[ProtoMember(7)]
		public int Omen;

		[ProtoMember(8)]
		public bool Interupt;
	}
}
