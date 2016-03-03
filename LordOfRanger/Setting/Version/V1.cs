﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
// ReSharper disable JoinDeclarationAndInitializer

namespace LordOfRanger.Setting.Version {
	internal class V1 :IF {
		private const int VERSION = 1;

		private struct ArdHeader {
			internal int id;
			internal Act.InstanceType instanceType;
			internal int priority;
			internal int skillIconSize;
			internal int disableSkillIconSize;
			internal int pushDataSize;
			internal int sendDataSize;
		}

		public Mass Load( string filename ) {
			var mass = new Mass();
			mass.Init();
			mass.name = filename;

			var fs = new FileStream( Mass.SETTING_PATH + mass.name + Mass.EXTENSION, FileMode.Open, FileAccess.Read );
			var array = new byte[fs.Length];

			fs.Read( array, 0, (int)fs.Length );
			fs.Close();

			var offset = 0;
			var version = BitConverter.ToInt32( array, offset );
			offset += 4;

			if( version != VERSION ) {
				throw new InvalidDataException();
			}

			var titleSize = BitConverter.ToInt32( array, offset );
			offset += 4;
			var hotKeySize = BitConverter.ToInt32( array, offset );
			offset += 4;
			var headerSize = BitConverter.ToInt32( array, offset );
			offset += 4;
			mass.Sequence = BitConverter.ToInt32( array, offset );
			offset += 4;
			//	var title = Encoding.UTF8.GetString( array, offset, titleSize );
			offset += titleSize;
			mass.hotKey = array.Skip( offset ).Take( hotKeySize ).ToArray()[0];
			offset += hotKeySize;
			var headerCount = headerSize / 28;
			var headers = new List<ArdHeader>();
			for( var i = 0; i < headerCount; i++ ) {
				var ardHeader = new ArdHeader();
				ardHeader.id = BitConverter.ToInt32( array, offset );
				offset += 4;
				ardHeader.instanceType = (Act.InstanceType)BitConverter.ToInt32( array, offset );
				offset += 4;
				ardHeader.priority = BitConverter.ToInt32( array, offset );
				offset += 4;
				ardHeader.skillIconSize = BitConverter.ToInt32( array, offset );
				offset += 4;
				ardHeader.disableSkillIconSize = BitConverter.ToInt32( array, offset );
				offset += 4;
				ardHeader.pushDataSize = BitConverter.ToInt32( array, offset );
				offset += 4;
				ardHeader.sendDataSize = BitConverter.ToInt32( array, offset );
				offset += 4;
				headers.Add( ardHeader );
			}

			foreach( var ardHeader in headers ) {
				// ReSharper disable once SwitchStatementMissingSomeCases
				switch( ardHeader.instanceType ) {
					case Act.InstanceType.COMMAND:
						var c = new Command();
						c.Id = ardHeader.id;
						c.Priority = ardHeader.priority;
						c.SkillIcon = BinaryToBitmap( array.Skip( offset ).Take( ardHeader.skillIconSize ).ToArray() );
						offset += ardHeader.skillIconSize;
						c.DisableSkillIcon = BinaryToBitmap( array.Skip( offset ).Take( ardHeader.disableSkillIconSize ).ToArray() );
						offset += ardHeader.disableSkillIconSize;
						c.Push = array.Skip( offset ).Take( ardHeader.pushDataSize ).ToArray();
						offset += ardHeader.pushDataSize;
						c.sendList = array.Skip( offset ).Take( ardHeader.sendDataSize ).ToArray();
						offset += ardHeader.sendDataSize;
						mass.Add( c );
						break;
					case Act.InstanceType.BARRAGE:
						var b = new Barrage();
						b.Id = ardHeader.id;
						b.Priority = ardHeader.priority;
						b.SkillIcon = BinaryToBitmap( array.Skip( offset ).Take( ardHeader.skillIconSize ).ToArray() );
						offset += ardHeader.skillIconSize;
						b.DisableSkillIcon = BinaryToBitmap( array.Skip( offset ).Take( ardHeader.disableSkillIconSize ).ToArray() );
						offset += ardHeader.disableSkillIconSize;
						b.Push = array.Skip( offset ).Take( ardHeader.pushDataSize ).ToArray();
						offset += ardHeader.pushDataSize;
						b.send = array.Skip( offset ).Take( ardHeader.sendDataSize ).ToArray()[0];
						offset += ardHeader.sendDataSize;
						mass.Add( b );
						break;
					case Act.InstanceType.TOGGLE:
						var t = new Toggle();
						t.Id = ardHeader.id;
						t.Priority = ardHeader.priority;
						t.SkillIcon = BinaryToBitmap( array.Skip( offset ).Take( ardHeader.skillIconSize ).ToArray() );
						offset += ardHeader.skillIconSize;
						t.DisableSkillIcon = BinaryToBitmap( array.Skip( offset ).Take( ardHeader.disableSkillIconSize ).ToArray() );
						offset += ardHeader.disableSkillIconSize;
						t.Push = array.Skip( offset ).Take( ardHeader.pushDataSize ).ToArray();
						offset += ardHeader.pushDataSize;
						t.send = array.Skip( offset ).Take( ardHeader.sendDataSize ).ToArray()[0];
						offset += ardHeader.sendDataSize;
						mass.Add( t );
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return mass;
		}

		public byte GetHotKey( string filename ) {
			try {
				var fs = new FileStream( Mass.SETTING_PATH + filename + Mass.EXTENSION, FileMode.Open, FileAccess.Read );
				var array = new byte[fs.Length];

				fs.Read( array, 0, (int)fs.Length );
				fs.Close();

				var offset = 0;
				//	var version = BitConverter.ToInt32( array, offset );
				offset += 4;
				var titleSize = BitConverter.ToInt32( array, offset );
				offset += 4;
				var hotKeySize = BitConverter.ToInt32( array, offset );
				offset += 4;
				//	var headerSize = BitConverter.ToInt32( array, offset );
				offset += 4;
				//	sequence = BitConverter.ToInt32( array, offset );
				offset += 4;
				//	string title = Encoding.UTF8.GetString( array, offset, titleSize );
				offset += titleSize;
				return array.Skip( offset ).Take( hotKeySize ).ToArray()[0];

			} catch( Exception ) {
				return 0x00;
			}
		}
		private static Bitmap BinaryToBitmap( IReadOnlyCollection<byte> binary ) {
			if( binary.Count == 0 ) {
				return null;
			}
			return (Bitmap)new ImageConverter().ConvertFrom( binary );
		}

	}
}
