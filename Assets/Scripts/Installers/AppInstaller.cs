using Zenject;

public class AppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<AppController>().AsSingle();
    }
}