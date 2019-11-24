using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Net;
using System.Net.Sockets;

namespace Project_hleb
{
    public partial class Form1 : Form
    {
        static double Tm = 0;
        static double Tv = 0;
        static double Tt = 0;
        static double P;
        static double D;
        static double I;
        private double Tm0;
        private double Tv0;
        private double Tt0;
        private double F;
        private double G;
        private double K;
        private double C;
        private double L1;
        private double L2;
        private double v;
        private int time_rabota;
        static double deltaT = 0.1;
        static double y1;
        static double y2;
        static double y3;
        static double x1;
        static double x2;
        static double x3;
        static double k1 = 10;
        static double k2 = 10;
        static double U1;
        static double U2;
        static double U3;
        static double y1prev = 0;
        Thread Potok1;
        Thread Potok2;
        Thread Potok3;
        bool br;
        bool s;
        static string Tm1;
        static string Tv1;
        static string Tt1;
        public Form1()
        {
            InitializeComponent();
            Potok1 = new Thread(PotokFunction1);//Создаем первый поток
            Potok2 = new Thread(PotokFunction2);//Создаем второй поток
                                                // Potok3 = new Thread(PotokFunction3);//Создаем третий поток
            br = false;
        }
        private void Button1_Click_1(object sender, EventArgs e)
        {

            timer_rabota.Enabled = true;

            P = Convert.ToDouble(textBox15.Text);
            D = Convert.ToDouble(textBox14.Text);
            I = Convert.ToDouble(textBox13.Text);
            // Tm0 = Convert.ToDouble(textBox12.Text);
            Tv0 = Convert.ToDouble(textBox16.Text);
            // Tt0 = Convert.ToDouble(textBox17.Text);
            F = Convert.ToDouble(textBox4.Text);
            G = Convert.ToDouble(textBox5.Text);
            K = Convert.ToDouble(textBox6.Text);
            C = Convert.ToDouble(textBox7.Text);
            L1 = Convert.ToDouble(textBox8.Text);
            L2 = Convert.ToDouble(textBox9.Text);
            // v = Convert.ToDouble(textBox10.Text);///ввод значений из ячеек
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            textBox9.Enabled = false;
            textBox10.Enabled = false;
            textBox12.Enabled = false;
            textBox13.Enabled = false;
            textBox14.Enabled = false;
            textBox15.Enabled = false;
            textBox16.Enabled = false;
            textBox17.Enabled = false;
            button1.Enabled = false;
            if (!Potok1.IsAlive)
            {
                //Potok1.Join();
                //Potok1.Abort();
                Potok1.Start();// если поток не прерван (или завершён корректно), то запускаем поток 
            }
            if (!Potok2.IsAlive)
            {
                //Potok2.Join();
                //Potok2.Abort();
                Potok2.Start();
            }
            /* if (!Potok3.IsAlive)
             {
                 //Potok2.Join();
                 //Potok2.Abort();
                 Potok3.Start();
             }*/
            s = false;
            if (Potok1.ThreadState == System.Threading.ThreadState.Suspended) Potok1.Resume();// если поток  прерван, то возобновляем 
            if(Potok2.ThreadState == System.Threading.ThreadState.Suspended) Potok2.Resume();
            //if (Potok3.ThreadState == System.Threading.ThreadState.Suspended) Potok3.Resume();
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            timer_rabota.Enabled = false;
            button1.Enabled = true;
            s = true;
        }

        private void timer_rabota_Tick(object sender, EventArgs e)
        {
            time_rabota++;

            textBox1.Text = Tm.ToString();
            textBox2.Text = Tv.ToString();
            textBox3.Text = Tt.ToString();
            chart1.Series[0].Points.AddXY(time_rabota, Tm);
            chart1.Series[1].Points.AddXY(time_rabota, Tv);
            chart1.Series[2].Points.AddXY(time_rabota, Tt);
        }
        void PotokFunction1()//Функция для нахождения значений параметров
        {
            while (!br)
            {
                y1 = Tv - Tv0;
                U1 = P * y1;
                U2 = I * y1 * deltaT + U2;
                U3 = (y1 * y1prev) * D / deltaT;
                Tt = U1 + U2 + U3;
                Tm = x1 * deltaT + Tm;
                y3 = Tt * K * F + Tm * C * G - Tv * (C * G + K * F);
                y2 = y3 - k2;
                Tv = y2 / L1 * deltaT + Tv;
                x2 = ((K * F) / L2) * x3;
                x3 = Tv - Tm;
                x1 = x2 - k1;
                Tv = P + D;
                Tm1 = Tm.ToString();
                Tv1 = Tv.ToString();
                Tt1 = Tt.ToString();
                if (s) Potok1.Suspend();
            }
        }
        
        void PotokFunction2()//Функция для клиента, который передаёт данные серверу
        {
            while (!br)
            {
                try
                {
                    Linkoln(904);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.ReadLine();
                }
                if (s) Potok2.Suspend();
            }
        }

        /*static void SendMessageFromSocket(int port)
         {
             // Буфер для входящих данных
             byte[] bytes = new byte[1024];

             // Соединяемся с удаленным устройством

             // Устанавливаем удаленную точку для сокета
             IPHostEntry ipHost = Dns.GetHostEntry("localhost");
             IPAddress ipAddr = ipHost.AddressList[0];
             IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

             Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

             // Соединяем сокет с удаленной точкой
             sender.Connect(ipEndPoint);

             string messageTm = Tm1;

             Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString() + "\n");
             byte[] msgTm = Encoding.UTF8.GetBytes(messageTm);

             // Отправляем данные через сокет
             int bytesSentTm = sender.Send(msgTm);


             string messageTv = Tv1;

             Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString() + "\n");
             byte[] msgTv = Encoding.UTF8.GetBytes(messageTv);

             int bytesSentD = sender.Send(msgTv);

             string messageTt = Tt1;

             Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString() + "\n");
             byte[] msgTt = Encoding.UTF8.GetBytes(messageTt);

             int bytesSentTt = sender.Send(msgTt);
             // Освобождаем сокет
             sender.Shutdown(SocketShutdown.Both);
             sender.Close();
         }
         */

        
            static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            /*Класс Socket для сетевого взаимодействия
             * AddressFamily: возвращает все адреса, используемые сокетом, InterNetwork: адрес по протоколу IPv4
             * SocketType: возвращает тип сокета, Stream: обеспечивает надежную двустороннюю передачу данныx
             * ProtocolType: возвращает одно из значений перечисления*/

            static void Linkoln(int port)
            {

                socket.Connect("127.0.0.1", 904);//Подключение к серверу
                string message = Tv1;//Считываем сообщение пользователя
                string message1 = Tm1;//Считываем сообщение пользователя    
                byte[] buffer = Encoding.UTF8.GetBytes(message);//Кодировка массива байтов
                socket.Send(buffer);//Отправка  
                while (true)
                {
                    Console.ReadLine();
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();


            }

        private void Form1_Leave(object sender, EventArgs e)
        {
            br = true;
        }
    }
    
    
}




