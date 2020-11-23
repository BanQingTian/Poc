//
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
//
// GENERATED USING @colyseus/schema 0.4.61
//

using Colyseus.Schema;

namespace SchemaTest.Forwards {
	public class StateV2 : Schema {
		[Type(0, "string")]
		public string str = "";

		[System.Obsolete("field 'map' is deprecated.", true)]
		[Type(1, "map", typeof(MapSchema<PlayerV2>))]
		public MapSchema<PlayerV2> map = new MapSchema<PlayerV2>();

		[Type(2, "number")]
		public float countdown = 0;
	}
}
