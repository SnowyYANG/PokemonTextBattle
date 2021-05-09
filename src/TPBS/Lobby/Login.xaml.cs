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
using System.Windows.Threading;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO.Lobby
{
    /// <summary>
    /// Interaction logic for GlanceLobbies.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            PBOClient.LoginFailed_Name += () => LoginFailed("不能使用的登陆名。");
            PBOClient.LoginFailed_Version += (sv) =>
            {
                if (sv < PBOMarks.VERSION) LoginFailed("客户端与服务器的版本不兼容，客户端版本较新。");
                else LoginFailed("客户端与服务器的版本不兼容，请到http://snowy.asia/下载最新版客户端。");
            };
            PBOClient.LoginFailed_Full += () => LoginFailed("服务器已满。");
            PBOClient.LoginFailed_Disconnect += () => LoginFailed("连接到服务器失败。");
            PBOClient.CurrentChanged += () =>
              {
                  if (PBOClient.Current != null)
                  {
                      var server = servers.Text;
                      var ss = Config.Servers;
                      if (ss.FirstOrDefault() != server)
                      {
                          ss.Remove(server);
                          var n = ss.Count;
                          if (n > 30) ss.RemoveRange(30, n - 30);
                          ss.Insert(0, server);
                          servers.Items.Refresh();
                      }
                  }
              };
            var na = Config.Name;
            if (na != null) name.Text = na;
            servers.ItemsSource = Config.Servers;
            servers.SelectedIndex = 0;
        }

        private void LoginFailed(string message)
        {
            MessageBox.Show(message);
            IsEnabled = true;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(servers.Text) && !string.IsNullOrWhiteSpace(name.Text))
            {
                var na = name.Text.Trim();
                var av = 0;
                PBOClient.Login(servers.Text.Trim(), na, (ushort)av);
                Config.Name = na;
                IsEnabled = false;
            }
        }
    }
}
