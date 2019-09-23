using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchmakingMenu : MonoBehaviour {
    [SerializeField]
    private InputField ipAdressInput, portInput;

    [SerializeField]
    private NetworkManager networkManagerPrefab;

    [SerializeField]
    private string gameScene;

    private NetworkManager networkManager;

    private const int MaxPlayers = 32;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        networkManager = Instantiate(networkManagerPrefab);
    }

    public void Host() {
        var server = new UDPServer(MaxPlayers);
        server.Connect(ipAdressInput.text, ushort.Parse(portInput.text));

        networkManager.Initialize(server);

        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    public void Connect() {
        var client = new UDPClient();
        client.Connect(ipAdressInput.text, ushort.Parse(portInput.text));

        networkManager.Initialize(client);
    }
}