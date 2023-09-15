using UnityEngine;
using UnityEngine.UI;

public class UIReloadModule : MonoBehaviour
{
    [SerializeField] private Slider reloadModule;

    [SerializeField] private float trackModuleRight = 0;
    [SerializeField] private float trackModuleLeft = 0;
    

    [SerializeField] private VehicleModule[] vehicleModule;
    
    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += UpdateVehicleModules;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned -= UpdateVehicleModules;
        }
        
        if (vehicleModule != null)
        {
            vehicleModule[0].OnTimerUpdate -= UpdateLeftTrackModule;
            vehicleModule[1].OnTimerUpdate -= UpdateRightTrackModule;
        }
        
        
    }

    private void UpdateVehicleModules(Vehicle vehicle)
    {
        vehicleModule = vehicle.GetComponents<VehicleModule>();
        SubscribeToModuleUpdates(vehicleModule);
    }

    private void SubscribeToModuleUpdates(VehicleModule[] modules)
    {
        modules[0].OnTimerUpdate += UpdateLeftTrackModule;
        modules[1].OnTimerUpdate += UpdateRightTrackModule;
        // Добавьте подписку на другие модули, если необходимо.
    }

    private void UpdateLeftTrackModule(float time)
    {
        trackModuleLeft = time;
    }

    private void UpdateRightTrackModule(float time)
    {
        trackModuleRight = time;
    }

    private void Update()
    {
        reloadModule.value = (trackModuleLeft > trackModuleRight) ? trackModuleLeft : trackModuleRight;

    }
}
