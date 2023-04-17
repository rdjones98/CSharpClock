using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;  
using System.Runtime.InteropServices;
 
namespace CSharpClock
{
    public class Clock : Form
    {
		// Panel for Clock, Setting Timer, Actual Timer and Stopwatch
		private static FlowLayoutPanel fpClock = new FlowLayoutPanel();
		private FlowLayoutPanel fpSetTimer = new FlowLayoutPanel();
		private FlowLayoutPanel fpTimer = new FlowLayoutPanel();
		private FlowLayoutPanel fpStopwatch = new FlowLayoutPanel();
		private FlowLayoutPanel fpGrades = new FlowLayoutPanel();

   		private static Label lblTimer = new Label();
   		private static Label lblStopwatch = new Label();
		private static Label lblClock = new Label();
		private NumericUpDown spinTimer = new NumericUpDown();
		private NumericUpDown spinNumGrades = new NumericUpDown();

		// Flag to end timer & track stopwatch
		private static long endTime = 0;
		private static long stopWatchCntr = -1;

		// Popup menu
		private ContextMenuStrip PopupMenu = new ContextMenuStrip();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Clock());
        }
        public Clock()
        {
	        InitComponents();
        }
       
        private void InitComponents()
        {
        	this.Text = "Clock/Timer";
            this.ClientSize = new Size(240, 30);
			this.StartPosition = FormStartPosition.Manual;
			this.Location = new Point(0, 0);

			// Build Clock Panel 
			lblClock.Font = new Font("Calibri", 18);
			lblClock.AutoSize = true;
			fpClock.Controls.Add(lblClock);

			// Build Timer Panel
			lblTimer.AutoSize = true;
			lblTimer.Font = new Font("Calibri", 18);
			Button stopBtn = new Button();			
			stopBtn.Text="Stop";
			stopBtn.Height = 20;
			stopBtn.AutoSize=true;
			stopBtn.Click += new EventHandler(StopButton_Click);  
			fpTimer.Controls.Add(lblTimer);
			fpTimer.Controls.Add(stopBtn);
	
			// Build Set Timer Panel
			Label l = new Label();
			l.Text="Timer:";
			l.Padding = new Padding(6);
			l.AutoSize=true;
			spinTimer.Padding = new Padding(6);
			spinTimer.Value = 5;
			spinTimer.Maximum = 59;
			spinTimer.Minimum = 1;
			spinTimer.AutoSize=true;
			Button startBtn = new Button();			
			startBtn.Text="Start";
			startBtn.Height = 20;
			startBtn.AutoSize=true;
			startBtn.Click += new EventHandler(StartButton_Click);  
			fpSetTimer.Controls.Add( l);
			fpSetTimer.Controls.Add(spinTimer);
			fpSetTimer.Controls.Add(startBtn);

			// Build Stopwatch Panel
			lblStopwatch.AutoSize = true;
			lblStopwatch.Font = new Font("Calibri", 18);
			fpStopwatch.Controls.Add(lblStopwatch);

			// Build Grades Panel
			Label l2 = new Label();
			l2.Text="Num Grades:";
			l2.Padding = new Padding(6);
			l2.AutoSize=true;
			spinNumGrades.Padding = new Padding(6);
			spinNumGrades.Value = 20;
			spinNumGrades.Maximum = 100;
			spinNumGrades.Minimum = 1;
			spinNumGrades.AutoSize=true;
			Button gradesBtn = new Button();			
			gradesBtn.Text="Go";
			gradesBtn.Height = 20;
			gradesBtn.Width=20;
			gradesBtn.AutoSize=true;
			gradesBtn.Click += new EventHandler(GradesButton_Click);  
			fpGrades.Controls.Add( l2 );
			fpGrades.Controls.Add(spinNumGrades);
			fpGrades.Controls.Add(gradesBtn);

			// Build Popup Menu
			this.ContextMenuStrip = PopupMenu;
			ToolStripMenuItem TimerMenu = new ToolStripMenuItem("Timer");
			ToolStripMenuItem StopwatchMenu = new ToolStripMenuItem("Stop Watch");
			ToolStripMenuItem ClockMenu = new ToolStripMenuItem("Clock");
			ToolStripMenuItem GradesMenu = new ToolStripMenuItem("Grades");
			PopupMenu.Items.Add(TimerMenu);
			PopupMenu.Items.Add(StopwatchMenu);
			PopupMenu.Items.Add(ClockMenu);
			PopupMenu.Items.Add(GradesMenu);
			PopupMenu.Show();
			TimerMenu.Click += new System.EventHandler(this.TimerMenuItemClick);
			StopwatchMenu.Click += new System.EventHandler(this.StopwatchMenuItemClick);
			ClockMenu.Click += new System.EventHandler(this.ClockMenuItemClick);
			GradesMenu.Click += new System.EventHandler(this.GradesMenuItemClick);
			
			// Display clock
			this.Controls.Add(fpClock);

            // Handle resize
			this.SizeChanged += new EventHandler(Clock_SizeChanged);

            // Start Thread
			ThreadPool.QueueUserWorkItem(BackgroundTaskWithObject, this);           
        }
		private void StartButton_Click(object sender, EventArgs e)  
		{ 
			endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + 60000*(int)spinTimer.Value;
			
			Controls.Clear();
			Controls.Add(fpTimer);
		}
		private void StopButton_Click(object sender, EventArgs e)  
		{ 
			endTime = 0;
			
			Controls.Clear();
			Controls.Add(fpClock);
		}
		private void GradesButton_Click(object sender, EventArgs e)  
		{ 
			int numGrades = (int)spinNumGrades.Value;;
			Form f= new Form();
			f.Text="Grades";
			TableLayoutPanel p = new TableLayoutPanel();
			f.Size = new Size(600, 800);
			p.ColumnCount = 6;  
    		p.RowCount = numGrades;
			p.Size = new Size(f.Width,f.Height);
 
			f.Controls.Add(p); 
			for( int i=1; i<=numGrades;i++)
			{		
				Label l1 = new Label();
				l1.Text="Missed " + i;
				Label l2 = new Label();
				double gpa = (numGrades-i)*100.0/numGrades;
				l2.Text = "" + Convert.ToInt32(gpa);	
				p.Controls.Add(l1);
				p.Controls.Add(l2);
			}
			f.Show();
		}

		private void TimerMenuItemClick(object sender, EventArgs e)
		{
			stopWatchCntr  = -1;
			Controls.Clear();
			if( endTime == 0 )
				Controls.Add(fpSetTimer);
			else
				Controls.Add(fpTimer);
			
		}
		private void ClockMenuItemClick(object sender, EventArgs e)
		{
			stopWatchCntr  = -1;
			Controls.Clear();
			Controls.Add(fpClock);
		}
		private void GradesMenuItemClick(object sender, EventArgs e)
		{
			stopWatchCntr  = -1;
			Controls.Clear();
			Controls.Add(fpGrades);
		}


		private void StopwatchMenuItemClick(object sender, EventArgs e)
		{
			lblStopwatch.Text = "00:00:00";
			Controls.Clear();
			Controls.Add(fpStopwatch);
			stopWatchCntr = 0;
		}

		static void BackgroundTaskWithObject(Object theClock)  
		{
			while(true)
			{
				lblClock.Text = DateTime.Now.ToString("hh:mm tt");

				if( endTime != 0 )
				{
					long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					long timeLeft = endTime - now;
					if( timeLeft <=0 )
					{
						for( int i=0; i<3; i++)
					        Console.Beep(440, 100);
						endTime = 0;
						((Clock)theClock).Controls.Clear();
						((Clock)theClock).Controls.Add(fpClock);
					}
					else
					{
						TimeSpan ts = TimeSpan.FromMilliseconds(timeLeft);
						lblTimer.Text = ts.ToString(@"mm\:ss");
					}
				}

				if( stopWatchCntr > -1 )
				{
				    long s = stopWatchCntr % 60;
					long m = (stopWatchCntr / 60) % 60;
					long h = (stopWatchCntr / (60 * 60)) % 24;
					lblStopwatch.Text = String.Format("{0,2:00}:{1,2:00}:{2,2:00}", h,m,s);
					stopWatchCntr ++;
				}
				Thread.Sleep(1000);
				PreventSleep(); 
			}
		}
		private void Clock_SizeChanged(Object sender, EventArgs e) 
		{
   			int h = Height;
			double ratio = 1 + (h - 60)/60.0;
			Font f = new Font("Calibri", (int)(18*ratio));
			lblClock.Font = f;
			lblTimer.Font = f;
			lblStopwatch.Font = f;

			fpClock.Size = new Size(Width,Height);
			fpTimer.Size = new Size(Width,Height);
			fpStopwatch.Size = new Size(Width,Height);
		}
		public static void PreventSleep() {
             SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | 
				EXECUTION_STATE.ES_DISPLAY_REQUIRED | 
				EXECUTION_STATE.ES_SYSTEM_REQUIRED );
        }

        public static void AllowSleep() {
             SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
       }

	     [FlagsAttribute]
	        public enum EXECUTION_STATE : uint
	        {
	            ES_AWAYMODE_REQUIRED = 0x00000040,
	            ES_CONTINUOUS = 0x80000000,
	            ES_DISPLAY_REQUIRED = 0x00000002,
	            ES_SYSTEM_REQUIRED = 0x00000001
	            // Legacy flag, should not be used.
	            // ES_USER_PRESENT = 0x00000004
	        }
	
	        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    }
}