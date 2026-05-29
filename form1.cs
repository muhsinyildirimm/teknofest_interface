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
using System.Net.Sockets; //raspberry pi dan veri çekmek için 
using System.Threading;
using System.Net.Configuration;
using static Xamarin.Forms.Internals.Profile;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.IO;
using System.Text;


using LibVLCSharp.Shared;


namespace WindowsFormsApp12
{

    public partial class Form1 : Form
    {
        sbyte progressBarRed = 2; //progressbar kırmızı renk için. 

        //kamera

        public LibVLC _libVLC;
        public MediaPlayer _mp;
        public Media media;

        public Size oldVideoSize;
        public Size oldFormSize;
        public Point oldVideoLocation;



        string[] array = null;
        string readdata = null;
        private TcpClient client;
        private NetworkStream stream;
        private Thread listenThread = null;
        private bool listening = false;
        private bool connection = false;
        private double n;
        private char letter;
        private string numberText;
        string arrayElement;
        private int trackBarValue = 0;
        private int trackBarValue_lev = 0;
        private double sari = 0;
        private bool is100 = false;
        private bool is48 = false;
        private bool isAcil = false;

        private bool trackBarValueChanged = false;
        private bool trackBarValueChanged_lev = false;

        private StreamWriter sw; 
        StringBuilder sb = new StringBuilder();
        private string zaman = "0";
        private string ivme = "0";
        private string hiz = "0";
        private string konum = "0";

        private void Form1_Load(object sender, EventArgs e)
        {
            //TRACK BAR KONTROLÜ

            trackbar_RL.Minimum = -50;
            trackbar_RL.Maximum = 50;

            trackbar_up.Maximum = 50;

            trackbar_ROLL.Maximum = 90;
            trackbar_ROLL.Minimum = -90;

            trackbar_PITCH.Maximum = 90;
            trackbar_PITCH.Minimum = -90;

            trackbar_YAW.Maximum = 90;
            trackbar_YAW.Minimum = -90;

            //kamera
            Core.Initialize();
            oldVideoSize = videoView1.Size;
            oldFormSize = this.Size;
            oldVideoLocation = videoView1.Location;
            //VLC stuff
            _libVLC = new LibVLC();
            _mp = new MediaPlayer(_libVLC);
            videoView1.MediaPlayer = _mp;

            string dosyaYolu = @"C:\Users\Muhsin YILDIRIM\Desktop\veriler.csv";
            sw = new StreamWriter(dosyaYolu, false);


        }

        public Form1()
        {
            InitializeComponent();

            //kırmızı renk değişikliğini sağlıyor
            ModifyProgressBarColor.SetState(progress_tempt1, progressBarRed);
            ModifyProgressBarColor.SetState(progress_tempt2, progressBarRed);
            ModifyProgressBarColor.SetState(progress_tempt3, progressBarRed);
            ModifyProgressBarColor.SetState(progress_tempt4, progressBarRed);
            ModifyProgressBarColor.SetState(progress_tempt5, progressBarRed);
            ModifyProgressBarColor.SetState(progress_tempt6, progressBarRed);


        }




        private void button_Connect_Click(object sender, EventArgs e)
        {
            if (!connection)
            {
                client = new TcpClient();
                try
                {
                    client.Connect("192.168.1.104", 5560);
                    stream = client.GetStream();
                    label_status.Text = "Status: Baglandi";
                    connection = true;
                }
                catch (Exception ex)
                {
                    label_status.Text = "Status: Baglanamadi";
                    connection = false;
                }
            }


        }


        private void button_Speed_Click(object sender, EventArgs e)
        {

            if (connection)
            {
                listening = true;
                isAcil = false;
                listenThread = new Thread(ListenForData);
                listenThread.Start();

                byte[] data = Encoding.ASCII.GetBytes("GET");
                stream.Write(data, 0, data.Length);

                trackBar_speedup.Value = 20;


            }

        }

        private void ListenForData()
        {

            Invoke(new Action(() => { label_status.Text = "Status: Listen icine girdi"; }));
            while (listening)
            {


                
                if (stream.DataAvailable)
                {
                    try
                    {
                        zaman = Convert.ToString((Convert.ToInt32(label_zaman.Text)));
                        ivme = Convert.ToString((Convert.ToInt32(label_ivme.Text)));
                        hiz = Convert.ToString((Convert.ToInt32(progress_hiz.Value)));
                        konum = Convert.ToString((Convert.ToInt32(progress_mesafe.Value)));
                    }
                    catch
                    {
                        zaman = "0";
                        ivme = "0";
                        hiz = "0";
                        konum = "0";
                    }


                    //sb.Append(zaman + "," + ivme + "," + hiz + "," + konum);
                    //sw.WriteLine(sb.ToString());
                    //sb.Clear();

                    byte[] data = new byte[1024];
                    int bytesRead = stream.Read(data, 0, data.Length);
                    string message = Encoding.ASCII.GetString(data, 0, bytesRead);
                    UpdateLabel(message);

                    array = message.Split(',');


                    if (textBox2.Text == "Unknown Command")
                    {
                        Invoke(new Action(() => { textBox2.Text = "abc"; }));
                    }

                    else
                    {
                        Invoke(new Action(() => { textBox1.Text = Convert.ToString(array.Length); }));

                        if (array.Length > 1)
                        {

                            for (int i = 0; i < array.Length; i++)
                            {
                                char letter = 'a';
                                string numberText = "77";

                                string arrayElement = array[i];


                                if (string.IsNullOrEmpty(arrayElement)) continue;

                                letter = arrayElement[0];


                                if (!char.IsLetter(letter)) continue;


                                numberText = arrayElement.Substring(1);

                                double number;
                                if (!double.TryParse(numberText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out number)) continue;


                                switch (letter)
                                {
                                    case 'a':
                                        Invoke(new Action(() =>
                                        {
                                            label_sayac.Text = "Sayac: " + Convert.ToString(number);
                                        }));
                                        break;

                                    case 'c':
                                        Invoke(new Action(() =>
                                        {
                                            progress_mesafe.Value = (long)number;
                                            progress_mesafe.Text = Convert.ToString(number);
                                        }));
                                        break;

                                    case 'e':
                                        sari = number;
                                        break;

                                    case 'b':
                                        Invoke(new Action(() =>
                                        {
                                            label_zaman.Text = Convert.ToString(number);
                                        }));
                                        break;


                                    case 'd':
                                        Invoke(new Action(() =>

                                        {
                                            if (number > 100)
                                            {
                                                progress_hiz.Maximum = 150;
                                            }
                                            progress_hiz.Value = (long)number;
                                            progress_hiz.Text = Convert.ToString(number);

                                        }));
                                        break;

                                    case 'f':
                                        Invoke(new Action(() =>
                                        {
                                            //trackbar_RL.Value = (int)number;
                                            label_RLvalue.Text = Convert.ToString(number);

                                        }));
                                        break;
                                    case 'g':
                                        Invoke(new Action(() =>
                                        {
                                            //trackbar_up.Value = (int)number;
                                            lbl_upValue.Text = Convert.ToString(number);

                                        }));
                                        break;
                                    case 'h':
                                        Invoke(new Action(() =>
                                        {
                                            //trackbar_ROLL.Value = (int)number;
                                            labeltrackbar_ROLL.Text = Convert.ToString(number);
                                        }));
                                        break;
                                    case 'i':
                                        Invoke(new Action(() =>
                                        {
                                           // trackbar_PITCH.Value = (int)number;
                                            labeltrackbar_PITCH.Text = Convert.ToString(number);
                                        }));
                                        break;
                                    case 'j':
                                        Invoke(new Action(() =>
                                        {
                                            //trackbar_YAW.Value = (int)number;
                                            labeltrackbar_YAW.Text = Convert.ToString(number);
                                        }));
                                        break;

                                    case 'k':
                                        Invoke(new Action(() =>
                                        {
                                            progress_X.Value = (long)number;
                                            progress_X.Text = Convert.ToString(number);
                                        }));
                                        break;

                                    case 'l':
                                        Invoke(new Action(() =>
                                        {
                                            progress_Y.Value = (long)number;
                                            progress_Y.Text = Convert.ToString(number);
                                        }));
                                        break;

                                    case 'm':
                                        Invoke(new Action(() =>
                                        {
                                            progress_Z.Value = (long)number;
                                            progress_Z.Text = Convert.ToString(number);
                                        }));
                                        break;

                                    //SICAKLIK
                                    case 'o':
                                        Invoke(new Action(() =>
                                        {
                                            if (number > 100)
                                            {
                                                number = 24;
                                            }
                                            lbl_tempt1.Text = Convert.ToString(number);
                                            progress_tempt1.Value = (int)(number);

                                        }));
                                        break;
                                    case 'p':
                                        Invoke(new Action(() =>
                                        {
                                            if (number > 100)
                                            {
                                                number = 24;
                                            }
                                            lbl_tempt2.Text = Convert.ToString(number);
                                            progress_tempt2.Value = (int)(number);
                                        }));
                                        break;
                                    case 'q':
                                        Invoke(new Action(() =>
                                        {
                                            if (number > 100)
                                            {
                                                number = 24;
                                            }
                                            lbl_tempt3.Text = Convert.ToString(number);
                                            progress_tempt3.Value = (int)(number);
                                        }));
                                        break;
                                    case 'r':
                                        Invoke(new Action(() =>
                                        {
                                            if (number > 100)
                                            {
                                                number = 24;
                                            }
                                            lbl_tempt4.Text = Convert.ToString(number);
                                            progress_tempt4.Value = (int)(number);
                                        }));
                                        break;
                                    case 's':
                                        Invoke(new Action(() =>
                                        {
                                            if (number > 100)
                                            {
                                                number = 24;
                                            }
                                            lbl_tempt5.Text = Convert.ToString(number);
                                            progress_tempt5.Value = (int)(number);
                                        }));
                                        break;
                                    case 't':
                                        Invoke(new Action(() =>
                                        {
                                            if(number > 100)
                                            {
                                                number = 24;
                                            }
                                            lbl_tempt6.Text = Convert.ToString(number);
                                            progress_tempt6.Value = (int)(number);

                                        }));
                                        break;

                                }
                                control();


                            }

                        }
                    }


                }


                // Bekleme süresi (milisaniye cinsinden)
                int sleepTime = 100;

                // Eğer emergency butonuna basıldıysa bekleme süresi 10 kat arttırılır
                if (!listening)
                {
                    sleepTime *= 10;
                }
                Thread.Sleep(sleepTime);

            }
        }

        private void UpdateLabel(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateLabel), message);
            }
            else
            {
                textBox2.Text = message;
            }
        }


        private void control()
        {
            

            if (sari > 0 && !is100)
            {
                Invoke(new Action(() =>
                {
                    label_sari.Text = "Son 100 metre ";

                }));
                is100 = true;

            }

            if (sari > 10 && Convert.ToDouble(progress_hiz.Text) > 15 && !is48)
            // degistir sari>50 hiz>100
            {
                Invoke(new Action(() =>
                {
                    label_sari.Text = "Son 48 metre ";

                }));
                is48 = true;
            }

        }



        private void trackbar_RL_ValueChanged(object sender, EventArgs e)
        {
            label_RLvalue.Text = trackbar_RL.Value.ToString();
        }

        private void trackbar_up_ValueChanged(object sender, EventArgs e)
        {
            lbl_upValue.Text = trackbar_up.Value.ToString();
        }

        private void button_Emergency_Click(object sender, EventArgs e)
        {
            listening = false;
            byte[] data = Encoding.ASCII.GetBytes("ACIL");
            stream.Write(data, 0, data.Length);
            label_status.Text = "Status: Acil durum acildi";
            listening = true;
            
        }

        private void button_frenkapat_Click(object sender, EventArgs e)
        {
            listening = false;
            byte[] data = Encoding.ASCII.GetBytes("BACK");
            stream.Write(data, 0, data.Length);
            label_status.Text = "Status: Fren geri cekildi";
            listening = true;
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            listening = false; // stop the ListenForData loop
            connection = false;

            try
            {
                byte[] data = Encoding.ASCII.GetBytes("EXIT");
                stream.Write(data, 0, data.Length);

                label_status.Text = "Status: Baglanti Basarili olarak Durduruldu";
            }
            catch (Exception ex)
            {
                label_status.Text = "Status: Durdurulacak Baglanti YOK";
            }

            if (client != null) client.Close(); // close the TcpClient object
            if (stream != null) stream.Close(); // close the NetworkStream object
            sw.Close();

        }

        private void button_kill_Click(object sender, EventArgs e)
        {
            listening = false; // stop the ListenForData loop
            connection = false;

            try
            {
                byte[] data = Encoding.ASCII.GetBytes("KILL");
                stream.Write(data, 0, data.Length);
                label_status.Text = "Status: Baglanti Basarili olarak Kesildi";
            }
            catch (Exception ex)
            {
                label_status.Text = "Status: Kesilecek Baglanti YOK";
            }

            if (client != null) client.Close(); // close the TcpClient object
            if (stream != null) stream.Close(); // close the NetworkStream object
            sw.Close();

        }


        private void trackBar_speedup_ValueChanged(object sender, EventArgs e)
        {
            trackBarValue = trackBar_speedup.Value;
            trackBarValueChanged = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (trackBarValueChanged)
            {
                label_trackValue.Text = trackBarValue.ToString();
                trackBarValueChanged = false;
                listening = false;
                byte[] data = Encoding.ASCII.GetBytes("ITKI_" + trackBarValue);

                stream.Write(data, 0, data.Length);

                label_status.Text = "Status: Itki degeri: " + trackBarValue;
                label_status2.Text = Convert.ToString(listenThread.ThreadState) + ", " + Convert.ToString(listenThread.IsAlive);

                listening = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listening = false;
            byte[] data = Encoding.ASCII.GetBytes("ACIL");
            stream.Write(data, 0, data.Length);
            label_status.Text = "Status: Acil Durum aktiflestirildi";
            listening = true;
            label_status2.Text = Convert.ToString(listenThread.ThreadState) + ", " + Convert.ToString(listenThread.IsAlive);
        }

        private void button_camera_Click(object sender, EventArgs e)
        {
            _mp.Play(new Media(_libVLC, new Uri("rtsp://admin:123456@192.168.9.103")));
        }

        private void trackbar_ROLL_ValueChanged(object sender, EventArgs e)
        {
            labeltrackbar_ROLL.Text = trackbar_ROLL.Value.ToString();
        }

        private void trackbar_YAW_ValueChanged(object sender, EventArgs e)
        {
            labeltrackbar_YAW.Text = trackbar_YAW.Value.ToString();
        }

        private void trackbar_PITCH_ValueChanged(object sender, EventArgs e)
        {
            labeltrackbar_PITCH.Text = trackbar_PITCH.Value.ToString();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (trackBarValueChanged_lev)
            {
                label_trackValue_lev.Text = trackBarValue_lev.ToString();
                trackBarValueChanged_lev = false;
                listening = false;
                byte[] data = Encoding.ASCII.GetBytes("LEV_" + trackBarValue_lev);

                stream.Write(data, 0, data.Length);

                label_status.Text = "Status: Lev degeri: " + trackBarValue_lev;
                label_status2.Text = Convert.ToString(listenThread.ThreadState) + ", " + Convert.ToString(listenThread.IsAlive);

                listening = true;
            }
        }

        private void trackBar_levitation_ValueChanged(object sender, EventArgs e)
        {
            trackBarValue_lev = trackBar_levitation.Value;
            trackBarValueChanged_lev = true;
        }
    }
    // PROGRESSBAR KIRMIZI RENK İÇİN KOD
    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
