using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class GameSystemsInstaller : MonoInstaller
{
    public GameObject PlayerUIPrefab;
    public GameObject AudioManagerPrefab;

    public override void InstallBindings()
    {
        //Debug.Log("Creating GameSystem Bindings");
        Container.Bind<IGameInstance>().To<GameInstance>().AsSingle();
        Container.Bind<IPlayerData>().To<GameInstance>().FromResolve();

        Container.Bind<IAudioManager>().To<AudioManager>().FromComponentsInNewPrefab(AudioManagerPrefab).AsSingle();
        //Container.Bind<ILevelManager>().To<LevelManager>().FromComponentInNewPrefab(LevelManagerPrefab).AsSingle();
    }
}