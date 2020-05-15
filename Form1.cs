using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KarelDrawer
{
    public partial class Form1 : Form
    {
        public static Form mainForm;
        DrawingUtils drawingUtils;

        public static int mfxs = 1920, mfys = 1080;

        List<KarelClick> karelMovementActions = new List<KarelClick>();

        public Form1()
        {
            // Instancias y cosas
            mainForm = this;

            drawingUtils = new DrawingUtils(this);

            // Eventos
            MouseClick += MouseClicker;

            // Opacidad
            Opacity = .3;

            // Iniciar componente
            InitializeComponent();

            // Ajustar posicion
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            mainForm.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            // Pintar
            RefreshMainPanel();
        }

        private void RefreshMainPanel()
        {
            MainPanel.Refresh();
        }

        private int xOffset = -8;
        private int yOffset = 74;

        private int gxSize = 24;
        private int gySize = 24;

        private bool nextInvalid = false;

        private void setNextInvalid()
        {
            nextInvalid = true;
        }

        private void KeyFuncDown(object sender, KeyEventArgs e)
        {
            MainPanel.Refresh();

            // Eventos de la app
            if (e.KeyCode == Keys.Escape)
            {
                Clipboard.SetText(generateCode());

                mainForm.Close();
                return;
            }

            if(e.KeyCode == Keys.Back)
            {
                karelMovementActions = new List<KarelClick>();
                RefreshMainPanel();
                return;
            }

            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;

            Color color = Color.Blue;

            if (e.KeyCode == Keys.E) color = Color.Red;
            if (e.KeyCode == Keys.T)
            {
                color = Color.Orange;
                setNextInvalid();
            }

            if (color != Color.Blue)
            {
                karelMovementActions.Add(new KarelClick(getCoordsFromPixel(x, y), e.KeyCode, color));
                RefreshMainPanel();
            }
        }

        private Girdcoord getCoordsFromPixel(int x, int y)
        {
            int fx = (x - xOffset) / gxSize;
            int fy = (y - yOffset) / gySize;

            return new Girdcoord(fx, fy);
        }

        private void MainPanelPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen girdPen = new Pen(Color.Red, 1);

            Brush br = new SolidBrush(System.Drawing.Color.Red);

            // Hacer un dibujo de una gird
            drawingUtils.DrawGird(g, girdPen, gxSize, gySize, xOffset, yOffset);

            for(int i = 0; i < karelMovementActions.Count - 1; i++)
            {
                if(karelMovementActions[i + 1].valid)
                {
                    Pen pathpen = new Pen(karelMovementActions[i + 1].lineColor, 4);
                    g.DrawLine(pathpen, karelMovementActions[i].coord.x * gxSize + xOffset + gxSize / 2,
                                            karelMovementActions[i].coord.y * gySize + yOffset + gySize / 2,
                                        karelMovementActions[i + 1].coord.x * gxSize + xOffset + gxSize / 2,
                                        karelMovementActions[i + 1].coord.y * gySize + yOffset + gySize / 2);
                }

                if(i == karelMovementActions.Count - 2)
                {
                    g.DrawEllipse(new Pen(Color.Red, 1), new RectangleF(
                        karelMovementActions[i + 1].coord.x * gxSize + xOffset,
                        karelMovementActions[i + 1].coord.y * gySize + yOffset,
                        gxSize,
                        gySize
                        ));
                }
            }
        }



        private void MouseClicker(object sender, MouseEventArgs e)
        {
            Girdcoord coord = getCoordsFromPixel(e.X, e.Y);

            // Evitar hacer lineas diagonales pq sino esto peta

            if(karelMovementActions.Count > 0)
            {
                int i = karelMovementActions.Count - 1;
                if(!nextInvalid) if (coord.x != karelMovementActions[i].coord.x && coord.y != karelMovementActions[i].coord.y) return;
            }

            KarelClick nextKarelClick = new KarelClick(coord);

            if (nextInvalid)
            {
                nextKarelClick.valid = false;
                nextInvalid = false;
            }
            karelMovementActions.Add(nextKarelClick);

            RefreshMainPanel();
        }

        private string generateCode()
        {
            string res = "function left(){while(notFacingEast())turnRight();mov();}function right(){while(notFacingWest())turnRight();mov();}function up(){while(notFacingNorth())turnRight();mov();}function down(){while(notFacingSouth())turnRight();mov();}function mov(){if(frontIsBlocked()) removeWall();if(beepersPresent()) pickBeeper();move();if(trayPresent()){if(trayIsMine()){while(beepersInBag()) putBeeperInTray();}}}";

            res += "\nfunction main(){\n   ";

            for(int i = 0; i < karelMovementActions.Count - 1; i++)
            {
                int r = i + 1;

                if (!karelMovementActions[r].valid) continue;

                Girdcoord difference = new Girdcoord(karelMovementActions[i].coord.x - karelMovementActions[r].coord.x,
                                                     karelMovementActions[i].coord.y - karelMovementActions[r].coord.y);

                int dist = Math.Abs(difference.x) + Math.Abs(difference.y);

                res += "repeat(" + dist + "){\n   ";

                if (difference.x > 0)
                {
                    // Izquierda
                     res += "right();";
                }
                else if(difference.x < 0)
                {
                    // derecha
                    res += "left();";
                }
                else if(difference.y > 0)
                {
                    // abajo
                    res += "up();";
                }
                else if(difference.y < 0)
                {
                    // arriba
                    res += "down();";
                }

                res += "\n}";

                if (karelMovementActions[r].keyBinding == Keys.E) res += "if(exitPresent())exit();";
                if (karelMovementActions[r].keyBinding == Keys.T) res += "teleport();";
            }

            res += "\n}";

            return res;
        }
    }
}
