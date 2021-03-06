using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Net.NetworkInformation;

using System.Drawing;
using System.Windows.Forms;

using tsonamu;

namespace tsonamu
{
	class MainClass
	{
		public static NotifyIcon tray;

		public static System.Timers.Timer WriteWithDelay (String[] texts, double delay = 1)
		{
			Console.WriteLine(texts[0]);

			int i = 1;
			int p = texts.Length;

			System.Timers.Timer delaytimer = new System.Timers.Timer (delay * 1000);
			delaytimer.AutoReset = true;
			delaytimer.Elapsed += (object sender, ElapsedEventArgs e) => {
				if(i < p) {
					Console.WriteLine(texts[i]); i++;
				} else { delaytimer.Stop(); return; }
			};

			delaytimer.Start ();
			return delaytimer;
		}

		public static string TimeInBrackets() {
			string returnvalue = "[" + DateTime.Now.ToString ("HH:mm:ss") + "]";
			return returnvalue;
		}


		public static System.Timers.Timer monitorStatus(string hostname, string filename, bool log = true, double delay = 1)
		{
			LogWriter Logger = new LogWriter (filename);

			bool connection = true;
			int last_ping = 50;

			if (log == true) {
				Logger.LogText ("[" + DateTime.Now.ToString ("yyyy-MM-dd") + "] " + hostname, true);
			}

			ConsoleColor defaultcolor = Console.ForegroundColor;

			System.Timers.Timer monitortimer = new System.Timers.Timer (delay * 1000);
			monitortimer.AutoReset = true;
			monitortimer.Elapsed += (object sender, ElapsedEventArgs e) => {


				Ping pinging = new Ping ();
				try {
					PingReply pingstatus = pinging.Send (hostname);
					if(pingstatus.Status != IPStatus.Success) {
						throw(new Exception("Ping unsuccessful!"));
					}
					Console.WriteLine (TimeInBrackets () + " " + pingstatus.Status.ToString () + ": " + pingstatus.RoundtripTime.ToString () + "ms");
					if(log == true) { Logger.LogText (TimeInBrackets () + " " + pingstatus.Status.ToString () + ": " + pingstatus.RoundtripTime.ToString () + "ms", true); }

					if(connection == false) {
						tray.BalloonTipTitle = "tsonamu";
						tray.BalloonTipText = "Internet connection re-established!";
						tray.BalloonTipIcon = ToolTipIcon.Info;
						tray.ShowBalloonTip(5000);

						connection = true;
					}

					if((pingstatus.RoundtripTime / last_ping) > 4 && last_ping > 250) {
						tray.BalloonTipTitle = "tsonamu";
						tray.BalloonTipText = "Dramatic ping increase!";
						tray.BalloonTipIcon = ToolTipIcon.Warning;
						tray.ShowBalloonTip(5000);
					}


					if((pingstatus.RoundtripTime / last_ping) > 4 && last_ping > 250) {
						tray.BalloonTipTitle = "tsonamu";
						tray.BalloonTipText = "Ping has returned to normal!";
						tray.BalloonTipIcon = ToolTipIcon.Info;
						tray.ShowBalloonTip(5000);
					}


				} catch {
					Console.ForegroundColor = ConsoleColor.Red;


					if (Uri.CheckHostName (hostname) == UriHostNameType.Unknown) {
						Console.WriteLine (TimeInBrackets () + " Can't resolve hostname: " + hostname);
						if(log == true) { Logger.LogText(TimeInBrackets () + " DNS", true); }
					} else {
						Console.WriteLine (TimeInBrackets () + " Error!");
						if(log == true) { Logger.LogText(TimeInBrackets () + " ERR", true); }
					}

					if(connection == true) {
						tray.BalloonTipTitle = "tsonamu";
						tray.BalloonTipText = "Internet connection was lost!";
						tray.BalloonTipIcon = ToolTipIcon.Error;
						tray.ShowBalloonTip(5000);

						connection = false;
					}

					Console.ForegroundColor = defaultcolor;
				}


			};

			monitortimer.Start ();

			return monitortimer;

		}


		public static void Main (string[] args)
		{
			tray = new NotifyIcon();
			Icon appicon = Icon.ExtractAssociatedIcon (Assembly.GetExecutingAssembly ().Location);
			tray.Icon = appicon;
			tray.Visible = true;




			string hostname = "google.com";
			bool log = true;
			
			if (args.Length != 0) {
				string arguments = string.Join (" ", args, 0, args.Length);

				Regex regexHelp = new Regex(@"/\?");
				Match matchHelp = regexHelp.Match (arguments);
				if (matchHelp.Success)
				{
					Console.WriteLine ("tsonamu: the simplest of network availability monitorung utilities\n\nUsage:\n\ntsonamu.exe [-hostname url] [-nolog]\n\n-hostname   Hostname         This URL will be used to test internet connection. You can use any valid URL.\n-nolog      Do not log       The results will be printed to the console, but not saved to a text file.");
					return;
				}

				Regex regexHostname = new Regex("(?<=-hostname )(.*\\.[a-zA-Z]*)");
				Match matchHostname = regexHostname.Match(arguments);
				if (matchHostname.Success)
				{
					hostname = matchHostname.Value;
				}
				Regex regexLog = new Regex("(-nolog)");
				Match matchLog = regexLog.Match(arguments);
				if (matchLog.Success)
				{
					log = false;
				}
			}


			DirectoryInfo outputpath;

			if (args.Length != 0 && Directory.Exists (args [0].ToString ())) {
				outputpath = new DirectoryInfo (args [0].ToString ());
			} else {
				outputpath = new DirectoryInfo (Directory.GetCurrentDirectory ());
			}

			string logfile = outputpath.ToString () + "/tsonamu_log_" + DateTime.Now.ToString ("yyyy-MM-dd-HH-mm-ss") + ".txt";


			System.Timers.Timer writingtimer1 = WriteWithDelay (new string[]
				{
					"Welcome to TSONAMU!",
					"Loading secret libraries...",
					"Hiding critical evidence...",
					"Poisoning witnesses...\n",
					"OTACON: Snake, TSONAMU is ready to fire!",
					"                        \n                        \n             TSONAMU    \n                        \n        ox            oo\n        xx-----ox-----ox\n        xx-----xx-----ox\n        xx     II     ox\n               II       \n               II       \n               II       \n               II       \n              oooo      \n              ooxx      \n\n",
					"Press the return key to stop the program!\n\n"
				}, 0.5);

			while (writingtimer1.Enabled) {
				
			}


			//FileStream out_strm = new FileStream (outputpath.ToString () + "/tsonamu_log_" + DateTime.Now.ToString ("yyyy-MM-dd-HH-mm-ss") + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
			//StreamWriter out_wrtr = new StreamWriter (out_strm);

			//Console.SetOut (out_wrtr);

			Console.WriteLine ("[" + DateTime.Now.ToString ("HH:mm:ss") + "] Observation started!");

			if (log == true) {
				monitorStatus (hostname, logfile, true, 5);
			} else {
				monitorStatus (hostname, logfile, false, 5);
			}

			Console.ReadLine ();
			tray.Visible = false;
			tray.Dispose ();

		}
	}
}
