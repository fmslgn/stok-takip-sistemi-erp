using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace StokTakip.Wpf.Helpers;

/// <summary>
/// ComboBox üzerinde seçim yokken açık gri placeholder gösterir; listeye sahte kayıt eklenmez.
/// </summary>
public static class ComboBoxPlaceholderHelper
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(ComboBoxPlaceholderHelper),
            new PropertyMetadata(null, OnPlaceholderTextChanged));

    private static readonly DependencyProperty PlaceholderTextBlockProperty =
        DependencyProperty.RegisterAttached(
            "PlaceholderTextBlock",
            typeof(TextBlock),
            typeof(ComboBoxPlaceholderHelper),
            new PropertyMetadata(null));

    public static string? GetText(DependencyObject obj) => (string?)obj.GetValue(TextProperty);

    public static void SetText(DependencyObject obj, string? value) => obj.SetValue(TextProperty, value);

    private static TextBlock? GetPlaceholderTextBlock(DependencyObject obj) =>
        (TextBlock?)obj.GetValue(PlaceholderTextBlockProperty);

    private static void SetPlaceholderTextBlock(DependencyObject obj, TextBlock? value) =>
        obj.SetValue(PlaceholderTextBlockProperty, value);

    private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ComboBox comboBox)
        {
            return;
        }

        comboBox.Loaded -= ComboBox_Loaded;
        comboBox.Loaded += ComboBox_Loaded;

        if (comboBox.IsLoaded)
        {
            AttachPlaceholder(comboBox);
        }
    }

    private static void ComboBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            AttachPlaceholder(comboBox);
        }
    }

    private static void AttachPlaceholder(ComboBox comboBox)
    {
        var placeholderMetni = GetText(comboBox);
        if (string.IsNullOrWhiteSpace(placeholderMetni) || GetPlaceholderTextBlock(comboBox) is not null)
        {
            return;
        }

        if (comboBox.Parent is not Panel parentPanel)
        {
            return;
        }

        var index = parentPanel.Children.IndexOf(comboBox);
        if (index < 0)
        {
            return;
        }

        parentPanel.Children.Remove(comboBox);

        var kapsayici = new Grid();
        kapsayici.Children.Add(comboBox);

        var placeholder = new TextBlock
        {
            Text = placeholderMetni,
            IsHitTestVisible = false,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(12, 0, 42, 0),
            FontSize = comboBox.FontSize > 0 ? comboBox.FontSize : 13,
            FontFamily = comboBox.FontFamily,
        };
        // Tema degisiminde renk DynamicResource ile guncellenir
        placeholder.SetResourceReference(TextBlock.ForegroundProperty, "PlaceholderForegroundBrush");

        kapsayici.Children.Add(placeholder);
        SetPlaceholderTextBlock(comboBox, placeholder);
        parentPanel.Children.Insert(index, kapsayici);

        void Guncelle()
        {
            placeholder.Visibility = SecimVarMi(comboBox) ? Visibility.Collapsed : Visibility.Visible;
        }

        comboBox.SelectionChanged += (_, _) => Guncelle();

        var selectedItemDescriptor = DependencyPropertyDescriptor.FromProperty(
            Selector.SelectedItemProperty,
            typeof(ComboBox));
        selectedItemDescriptor?.AddValueChanged(comboBox, (_, _) => Guncelle());

        var selectedIndexDescriptor = DependencyPropertyDescriptor.FromProperty(
            Selector.SelectedIndexProperty,
            typeof(ComboBox));
        selectedIndexDescriptor?.AddValueChanged(comboBox, (_, _) => Guncelle());

        Guncelle();
    }

    /// <summary>Gerçek seçim var mı kontrol eder; placeholder metni seçili sayılmaz.</summary>
    private static bool SecimVarMi(ComboBox comboBox)
    {
        if (comboBox.SelectedItem is string metin)
        {
            return !string.IsNullOrWhiteSpace(metin);
        }

        return comboBox.SelectedIndex >= 0 && comboBox.SelectedItem is not null;
    }
}
