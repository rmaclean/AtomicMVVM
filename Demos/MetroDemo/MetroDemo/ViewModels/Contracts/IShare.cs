
namespace MetroDemo.ViewModels.Contracts
{
    using Windows.ApplicationModel.DataTransfer;
    interface IShare
    {
        void Share(DataRequest dataRequest);
    }
}
