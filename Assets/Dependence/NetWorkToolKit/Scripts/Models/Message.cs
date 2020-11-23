// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.34
// 

using Colyseus.Schema;

public class Message : Schema {
	[Type(0, "ref", typeof(Header))]
	public Header header = new Header();

	[Type(1, "string")]
	public string content = "";
}

