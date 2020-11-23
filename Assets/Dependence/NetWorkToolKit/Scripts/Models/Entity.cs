// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.34
// 

using Colyseus.Schema;

public class Entity : Schema {
	[Type(0, "string")]
	public string id = "";

	[Type(1, "string")]
	public string owner = "";

	[Type(2, "int32")]
	public int type = 0;

	[Type(3, "string")]
	public string name = "";

	[Type(4, "ref", typeof(NetVector3))]
	public NetVector3 position = new NetVector3();

	[Type(5, "ref", typeof(NetQuaternion))]
	public NetQuaternion rotation = new NetQuaternion();

	[Type(6, "string")]
	public string extraInfo = "";
}

