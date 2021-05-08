using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Converters
{
    class UserColor : Converter<string>
    {
        public static readonly SolidColorBrush[] Brushes = { SBrushes.NewBrush(0xffd03000), SBrushes.NewBrush(0xff4890c0), SBrushes.NewBrush(0xffe8b800), SBrushes.NewBrush(0xff70A830), SBrushes.NewBrush(0xff403838), SBrushes.NewBrush(0xff907030), SBrushes.NewBrush(0xffb860b8), SBrushes.NewBrush(0xff807878), SBrushes.NewBrush(0xffc0c0c0), SBrushes.NewBrush(0xfff07070) };
        public static readonly UserColor C = new UserColor();
        public static SolidColorBrush GetChatBrush(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            var flag = username;
            if (username[0] == '[')
            {
                var r = username.IndexOf(']', 1);
                if (r != -1) flag = username.Substring(1, r - 1);
            }
            var i = flag.GetHashCode() % Brushes.Length;
            if (i < 0) i += Brushes.Length;
            return Brushes[i];
        }

        protected override object Convert(string value)
        {
            return GetChatBrush(value);
        }
    }
}
