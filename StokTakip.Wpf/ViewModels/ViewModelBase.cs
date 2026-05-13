using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// WPF ekranlarinda veri baglama icin ortak PropertyChanged altyapisini saglar.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
