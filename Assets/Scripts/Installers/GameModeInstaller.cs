using UnityEngine;
using Zenject;

public class GameModeInstaller : MonoInstaller<GameModeInstaller>
{
    public override void InstallBindings()
    {
        //Container.Bind<IGameModeState>().To<GameMode>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<IGameModeEvents>().To<GameMode>().FromComponentInHierarchy().AsSingle();

        //Container.Bind(typeof(IGameModeState), typeof(IGameModeEvents)).To<GameMode>().FromComponentInHierarchy().AsSingle();

        //// Bind the GameMode class as a singleton
        Container.Bind<GameMode>().FromComponentInHierarchy().AsSingle();
        
        // Bind the IGameModeState and IGameModeEvents interfaces
        Container.Bind<IGameModeState>().To<GameMode>().FromResolve();
        Container.Bind<IGameModeEvents>().To<GameMode>().FromResolve();
    }
}