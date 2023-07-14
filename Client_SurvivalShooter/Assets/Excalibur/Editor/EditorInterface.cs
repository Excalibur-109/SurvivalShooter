
namespace Excalibur
{
    public interface IWindowDrawer : IPersistant
    {
        string title { get; }
        void OpenWindow();
        void OnEditorGUI();
        void Initialize();
        void Terminate();
    }
}

