using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace AWSCredentialsClipboard
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Reading Clipboard");
			string clipboardText = GetClipboardData();
			string replacedString = Regex.Replace(clipboardText, @"[\r\n]+", " ");
			if(!string.IsNullOrEmpty(clipboardText))
			{
				Regex rx = new Regex(@"^(?=.*\baws_access_key_id\b)(?=.*\baws_secret_access_key\b)(?=.*\baws_session_token\b).+$");
				if (rx.IsMatch(replacedString)){
					string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
					string text = Regex.Replace(clipboardText, @"\[[0-9]+_[a-z | A-z]+\]", "[default]");
					try
					{
						File.WriteAllText(path + "\\.aws\\credentials", text);
						Console.WriteLine("AWS Settings Added");
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}
			}
		}

		private static string GetClipboardData()
		{
			try
			{
				string clipboardData = null;
				Exception threadException = null;

				Thread staThread = new Thread(
					delegate ()
					{
						try
						{
							clipboardData = Clipboard.GetText(TextDataFormat.Text);
						}
						catch (Exception ex)
						{
							threadException = ex;
						}
					});
				staThread.SetApartmentState(ApartmentState.STA);
				staThread.Start();
				staThread.Join();
				return clipboardData;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return string.Empty;
			}
		}
	}
}
