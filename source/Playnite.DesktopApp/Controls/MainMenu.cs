﻿using Playnite.Commands;
using Playnite.Common;
using Playnite.DesktopApp.ViewModels;
using Playnite.Extensions.Markup;
using Playnite.SDK;
using Playnite.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Playnite.DesktopApp.Controls
{
    public class MainMenu : ContextMenu
    {
        private readonly DesktopAppViewModel mainModel;

        static MainMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainMenu), new FrameworkPropertyMetadata(typeof(MainMenu)));
        }

        public MainMenu() : this(DesktopApplication.Current?.MainModel)
        {
        }

        public MainMenu(DesktopAppViewModel model)
        {
            if (model != null)
            {
                mainModel = model;
                InitializeItems();
            }
        }

        private MenuItem AddMenuChild(
            ItemCollection parent,
            string locString,
            RelayCommand command,
            object commandParameter = null,
            object icon = null)
        {
            var item = new MenuItem
            {
                Command = command,
                CommandParameter = commandParameter,
                InputGestureText = command?.GestureText
            };

            if (locString.StartsWith("LOC"))
            {
                item.SetResourceReference(MenuItem.HeaderProperty, locString);
            }
            else
            {
                item.Header = locString;
            }

            if (icon != null)
            {
                if (icon is string)
                {
                    item.Icon = Images.GetImageFromFile(ThemeFile.GetFilePath(icon as string, ThemeManager.DefaultTheme, ThemeManager.CurrentTheme));
                }
                else
                {
                    var resource = icon as BitmapImage;
                    var image = new Image() { Source = resource };
                    RenderOptions.SetBitmapScalingMode(image, RenderOptions.GetBitmapScalingMode(resource));
                    item.Icon = image;
                }
            }            

            parent.Add(item);
            return item;
        }

        public void InitializeItems()
        {
            // Add Game
            var addGameItem = AddMenuChild(Items, "LOCMenuAddGame", null, null, ResourceProvider.GetResource("AddGameIcon"));
            AddMenuChild(addGameItem.Items, "LOCMenuAddGameManual", mainModel.AddCustomGameCommand);
            AddMenuChild(addGameItem.Items, "LOCMenuAddGameInstalled", mainModel.AddInstalledGamesCommand);
            AddMenuChild(addGameItem.Items, "LOCMenuAddGameEmulated", mainModel.AddEmulatedGamesCommand);
            Items.Add(new Separator());

            // Update Library
            var updateItem = AddMenuChild(Items, "LOCMenuReloadLibrary", null, null, ResourceProvider.GetResource("UpdateDbIcon"));
            AddMenuChild(updateItem.Items, "LOCUpdateAll", mainModel.UpdateGamesCommand);
            updateItem.Items.Add(new Separator());
            foreach (var plugin in mainModel.Extensions.LibraryPlugins.Values)
            {
                var item = new MenuItem
                {
                    Header = plugin.Plugin.Name,
                    Command = mainModel.UpdateLibraryCommand,
                    CommandParameter = plugin.Plugin
                };

                updateItem.Items.Add(item);
            }

            // Library
            var libraryItem = AddMenuChild(Items, "LOCLibrary", null);
            AddMenuChild(libraryItem.Items, "LOCMenuConfigureEmulatorsMenuTitle", mainModel.OpenEmulatorsCommand);
            AddMenuChild(libraryItem.Items, "LOCMenuLibraryManagerTitle", mainModel.OpenDbFieldsManagerCommand);
            AddMenuChild(libraryItem.Items, "LOCMenuDownloadMetadata", mainModel.DownloadMetadataCommand);

            // Extensions
            var extensionsItem = AddMenuChild(Items, "LOCExtensions", null);
            AddMenuChild(extensionsItem.Items, "LOCReloadScripts", mainModel.ReloadScriptsCommand);
            extensionsItem.Items.Add(new Separator());
            foreach (var function in mainModel.Extensions.ExportedFunctions)
            {
                var item = new MenuItem
                {
                    Header = function.Name,
                    Command = mainModel.InvokeExtensionFunctionCommand,
                    CommandParameter = function
                };

                extensionsItem.Items.Add(item);
            }

            // Open Client
            var openClientItem = AddMenuChild(Items, "LOCMenuOpenClient", null);
            foreach (var tool in mainModel.ThirdPartyTools)
            {
                var item = new MenuItem
                {
                    Header = tool.Name,
                    Command = mainModel.ThirdPartyToolOpenCommand,
                    CommandParameter = tool
                };

                openClientItem.Items.Add(item);
            }


            // Settings
            AddMenuChild(Items, "LOCMenuPlayniteSettingsTitle", mainModel.OpenSettingsCommand, null, ResourceProvider.GetResource("SettingsIcon"));

            // FullScreen
            Items.Add(new Separator());
            AddMenuChild(Items, "LOCMenuOpenFullscreen", mainModel.OpenFullScreenCommand, null, ResourceProvider.GetResource("FullscreenModeIcon"));
            Items.Add(new Separator());

            // About
            AddMenuChild(Items, "LOCMenuAbout", mainModel.OpenAboutCommand, null, ResourceProvider.GetResource("AboutPlayniteIcon"));

            // Links
            var linksItem = AddMenuChild(Items, "LOCMenuLinksTitle", null);
            AddMenuChild(linksItem.Items, "Patreon", GlobalCommands.NavigateUrlCommand, @"https://www.patreon.com/playnite", "Images/patreon.png");
            AddMenuChild(linksItem.Items, "Discord", GlobalCommands.NavigateUrlCommand, @"https://discord.gg/hSFvmN6", "Images/discord.png");
            AddMenuChild(linksItem.Items, "Twitter", GlobalCommands.NavigateUrlCommand, @"https://twitter.com/AppPlaynite", "Images/twitter.png");
            AddMenuChild(linksItem.Items, "Reddit", GlobalCommands.NavigateUrlCommand, @"https://www.reddit.com/r/playnite/", "Images/reddit.png");
            linksItem.Items.Add(new Separator());
            AddMenuChild(linksItem.Items, "LOCSDKDocumentation", GlobalCommands.NavigateUrlCommand, @"https://playnite.link/docs/", "Images/MenuIcons/script.png");

            // Feedback
            AddMenuChild(Items, "LOCMenuIssues", mainModel.ReportIssueCommand);

            // Check for update
            AddMenuChild(Items, "LOCCheckForUpdates", mainModel.CheckForUpdateCommand);
            Items.Add(new Separator());

            // Exit
            AddMenuChild(Items, "LOCExitAppLabel", mainModel.ShutdownCommand, null, ResourceProvider.GetResource("ExitIcon"));
        }
    }
}