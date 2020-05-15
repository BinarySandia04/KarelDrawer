using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarelDrawer
{
    class DrawingUtils
    {
        Form1 mainForm;

        public DrawingUtils(Form1 mainForm)
        {
            this.mainForm = mainForm;
        }

        public void DrawGird(Graphics g, Pen p, int xSize, int ySize, int xOffset, int yOffset)
        {

            int sx = Form1.mfxs, sy = Form1.mfys;

            // yOffset lo pondré en 80
            
            // Tranzar lineas horizontales
            for(int x = xOffset; x < sx; x += xSize)
            {
                g.DrawLine(p, x, 0, x, sy);
            }

            for (int y = yOffset; y < sy; y += ySize)
            {
                g.DrawLine(p, 0, y, sx, y);
            }
        }
    }
}
