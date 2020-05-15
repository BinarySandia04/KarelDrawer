using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KarelDrawer
{
    class KarelClick
    {
        public Girdcoord coord;

        public Keys keyBinding;
        public Color lineColor;

        public bool valid = true;

        public KarelClick(Girdcoord coord)
        {
            this.coord = coord;
            this.keyBinding = Keys.None;
            this.lineColor = Color.Green;
        }

        public KarelClick(Girdcoord coord, Keys key, Color lineColor)
        {
            this.coord = coord;
            this.keyBinding = key;
            this.lineColor = lineColor;
        }
    }

    public class Girdcoord
    {
        public int x;
        public int y;

        public Girdcoord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
