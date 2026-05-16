using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// WPF ekranlarinda veri baglama icin ortak PropertyChanged altyapisini saglar.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    private bool _isBusy;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Arka planda veri yuklenirken true olur; butonlarin IsEnabled baglantisi veya durum metni icin kullanilir.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        protected set
        {
            if (_isBusy == value)
            {
                return;
            }

            _isBusy = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

