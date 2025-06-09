using System;
using System.Collections.Generic;

/// <summary>
/// G is a global static service locator that provides access to registered services.
/// </summary>
public class G {
    private static readonly Dictionary<Type, IProviderHandler> Providers = new();
    
    /// <summary>
    /// Retrieves a registered service of type T.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <returns>The registered service instance.</returns>
    /// <exception cref="Exception">Thrown if the requested service is not registered.</exception>
    public static T Get<T>() where T : class, IProviderHandler {
        if (!Providers.TryGetValue(typeof(T), out var target)) {
            throw new Exception($"Service of type {typeof(T)} is not registered!");
        }
        return (T)target;
    }
    
    /// <summary>
    /// Registers a service instance of type T.
    /// </summary>
    /// <typeparam name="T">The type of the service to register.</typeparam>
    /// <param name="target">The instance of the service to register.</param>
    /// <exception cref="Exception">Thrown if the service is already registered.</exception>
    public static void Register<T>(T target) where T : class, IProviderHandler {
        Type type = typeof(T);
        if (Providers.ContainsKey(type)) {
            throw new Exception($"Service {type} is already registered!");
        }
        Providers[type] = target;
    }
    
    /// <summary>
    /// Unregisters a service instance of type T.
    /// </summary>
    /// <typeparam name="T">The type of the service to unregister.</typeparam>
    /// <param name="target">The instance of the service to unregister.</param>
    public static void Unregister<T>(T target) where T : class, IProviderHandler {
        Type type = typeof(T);
        Providers.Remove(type);
    }
    
    /// <summary>
    /// Clears all registered services from the service provider.
    /// </summary>
    public static void ClearAll() {
        Providers.Clear();
    }
    
    // UI
    public static class UI {
        public static SelectToolTypeUI SelectToolTypeUI => Get<SelectToolTypeUI>();
        public static SidePanelUI SidePanelUI => Get<SidePanelUI>();
    }
    
    // Managers
    public static GameAssets GameAssets => Get<GameAssets>();
    public static InputManager InputManager => Get<InputManager>();
    public static CameraManager CameraManager => Get<CameraManager>();
    public static DataManager DataManager => Get<DataManager>();
    public static GameManager GameManager => Get<GameManager>();
    public static WorldExpansionManager WorldExpansionManager => Get<WorldExpansionManager>();
    public static PlacementManager PlacementManager => Get<PlacementManager>();
    public static TaskManager TaskManager => Get<TaskManager>();
    public static UnitsManager UnitsManager => Get<UnitsManager>();
    public static QuestManager QuestManager => Get<QuestManager>();
}