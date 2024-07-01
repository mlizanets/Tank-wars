using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// ApplicationController manages the launch and mode of the application (client or host)
public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab; // Reference to the client singleton prefab
    [SerializeField] private HostSingleton hostPrefab; // Reference to the host singleton prefab

    // This method is called when the script instance is being loaded
    private async void Start()
    {
        DontDestroyOnLoad(gameObject); // Prevent this game object from being destroyed on scene load

        // Launch the application in the appropriate mode based on whether it is a dedicated server
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    // Method to launch the application in the appropriate mode (dedicated server or client/host)
    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            // Dedicated server specific logic (if any) can be added here
        }
        else
        {
            ClientSingleton clientSingleton = Instantiate(clientPrefab); // Instantiate the client singleton
            await clientSingleton.CreateClient(); // Create the client asynchronously

            HostSingleton hostSingleton = Instantiate(hostPrefab); // Instantiate the host singleton
            hostSingleton.CreateHost(); // Create the host

            // Logic to navigate to the main menu can be added here
        }
    }
}