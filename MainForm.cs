using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
//using JsonFileUpdater;

namespace SerialJsonFilesSender
{
    public partial class MainForm : Form
    {
        private int _fileDelayMs;
        private string selectedFolderPath;
        private SerialPort serialPort;
        //private FileSystemWatcher fileWatcher;
        private string info;
        //private bool FileWatcherEnabled = false;
        private System.Timers.Timer timer;
        private int _resendIntervalMs;
        SimpleLogger logger;
        private bool isSending;
        private int _baudrate;
        //private int _writeTimeOutMs;

        public MainForm()
        {
            logger = new SimpleLogger();
            logger.LogInfo("Application started.");
            InitializeComponent();
            cmbPort.DataSource = SerialPort.GetPortNames();
            timer = new System.Timers.Timer();
            logger.LogInfo("Finished initialization.");
            try
            {
                _fileDelayMs = Properties.Settings.Default.FileSendDelayMs;
                _resendIntervalMs = Properties.Settings.Default.ResendIntervalMs;
                _baudrate = Properties.Settings.Default.BaudRate;
                //_writeTimeOutMs = Properties.Settings.Default.WriteTimeout;
            }
            catch (Exception)
            {
                _fileDelayMs = 95;
                _resendIntervalMs = 900;
                _baudrate = 9600;
                //_writeTimeOutMs = 100;
            }
        }

        private void InitializeSerialPort()
        {
            //serialPort = new SerialPort(cmbPort.SelectedValue.ToString(), int.Parse(cmbBaudRate.SelectedItem.ToString()));
            serialPort = new SerialPort(cmbPort.SelectedValue.ToString(), _baudrate);
            serialPort.DataReceived += SerialPort_DataReceived;
            //serialPort.WriteTimeout = _writeTimeOutMs;
            serialPort.Open();
            logger.LogInfo("Port opened OK");
        }

        private void UnloadSerialPort()
        {
            if(serialPort != null)
            {
                serialPort.DataReceived -= SerialPort_DataReceived;
                CloseSerialPort();
                serialPort.Dispose();
            }
            logger.LogInfo("Port Unloaded");
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (serialPort == null)
            {
                MessageBox.Show("Serial port is not open. Please open the serial port first.");
                logger.LogError("Serial port is not open.");
                return;
            }
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFolderPath = folderBrowserDialog.SelectedPath;
                    lblFolderPath.Text = selectedFolderPath;
                    logger.LogInfo($"Selected Dir: {selectedFolderPath}");
                }
                else
                {
                    logger.LogWarn("No Dir selected.");
                }
            }
        }
        private bool StartUplaodJsonFiles()
        {
            logger.LogInfo("Starting Upload files.");
            if (!Directory.Exists(selectedFolderPath))
            {
                MessageBox.Show($"Select a valid folder to Upload files from.");
                logger.LogError("Dir not selected. Returning.");
                return false;
            }            
            //try
            //{
            //    UPDATE_DELAY_MS = int.Parse(txtDirDelay.Text);                
            //}
            //catch (Exception)
            //{
            //    UPDATE_DELAY_MS = 1000;
            //}
            logger.LogInfo($"Resend Interval Set to {_resendIntervalMs}ms");
            Console.WriteLine($"{_resendIntervalMs}");
            timer.Interval = _resendIntervalMs;
            timer.Elapsed += (sender, e) =>
            {
                if (!isSending) SendFiles();
            };
            logger.LogInfo("Restarting Timer.");
            timer.Start();
            return true;
        }

        /*private void SetupFileWatcher(string path)
        {
            if (!FileWatcherEnabled) return;
            fileWatcher.Path = path;
            fileWatcher.Filter = "*.json";
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Changed += OnChanged;
            fileWatcher.EnableRaisingEvents = true;
        }*/

        /*private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!FileWatcherEnabled) return;
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"File changed: {e.FullPath}");
            System.Threading.Thread.Sleep(100);
            if (File.Exists(e.FullPath))
                await SendFileAsync(e.FullPath);
        }*/
        /*public byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }*/
        private string ExtractValue(string json, string key)
        {
            string searchPattern = $"\"{key}\":\"";

            // Find the starting index of the key
            int startIndex = json.IndexOf(searchPattern);

            string response;
            if (startIndex == -1)
            {
                response = $"Key '{key}' not found in JSON string.";
                //Invoke(new Action(() => DisplayResponse(response)));
                Console.WriteLine(response);
                return string.Empty;
            }

            // Move the start index to the beginning of the value
            startIndex += searchPattern.Length;

            // Find the end of the value
            int endIndex = json.IndexOf("\"", startIndex);

            if (endIndex == -1)
            {
                response = $"Value for key '{key}' not properly terminated.";
                //Invoke(new Action(() => DisplayResponse(response)));
                Console.WriteLine(response);
                return string.Empty;
            }
            string value = json.Substring(startIndex, endIndex - startIndex);
            //Invoke(new Action(() => DisplayResponse($"Value for key '{key}' is '{value}'.")));
            Console.WriteLine($"Value for key '{key}' is '{value}'.");
            return value;
        }
        /*private async Task SendFileAsync(string file)
        {
            try
            {                
                string jsonData = await Task.Run(() => File.ReadAllText(file));
                string idValue = ExtractValue(jsonData, "ID");
                int id = int.Parse(idValue);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(idValue + jsonData);
                serialPort.Write(bytesToSend, 0, bytesToSend.Length);
                info = $"Sent: {file}" + Environment.NewLine + $"Data: {idValue + jsonData}";
                //await Task.Delay(SEND_DELAY_MS);
            }
            catch (Exception ex)
            {
                info = $"Failed to send {file}: {ex.Message}";
            }
            Invoke(new Action(() => DisplayInfo(info)));
        }*/

        /*private async Task SendFilesAsync()
        {
            List<string> jsonFiles = Directory.GetFiles(selectedFolderPath, "*.json").ToList();
            foreach (string file in jsonFiles)
            {                
                try
                {
                    string jsonData = await Task.Run(() => File.ReadAllText(file));
                    string idValue = ExtractValue(jsonData, "ID");
                    byte[] bytesToSend = Encoding.UTF8.GetBytes(idValue + jsonData);
                    serialPort.Write(bytesToSend, 0, bytesToSend.Length);
                    info = $"Sent: {file}";
                    await Task.Delay(SEND_DELAY_MS);
                }
                catch (Exception ex)
                {
                    info = $"Failed to send: {file}: {ex.Message}";
                }
                Invoke(new Action(() => DisplayInfo(info)));
            }
        }*/
        private void SendFiles()
        {
            isSending = true;
            logger.LogInfo("Sending files.");
            List<string> jsonFiles = Directory.GetFiles(selectedFolderPath, "*.json").ToList();
            foreach (string file in jsonFiles)
            {
                logger.LogInfo($"Processing file: {Path.GetFileName(file)}");
                try
                {
                    string jsonData = File.ReadAllText(file);
                    string idValue = ExtractValue(jsonData, "ID");
                    byte[] bytesToSend = Encoding.UTF8.GetBytes(idValue + jsonData);
                    logger.LogInfo($"Sending data to device ID: {idValue}");
                    serialPort.Write(bytesToSend, 0, bytesToSend.Length);
                    info = $"Sent: {file}";
                    logger.LogInfo($"Data sent. Delaying for {_fileDelayMs}ms");
                    //Task.Delay(SEND_DELAY_MS).Wait();
                    Thread.Sleep(_fileDelayMs);
                }
                catch (Exception ex)
                {
                    info = $"Failed to send: {file}: {ex.Message}";
                    logger.LogError(info);
                }
                Invoke(new Action(() => DisplayInfo(info)));
            }
            isSending = false;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            logger.LogInfo("Receiving data from device...");
            string response;
            try
            {
                response = StringToHexString(serialPort.ReadExisting());
                logger.LogInfo($"Received: {response}");
            }
            catch (Exception ex)
            {
                response = $"Error reading from serial port: {ex.Message}";
                logger.LogError(response);
            }
            Invoke(new Action(() => DisplayResponse(response)));
        }
        public static string StringToHexString(string input)
        {
            //byte[] bytes = Encoding.Default.GetBytes(str);
            //string hexString = BitConverter.ToString(bytes);
            //hexString = hexString.Replace("-", "");
            string hexString = "";
            char[] values = input.ToCharArray();
            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                hexString += $"{value:X} ";
            }
            return hexString.Trim();
        }

        private void DisplayInfo(string info)
        {
            
            txtInfo.AppendText(info + Environment.NewLine);
        }
        private void DisplayResponse(string response)
        {
            logger.LogInfo("Response: " + response);
            //txtResponse.AppendText(response + Environment.NewLine);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.LogInfo("Application Closing");
            UnloadSerialPort();
        }

        private void CloseSerialPort()
        {
            try
            {
                if (serialPort.IsOpen) serialPort.Close();
                logger.LogInfo("Port closed");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error reading from serial port: {ex.Message}");
            }
        }

        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            if (btnOpenClose.Text == "&Open")
            {
                logger.LogInfo("Opening Port");
                InitializeSerialPort();
                btnOpenClose.Text = "Cl&ose";
            }
            else
            {
                logger.LogInfo("Closing Port");
                UnloadSerialPort();
                btnOpenClose.Text = "&Open";
            }
        }

        //private void btnOpenUpdater_Click(object sender, EventArgs e)
        //{
        //    logger.LogInfo("Opening updater.");
        //    JsonUpdater jsonUpdater = new JsonUpdater(selectedFolderPath);
        //    jsonUpdater.Show();
        //}

        private void btnTest_Click(object sender, EventArgs e)
        {
            string idValue = "04";
            
            //int id = int.Parse(idValue);
            //Console.WriteLine($"ID: {idValue}, {id}");
            byte[] bytesToSend = Encoding.UTF8.GetBytes(idValue);

            //byte[] bytesToSend = Encoding.UTF8.GetBytes(data);
            //byte[] bytesToSend = new byte[1];
            //bytesToSend[0] = (byte)HexstrToInt(data);

            serialPort.Write(bytesToSend, 0, bytesToSend.Length);
        }
        public static int HexstrToInt(string hex)
        {
            // Convert the number expressed in base-16 to an integer.
            return Convert.ToInt32(hex, 16);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //SEND_DELAY_MS = int.Parse(txtFileDelay.Text);            
            logger.LogInfo($"Starting upload with file delay {_fileDelayMs}ms");
            if (StartUplaodJsonFiles())
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            logger.LogInfo("Stopping Upload");
            timer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            OptionsForm optionsForm = new OptionsForm(_fileDelayMs, _resendIntervalMs, _baudrate);
            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                _fileDelayMs = optionsForm.FileDelayMs;
                _resendIntervalMs = optionsForm.ResendIntervalMs;
                //_writeTimeOutMs = optionsForm.WriteTimeOutMs;
                _baudrate = optionsForm.BaudRate;
                //Properties.Settings.Default.WriteTimeout = _writeTimeOutMs;
                Properties.Settings.Default.FileSendDelayMs = _fileDelayMs;
                Properties.Settings.Default.ResendIntervalMs = _resendIntervalMs;
                Properties.Settings.Default.BaudRate = _baudrate;
                Properties.Settings.Default.Save();
                logger.LogInfo($"Options set: FileDelayMs: {_fileDelayMs}, ResendIntervalMs: {_resendIntervalMs}, BaudRate: {_baudrate}");
            }
        }
    }
}
