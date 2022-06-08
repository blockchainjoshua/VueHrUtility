using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VueHrUtilityV1.Models;

namespace VueHrUtilityV1
{

    public partial class Form1 : Form, IHostedService, IDisposable
    {

        //private const int CP_NOCLOSE_BUTTON = 0x200;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams myCp = base.CreateParams;
        //        myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
        //        return myCp;
        //    }
        //}
        bool isStarted = false;

        private System.Threading.Timer _timer = null;

        public Form1()
        {
            InitializeComponent();
            TrayMenuContext();
        }
        private void TrayMenuContext()
        {
            this.notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            //this.notifyIcon.ContextMenuStrip.Items.Add("Start", null, this.MenuStart_Click);
            //this.notifyIcon.ContextMenuStrip.Items.Add("Stop", null, this.MenuStop_Click);
            this.notifyIcon.ContextMenuStrip.Items.Add("Exit", null, this.MenuExit_Click);
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized
            //hide it from the task bar
            //and show the system tray icon (represented by the NotifyIcon control)
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }
        /// <summary>
        /// Show application whem double click icon in tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        /// <summary>
        /// Toogle Button Start and Stop the time hosted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ButtonCapture_Click(object sender, EventArgs e)
        //{

        //    if (!isStarted)
        //    {
        //        StartAsync(default);
        //        isStarted = true;
        //        //buttonCapture.Text = "Stop";
        //        this.notifyIcon.Text = "Vue HR Utility is Running";
        //        this.notifyIcon.ContextMenuStrip.Items[0].Enabled = false;
        //        this.notifyIcon.ContextMenuStrip.Items[1].Enabled = true;
        //        if (this.WindowState == FormWindowState.Normal)
        //        {
        //            Hide();
        //            notifyIcon.Visible = true;
        //            //notifyIcon.ShowBalloonTip(1000);
        //        }
        //    }
        //    else
        //    {
        //        StopAsync(default);
        //        isStarted = false;
        //        //buttonCapture.Text = "Start";
        //        this.notifyIcon.Text = "Vue HR Utility is not Running";
        //        this.notifyIcon.ContextMenuStrip.Items[0].Enabled = true;
        //        this.notifyIcon.ContextMenuStrip.Items[1].Enabled = false;
        //    }

        //}

        /// <summary>
        /// Screen Capture
        /// </summary>
        private void CaptureMyScreen(ConfigurationModel config)
        {
            try
            {
                int screenLeft = SystemInformation.VirtualScreen.Left;
                int screenTop = SystemInformation.VirtualScreen.Top;
                int screenWidth1 = SystemInformation.VirtualScreen.Width;
                int screenHeight1 = SystemInformation.VirtualScreen.Height;

                var dateTimeNow = DateTime.Now.ToString("MM-dd-yyyy hh mm ss tt");
                //Get Screen Width
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;

                //Get Screen Height
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                //Creating a new Bitmap object
                Bitmap captureBitmap = new Bitmap(screenWidth1, screenHeight1, PixelFormat.Format32bppArgb);

                //Creating a Rectangle object which will capture our Current Screen
                Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

                //Creating a New Graphics Object
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);

                //Copying Image from The Screen
                captureGraphics.CopyFromScreen(screenLeft, screenTop, 0, 0, captureBitmap.Size);

                var fiilePath = CreatePath(config.FilePath + config.LastName +", " +config.FirstName + "\\" + DateTime.Now.ToString("MMMM") + "\\" + DateTime.Now.ToString("dddd , MM-dd-yyyy"));

                if (Directory.Exists(fiilePath))
                {
                    //Saving the Image File
                    captureBitmap.Save(fiilePath + "\\" + dateTimeNow + ".jpg", ImageFormat.Jpeg);
                }

            }

            catch (Exception ex)
            {
              
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var config = JsonFileReader.Read<ConfigurationModel>("config.json");
            employeeId.Text = config.EmployeeId;
            employeeName.Text = config.LastName + " , " + config.FirstName;

            StartAsync(default);
            isStarted = true;
            //buttonCapture.Text = "Stop";
            this.notifyIcon.Text = "Vue HR Utility is Running";
            //this.notifyIcon.ContextMenuStrip.Items[0].Enabled = false;
            //this.notifyIcon.ContextMenuStrip.Items[1].Enabled = true;
        }

        void MenuStart_Click(object sender, EventArgs e)
        {
            StartAsync(default);
            isStarted = true;
            //buttonCapture.Text = "Stop";
            this.notifyIcon.Text = "Vue HR Utility is Running";
            //this.notifyIcon.ContextMenuStrip.Items[0].Enabled = false;
            //this.notifyIcon.ContextMenuStrip.Items[1].Enabled = true;
        }

        void MenuStop_Click(object sender, EventArgs e)
        {
            StopAsync(default);
            isStarted = false;
            //buttonCapture.Text = "Start";
            this.notifyIcon.Text = "Vue HR Utility is not Running";
            //this.notifyIcon.ContextMenuStrip.Items[0].Enabled = true;
            //this.notifyIcon.ContextMenuStrip.Items[1].Enabled = false;
        }

        void MenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new System.Threading.Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            //Get configuration from config.json file
            var config = JsonFileReader.Read<ConfigurationModel>("config.json");

            //Parse break out from config file
            TimeSpan breakOut = DateTime.Parse(config.BreakOut).TimeOfDay;

            //Parse break in from config file
            TimeSpan breakIn = DateTime.Parse(config.BreakIn).TimeOfDay;

            //Api call url with employee ID
            string url = config.ApiUrl + config.EmployeeId;

            //Api call to get attendance by EmployeeId as of today
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);

            //Deserialize api call result
            var attendance = JsonConvert.DeserializeObject<List<Attendance>>(result);

            //Check if attendance from api call is not null or empty
            if (attendance != null && attendance.Count() != 0)
            {
                //Check if employee is timed in
                var isTimedIn = attendance.Any(x => x.Status == "TimeIn");
                //Check if employee is timed out
                var isTimedOut = attendance.Any(x => x.Status == "TimeOut");

                var isBreakTIme = (DateTime.Now.TimeOfDay > breakOut) && (DateTime.Now.TimeOfDay < breakIn);

                if (isTimedIn && !isTimedOut && !isBreakTIme)
                {
                    //Check if the current time is not between beaktime from config file
                    CaptureMyScreen(config);
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private string CreatePath(string path)
        {


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static class JsonFileReader
        {
            public static T Read<T>(string filePath)
            {
                string text = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(text);
            }
        }


    }
}
