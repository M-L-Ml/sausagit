using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

namespace SourceGit.Views
{
    public class MenuItemExtension : AvaloniaObject
    {
        public static readonly AttachedProperty<string> CommandProperty =
            AvaloniaProperty.RegisterAttached<MenuItemExtension, MenuItem, string>("Command", string.Empty, false, BindingMode.OneWay);
    }

}

namespace PSGit.Views
{
    using SourceGit.Views;
    using SourceGit.ViewModels;
    using System.Diagnostics;

    public static class MenuItemModelExtension
    {

        public static MenuItem CreateMenuItemFromModel(this MenuItemModel x)
        {
            if (x is ContextMenuModel)
                throw new InvalidOperationException("Use CreateContextMenuFromModel");
            var menu = new MenuItem()
            {
                Header = x.Header.Text(),
                DataContext = x,
                Command = x.Command,
                IsEnabled = x.IsEnabled
            };
            if (x.IconKey != null)
                menu.Icon = App.CreateMenuIcon(x.IconKey);
            x.SetViewSettingsFromModel(menu);
            if (x is MenuModel y)
                y.CreateSubItemsFromModel(menu);
            return menu;
        }
        public static MenuItem CreateMenuFromModel(this MenuModel menuModel)
             => menuModel.CreateMenuItemFromModel();

        public static MenuDeriv CreateMenuFromModelInternal<MenuDeriv>(this MenuModel menuModel)
            where MenuDeriv : ItemsControl, new()
        {
            var menu = new MenuDeriv() { DataContext = menuModel };
            CreateSubItemsFromModel(menuModel, menu);
            return menu;
        }
        public static void CreateSubItemsFromModel<MenuDeriv>(this MenuModel menuModel, MenuDeriv menu)
              where MenuDeriv : ItemsControl, new()
        {
            foreach (var item
                    in menuModel.Items.Select(x =>
                        {
                            return x.CreateMenuItemFromModel();
                        }))
                menu.Items.Add(item);
        }

        //TODO: refactor : use this method for cases where menuModel.CreateContextMenuFromModel(); and then `Open` called
        public static ContextMenu Open(this ContextMenuModel menuModel, Control? control)
        {
            var menu = menuModel.CreateContextMenuFromModel();
            menu.Open(control);
            return menu;

        }

        public static ContextMenu CreateContextMenuFromModel(this ContextMenuModel menuModel)
        {
            var menu = menuModel.CreateMenuFromModelInternal<ContextMenu>();
            menuModel.SetViewSettingsFromModel(menu);
            return menu;

        }
        public static void SetViewSettingsFromModel(this MenuItemModel menuModel, object menu)
        {
            var item = menu as MenuItem;
            if (menuModel.ViewToDo == null)
                return;
            foreach (var (propName, value) in menuModel.ViewToDo)
            {
                switch (propName)
                {
                    case ViewPropertySetting.Placement:
                        if (Enum.TryParse(value.ToString(), out PlacementMode placementMode))
                        {
                            ((ContextMenu)menu).Placement = placementMode;
                        }
                        else
                            Debug.Assert(false);
                        break;

                    case ViewPropertySetting.Views_MenuItemExtension_CommandProperty:
                        ((AvaloniaObject)menu).SetValue(MenuItemExtension.CommandProperty, value.ToString());
                        break;
                    case ViewPropertySetting.icon_Fill:
                        {

                            var m = item!;
                            var icon = (AvaloniaObject)m.Icon;
                            icon.SetValue(Avalonia.Controls.Shapes.Shape.FillProperty, value);
                        }
                        break;
                    case ViewPropertySetting.InputGesture:
                        {

                            var m = ((MenuItem)menu);
                            m.InputGesture = KeyGesture.Parse(value.ToString());
                            break;
                        }
                    case ViewPropertySetting.MinWidth:
                        {

                            var m = ((MenuItem)menu);
                            m.MinWidth = (int)value;
                        }
                        break;
                    case ViewPropertySetting.IconCreate:
                        {

                            (Uri uri, int width, int h) =
                            (IconCreationOptions)value;
                            var asset = Avalonia.Platform.AssetLoader.Open(uri);
                            item.Icon = new Image { Width = width, Height = h, Source = new Avalonia.Media.Imaging.Bitmap(asset) };


                        }
                        break;
                }



                //    == "Placement" && Enum.TryParse(value.ToString(), out PlacementMode placementMode))
                //{
                //    menu.Placement = placementMode;
                //}
            }
        }

    }
}
