namespace Kott.Utilities.UI
{
    public interface IView<in T>
    {
        void UpdateView(T data);
    }
}
