using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 *  ノード数が多くなると崩れる
 *  リンクは総ノードへのリンクとなる仕様にしている
 */

namespace Sample_SpringModel
{
    public partial class Form1 : Form
    {
        Graphics g;
        Node nd1;
        ArrayList al_nodes;
        //ArrayList al_links;

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            g = this.panel1.CreateGraphics();
            al_nodes = new ArrayList();
        }

        async private void button1_Click_1(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                HeavyMethod1(g);
            });
        }


        public void HeavyMethod1(Graphics g)
        {
            nd1 = new Node();

            al_nodes.Add(nd1);

            float length_s = 150f;
            float distance = 0.0f;
            float distance_x = 0.0f;
            float distance_y = 0.0f;
            float force = 0.0f;
            float force_x = 0.0f;
            float force_y = 0.0f;

            double sum_of_energy_tmp = 0;

            do
            {
                double sum_of_energy = 0;

                foreach (Node nd in al_nodes)
                {
                    force_x = 0;
                    force_y = 0;

                    foreach (Node nd_tmp in al_nodes)
                    {
                        if (nd == nd_tmp)
                        {
                            continue;
                        }
                        //                    力:= 力 + 定数 / 距離（ノード1, ノード2) ^2  // クーロン力()

                        distance_x = nd.rec.X - nd_tmp.rec.X;
                        distance_y = nd.rec.Y - nd_tmp.rec.Y;
                        distance = (float)Math.Sqrt(distance_x * distance_x + distance_y * distance_y);

                        if (distance <= 1)
                        {
                            distance = length_s;
                        }

                        force = (float)0.2 / distance / distance;

                        force_x = force_x - force * distance_x / distance;
                        force_y = force_y - force * distance_y / distance;
                    }

                    foreach (Node nd_tmp in al_nodes)
                    {
                        if (nd == nd_tmp)
                        {
                            continue;
                        }

                        //                    力:= 力 + バネ定数 * (距離(ノード1, ノード2) - バネの自然長)
                        distance_x = nd.rec.X - nd_tmp.rec.X;
                        distance_y = nd.rec.Y - nd_tmp.rec.Y;
                        distance = (float)Math.Sqrt(distance_x * distance_x + distance_y * distance_y);

                        if (distance <= 1)
                        {
                            distance = length_s;
                        }

                        force = (float)((float)0.1 * (distance - length_s));

                        force_x = force_x - force * distance_x / distance;
                        force_y = force_y - force * distance_y / distance;
                    }

                    //
                    //ノード１の速度 := (ノード1の速度 +　微小時間 * 力 / ノード1の質量) * 減衰定数
                    //ノード１の位置:= ノード1の位置 + 微小時間 * ノード1の速度
                    nd.rec.X = nd.rec.X + (int)(0.95 * force_x);
                    nd.rec.Y = nd.rec.Y + (int)(0.95 * force_y);

                    //運動エネルギーの合計:= 運動エネルギーの合計 + ノード1の質量 * ノード1の速度 ^ 2

                    sum_of_energy = sum_of_energy + (double)5 * (force_x * force_x + force_y * force_y);

                }
                try
                {
                    // Refresh
                    g.Clear(Color.White);

                    // repaint
                    foreach (Node nd in al_nodes)
                    {
                        nd.repainting(g);
                    }

                }
                catch
                {
                    Thread.Sleep(200);
                    continue;
                }

                Thread.Sleep(200);

                if (sum_of_energy < 7 * al_nodes.Count || sum_of_energy == sum_of_energy_tmp)
                {
                    break;
                }
                sum_of_energy_tmp = sum_of_energy;
            } while (true);
        }

        class Node
        {
            Random rand = new System.Random();
            public Rectangle rec;
            public double vx = 0.0;
            public double vy = 0.0;
            public double weight = 1.0;

            public Node()
            {
                rec = new Rectangle(rand.Next(100, 400), rand.Next(100, 400), 10, 10);
                vx = 0.0;
                vy = 0.0;
            }

            public void repainting(Graphics g)
            {
                if (rec.X <= 0)
                {
                    rec.X = 1;
                }
                if (rec.X >= 500)
                {
                    rec.X = 490;
                }
                if (rec.Y <= 0)
                {
                    rec.Y = 1;
                }
                if (rec.Y >= 500)
                {
                    rec.Y = 490;
                }

                g.DrawEllipse(new Pen(Brushes.DeepSkyBlue), rec);
            }

        }
 
    }
}
