using UnityEngine;

using Colyseus;

public class ClientComponent : MonoBehaviour
{
	public Client client;
	public Room<RoomState> room;

	// Use this for initialization
	public async void Start () {
		client = new Client("ws://localhost:2567");

		room = await client.Join<RoomState>("demo");
		await room.Connect();

		// OnApplicationQuit();
	}

	async void OnDestroy ()
	{
		// Make sure client will disconnect from the server
		await room.Leave ();
	}

	void OnApplicationQuit()
	{
		// Ensure the connection with server is closed immediatelly
		OnDestroy();
	}
}
