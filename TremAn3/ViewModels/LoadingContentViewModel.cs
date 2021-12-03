using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.ViewModels;
public class LoadingContentViewModel : ViewModelBase
{

    public MainViewModel MainVm => ViewModelLocator.Current.MainViewModel;

    private LoadingContentType _Type;

    public LoadingContentType Type
    {
        get => _Type;
        set {

            if (Set(ref _Type, value))
                IsDoingSomething = Type != LoadingContentType.Off;
        }
    }

    private bool _IsDoingSomething;

    public bool IsDoingSomething
    {
        get => _IsDoingSomething;
        set => Set(ref _IsDoingSomething, value);
    }

    public bool IsCurrently(LoadingContentType currentType, LoadingContentType askingType) => askingType == currentType;


}

public enum LoadingContentType
{
    Off, Generic,  ComputationInProgress, ComputingVectorData, ComputingGlobalVectorData
}
