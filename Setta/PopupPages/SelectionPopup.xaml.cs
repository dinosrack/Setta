using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;

namespace Setta.PopupPages;

public partial class SelectionPopup : Popup
{
    private readonly bool _isMultiSelect;
    private readonly List<string> _items;
    private readonly ObservableCollection<string> _selectedItems;
    private readonly Dictionary<string, Button> _buttons = new();

    public IReadOnlyCollection<string> Result => _selectedItems;

    public SelectionPopup(string title, IEnumerable<string> items, IEnumerable<string> selected, bool isMultiSelect)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        _isMultiSelect = isMultiSelect;
        _items = items.ToList();
        _selectedItems = new ObservableCollection<string>(selected ?? Enumerable.Empty<string>());

        BuildButtons();
    }

    void BuildButtons()
    {
        ItemsLayout.Children.Clear();
        _buttons.Clear();

        foreach (var item in _items)
        {
            var button = new Button
            {
                Text = item,
                Padding = new Thickness(10, 5),
                Margin = new Thickness(0, 5, 5, 0),
                FontFamily = "RubikRegular",
                BackgroundColor = GetBackgroundColor(item),
                HorizontalOptions = LayoutOptions.Start
            };

            // Задаём TextColor через AppThemeBinding
            button.SetAppThemeColor(
                Button.TextColorProperty,
                (Color)Application.Current.Resources["LightText"],
                (Color)Application.Current.Resources["DarkText"]);

            FlexLayout.SetBasis(button, FlexBasis.Auto);

            button.Clicked += (s, e) => OnItemTapped(item);

            _buttons[item] = button;
            ItemsLayout.Children.Add(button);
        }
    }



    void OnItemTapped(string item)
    {
        if (_isMultiSelect)
        {
            if (_selectedItems.Contains(item))
                _selectedItems.Remove(item);
            else
                _selectedItems.Add(item);
        }
        else
        {
            _selectedItems.Clear();
            _selectedItems.Add(item);
        }

        UpdateButtonStyles();
    }

    void UpdateButtonStyles()
    {
        foreach (var kvp in _buttons)
        {
            kvp.Value.BackgroundColor = GetBackgroundColor(kvp.Key);
        }
    }

    Color GetBackgroundColor(string item)
    {
        bool isSelected = _selectedItems.Contains(item);

        if (isSelected)
            return (Color)Application.Current.Resources["Important"];

        var themeKey = Application.Current.RequestedTheme == AppTheme.Dark ? "DarkElement" : "LightElement";

        if (Application.Current.Resources.TryGetValue(themeKey, out var value) && value is Color color)
            return color;

        return Colors.Gray;
    }


    void OnApply(object sender, EventArgs e)
    {
        Close(_selectedItems.ToList());
    }
}
