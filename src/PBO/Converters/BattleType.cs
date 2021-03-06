using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Converters
{
    public class BattleTypeBg : Converter<BattleType>
    {
        public readonly static BattleTypeBg C = new BattleTypeBg();
        static readonly SolidColorBrush[] c;
        static BattleTypeBg()
        {
            c = new SolidColorBrush[RomData.BATTLETYPES];
            c[0] = SBrushes.NewBrush(0xffdedecf);//normal
            c[1] = SBrushes.NewBrush(0xffde6a52);//fight
            c[2] = SBrushes.NewBrush(0xff7ebbff);//flying
            c[3] = SBrushes.NewBrush(0xffd06abb);//poison
            c[4] = SBrushes.NewBrush(0xfff4dc69);//ground
            c[5] = SBrushes.NewBrush(0xffdccd7e);//rock
            c[6] = SBrushes.NewBrush(0xffccdc2c);//bug
            c[7] = SBrushes.NewBrush(0xff7f7fdb);//ghost
            c[8] = SBrushes.NewBrush(0xffcdcddb);//steel
            c[9] = SBrushes.NewBrush(0xffff552d);//fire
            c[10] = SBrushes.NewBrush(0xff3fbbff);//water
            c[11] = SBrushes.NewBrush(0xff94ea6c);//grass
            c[12] = SBrushes.NewBrush(0xfff8c030);//electric
            c[13] = SBrushes.NewBrush(0xffff6abb);//psychic
            c[14] = SBrushes.NewBrush(0xff93f5ff);//ice
            c[15] = SBrushes.NewBrush(0xff947efd);//dragon
            c[16] = SBrushes.NewBrush(0xff946952);//dark
            c[17] = SBrushes.NewBrush(0xffffcfff);//fairy
        }

        protected override object Convert(BattleType value)
        {
            return value == BattleType.Invalid ? null : c[(int)(byte)value - 1];
        }
    }
    public class BattleTypeBorder : IValueConverter
    {
        public static readonly BattleTypeBorder C = new BattleTypeBorder();
        static readonly SolidColorBrush[] c;
        static readonly SolidColorBrush[] c2;
        static BattleTypeBorder()
        {
            c = new SolidColorBrush[RomData.BATTLETYPES];
            c[0] = SBrushes.NewBrush(0x80555553);//normal
            c[1] = SBrushes.NewBrush(0x80562c2d);//fight
            c[2] = SBrushes.NewBrush(0x802a3f7e);//flying
            c[3] = SBrushes.NewBrush(0x80552b40);//poison
            c[4] = SBrushes.NewBrush(0x8054542c);//ground
            c[5] = SBrushes.NewBrush(0x80543f2c);//rock
            c[6] = SBrushes.NewBrush(0x80545400);//bug
            c[7] = SBrushes.NewBrush(0x802b2b53);//ghost
            c[8] = SBrushes.NewBrush(0x803f3f52);//steel
            c[9] = SBrushes.NewBrush(0x807f1500);//fire
            c[10] = SBrushes.NewBrush(0x802a3f69);//water
            c[11] = SBrushes.NewBrush(0x8040552d);//grass
            c[12] = SBrushes.NewBrush(0x80695413);//electric
            c[13] = SBrushes.NewBrush(0x806b2c41);//psychic
            c[14] = SBrushes.NewBrush(0x803f5469);//ice
            c[15] = SBrushes.NewBrush(0x803f2a69);//dragon
            c[16] = SBrushes.NewBrush(0x802a2a13);//dark
            c[17] = SBrushes.NewBrush(0x807f556c);//fairy
            c2 = new SolidColorBrush[RomData.BATTLETYPES];
            c2[0] = SBrushes.NewBrush(0xff555553);//normal
            c2[1] = SBrushes.NewBrush(0xff562c2d);//fight
            c2[2] = SBrushes.NewBrush(0xff2a3f7e);//flying
            c2[3] = SBrushes.NewBrush(0xff552b40);//poison
            c2[4] = SBrushes.NewBrush(0xff54542c);//ground
            c2[5] = SBrushes.NewBrush(0xff543f2c);//rock
            c2[6] = SBrushes.NewBrush(0xff545400);//bug
            c2[7] = SBrushes.NewBrush(0xff2b2b53);//ghost
            c2[8] = SBrushes.NewBrush(0xff3f3f52);//steel
            c2[9] = SBrushes.NewBrush(0xff7f1500);//fire
            c2[10] = SBrushes.NewBrush(0xff2a3f69);//water
            c2[11] = SBrushes.NewBrush(0xff40552d);//grass
            c2[12] = SBrushes.NewBrush(0xff695413);//electric
            c2[13] = SBrushes.NewBrush(0xff6b2c41);//psychic
            c2[14] = SBrushes.NewBrush(0xff3f5469);//ice
            c2[15] = SBrushes.NewBrush(0xff3f2a69);//dragon
            c2[16] = SBrushes.NewBrush(0xff2a2a13);//dark
            c2[17] = SBrushes.NewBrush(0xff7f556c);//fairy
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is BattleType) || ((BattleType)value) == BattleType.Invalid ? null : parameter == null ? c[(byte)value - 1] : c2[(byte)value - 1];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class BattleTypeMoveButton : Converter<BattleType>
    {
        public static readonly BattleTypeMoveButton C = new BattleTypeMoveButton();

        protected override object Convert(BattleType value)
        {
            return null;
        }
    }
}
