// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.34
// 

using Colyseus.Schema;

public class RoomState : Schema {
	[Type(0, "map", typeof(MapSchema<Entity>))]
	public MapSchema<Entity> entities = new MapSchema<Entity>();

	[Type(1, "string")]
	public string roomID = "";

	[Type(2, "string")]
	public string roomName = "";

	[Type(3, "string")]
	public string psw = "";

	[Type(4, "string")]
	public string owner = "";

	[Type(5, "int32")]
	public int state = 0;
}

