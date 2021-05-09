using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using PokemonBattleOnline.Network;
using PokemonBattleOnline.PBO.Editor;

namespace PokemonBattleOnline.PBO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static MainWindow()
        {
            RoomWindow.Init();
        }

        public MainWindow()
        {
            InitializeComponent();
            editor.DataContext = EditorVM.Current;
            PBOClient.Disconnected += () => MessageBox.Show("连接到服务器中断。");
            PBOClient.CurrentChanged += PBOClient_CurrentChanged;
            PBOClient_CurrentChanged();
        }
        private void PBOClient_CurrentChanged()
        {
            //already in lock
            if (PBOClient.Current == null)
            {
                lobby.IsEnabled = false;
                lobby.Visibility = Visibility.Collapsed;
                login.Visibility = Visibility.Visible;
                login.IsEnabled = true;
            }
            else
            {
                login.Visibility = Visibility.Collapsed;
                lobby.Init(PBOClient.Current);
                lobby.IsEnabled = true;
                lobby.Visibility = Visibility.Visible;
            }
        }
        public bool lobby_Closing()
        {
            if (PBOClient.Current != null)
                if (ShowMessageBox.ExitLobby() == MessageBoxResult.Yes) PBOClient.Exit();
                else return true;
            return false;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = RoomWindow.Window_Closing(this) || lobby_Closing() || EditorVM.Current.Save();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Config.Save();
            Application.Current.Shutdown();
        }
    }
}
