using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Security.Policy;

namespace Keyword_Scraper_By_MasterC
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        static bool running = false;
        static List<string> dupe = new List<string>();
        delegate void Delegate(string line);
        List<string> prefixes = new List<string> { " ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "y", "x", "y", "z", "how", "which", "why", "where", "who", "when", "are", "what" };
        List<string> suffixes = new List<string> { " ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "y", "x", "y", "z", "like", "for", "without", "with", "versus", "vs", "to", "near", "except", "has" };
        bool prefixshow = false;
        bool suffixshow = false;
        private const int cGrip = 16;
        private const int cCaption = 32;
        public Form1()
        {
            InitializeComponent();

            AdjustAllColumnWidths();

            this.FormBorderStyle = FormBorderStyle.None;
            this.MouseDown += new MouseEventHandler(MouseDownEvent);
            this.MouseMove += new MouseEventHandler(MouseMoveEvent);
            this.MouseUp += new MouseEventHandler(MouseUpEvent);

            foreach (Control control in this.Controls)
            {
                control.MouseDown += new MouseEventHandler(MouseDownEvent);
                control.MouseMove += new MouseEventHandler(MouseMoveEvent);
                control.MouseUp += new MouseEventHandler(MouseUpEvent);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rtbKeywords.AllowDrop = true;
            rtbKeywords.DragEnter += new DragEventHandler(rtbKeywords_DragEnter);
            rtbKeywords.DragDrop += new DragEventHandler(rtbKeywords_DragDrop);
        }

        private void AdjustColumnWidths(ListView listView)
        {
            int totalWidth = 0;
            foreach (ColumnHeader column in listView.Columns)
            {
                totalWidth += column.Width;
            }

            int remainingWidth = listView.ClientSize.Width - totalWidth;
            if (remainingWidth > 0)
            {
                listView.Columns[listView.Columns.Count - 1].Width += remainingWidth;
            }
        }

        private void AdjustAllColumnWidths()
        {
            AdjustColumnWidths(KeywordsListView);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void btnImportKeywords_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt";
                ofd.Title = "Keywords File";

                if (ofd.ShowDialog()== DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    string[] keywords = System.IO.File.ReadAllLines(filePath);
                    rtbKeywords.Lines = keywords;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (rtbKeywords.Lines.Length > 0 || KeywordsListView.Items.Count > 0)
            {
                var result = MessageBox.Show("Are you sure you want to clear everything?", "Confirmation",MessageBoxButtons.YesNo , MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    rtbKeywords.Clear();
                    KeywordsListView.Items.Clear();
                }
            }
        }

        private void rtbKeywords_TextChanged(object sender, EventArgs e)
        {
            int lineCountKeywords = rtbKeywords.Lines.Length;
            lblKeywords.Text = lineCountKeywords.ToString();
        }

        private void UpdateLabelPosition(Label label, int baseX)
        {
            using (Graphics g = label.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(label.Text, label.Font);
                int newX = baseX - (int)textSize.Width;

                if (newX < 6)
                {
                    newX = 6;
                }

                label.Location = new Point(newX, label.Location.Y);
            }
        }

        private void lblKeywords_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblKeywords, 183);
        }

        private void lblResutls_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblResutls, 183);
        }

        private void lblThreads_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblThreads, 183);
        }

        private void lblTimeElapsed_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblTimeElapsed, 183);
        }

        private void lblCPM_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblCPM, 183);
        }

        private void BingSearch(string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int bingCount = 0;
            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }

            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");
                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if (running == true)
                        {
                            semaphore.Wait();
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < retryCount; i++)
                                    {
                                        try
                                        {
                                            HttpResponseMessage response = client.GetAsync("https://api.bing.com/osjson.aspx?query=" + keyword).Result;
                                            JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);

                                            foreach (string output in (JArray)jsonArray[1])
                                            {
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "Bing");
                                                    bingCount++;
                                                    lblBingCounts.Invoke((MethodInvoker)(() => lblBingCounts.Text = bingCount.ToString()));
                                                }
                                            }
                                            break;
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }

                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private int GetSearchResultCount(string keyword, string searchEngineUrl, string resultMarket)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync($"{searchEngineUrl}{Uri.EscapeDataString(keyword)}").Result;
                    string html = response.Content.ReadAsStringAsync().Result;

                    int startIndex = html.IndexOf(searchEngineUrl);
                    if (startIndex != -1)
                    {
                        startIndex = html.IndexOf(">", startIndex) + 1;
                        int endIndex = html.IndexOf("<", startIndex);
                        string resultText = html.Substring(startIndex, endIndex - startIndex).Replace(",", "");

                        if(int.TryParse(resultText.Split(' ')[0], out int resultCount))
                        {
                            return resultCount;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error fetching result count for '{keyword}' from '{searchEngineUrl}': {ex.Message}");
            }
            return 0;
        }

        private void GoogleSearch(string furl, string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int googleCount = 0;

            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }

            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");
                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if (running == true)
                        {
                            semaphore.Wait();
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i =0; i < retryCount; i++)
                                    {
                                        try
                                        {
                                            HttpResponseMessage response = client.GetAsync(furl + keyword).Result;
                                            string jsonData = response.Content.ReadAsStringAsync().Result;
                                            JArray jsonArray = JArray.Parse(jsonData);
                                            JArray suggestionsArray = jsonArray[1].ToObject<JArray>();
                                            string[] result = suggestionsArray.Select(suggestion => suggestion.ToString()).ToArray();
                                            foreach (string output in result)
                                            {
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "Google");
                                                    googleCount++;
                                                    lblGoogleCounts.Invoke((MethodInvoker)(() => lblGoogleCounts.Text = googleCount.ToString()));
                                                }
                                            }
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }
                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void Ebay(string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int ebayCount = 0;

            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }
            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");

                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if(running == true)
                        {
                            semaphore.Wait();
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < retryCount; i++)
                                    {
                                        try
                                        {
                                            HttpResponseMessage response = client.GetAsync("https://autosug.ebay.com/autosug?kwd=" + keyword).Result;
                                            string json = response.Content.ReadAsStringAsync().Result;
                                            int startIndex = json.IndexOf("(") + 1;
                                            int endIndex = json.LastIndexOf(")");
                                            string jsonData = json.Substring(startIndex, endIndex - startIndex);
                                            dynamic suggestions = JsonConvert.DeserializeObject<dynamic>(jsonData);
                                            foreach (string output in suggestions.res.sug)
                                            {
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "Ebay");
                                                    ebayCount++;
                                                    lblEbayCounts.Invoke((MethodInvoker)(() => lblEbayCounts.Text = ebayCount.ToString()));
                                                }
                                            }
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }
                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void YouTubeSearch(string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int youtubeCount = 0;

            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }

            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");
                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if (running == true)
                        {
                            semaphore.Wait();
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < retryCount; i++)
                                    {
                                        try
                                        {
                                            HttpResponseMessage response = client.GetAsync("https://suggestqueries.google.com/complete/search?client=youtube&ds=yt&q=" + keyword).Result;
                                            string responseString = response.Content.ReadAsStringAsync().Result;
                                            JArray jsonArray = JArray.Parse(responseString.Substring(19, responseString.Length - 20));
                                            foreach (var suggestion in jsonArray[1])
                                            {
                                                string output = suggestion[0].ToString();
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "YouTube");
                                                    youtubeCount++;
                                                    label26.Invoke((MethodInvoker)(() => label26.Text = youtubeCount.ToString()));
                                                }
                                            }
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }
                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }


        private void Amazon(string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int amazonCount = 0;

            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }

            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");
                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if (running == true)
                        {
                            semaphore.Wait();
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < retryCount; i++)
                                    {
                                        try
                                        {
                                            HttpResponseMessage response = client.GetAsync("https://completion.amazon.com/api/2017/suggestions?alias=aps&plain-mid=1&prefix=" + keyword).Result;
                                            JObject jsonObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                            JArray suggestions = (JArray)jsonObject["suggestions"];
                                            foreach (var suggestion in suggestions)
                                            {
                                                string output = (string)suggestion["value"];
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "Amazon");
                                                    amazonCount++;
                                                    lblAmazonCounts.Invoke((MethodInvoker)(() => lblAmazonCounts.Text = amazonCount.ToString()));
                                                }
                                            }
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }
                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void Yandex(string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int yandexCount = 0;

            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }
            
            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");
                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if (running == true)
                        {
                            semaphore.Wait();
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < retryCount; i++)
                                    {
                                        try
                                        {
                                            HttpResponseMessage response = client.GetAsync("https://yandex.com/suggest/suggest-ya.cgi?n=1000&part=" + keyword).Result;
                                            string jasonData = response.Content.ReadAsStringAsync().Result;
                                            int startIndex = jasonData.IndexOf('[');
                                            int endIndex = jasonData.LastIndexOf(']');
                                            string jasonArrayString = jasonData.Substring(startIndex, endIndex - startIndex - 6);
                                            JArray jsonArray = JArray.Parse(jasonArrayString + "]");
                                            JArray innerList = (JArray)jsonArray[1];
                                            var list = innerList.ToObject<List<string>>();
                                            foreach (string output in list)
                                            {
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "Yandex");
                                                    yandexCount++;
                                                    lblYandexCounts.Invoke((MethodInvoker)(() => lblYandexCounts.Text = yandexCount.ToString()));
                                                }
                                            }
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }
                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void DuckDuckGo(string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int duckCounts = 0;

            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }

            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");
                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if(running == true)
                        {
                            semaphore.Wait();
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < retryCount; i++)
                                    {
                                        try
                                        {
                                            HttpResponseMessage responseMessage = client.GetAsync("https://duckduckgo.com/ac/?q=" + keyword).Result;
                                            List<dynamic> phraseList = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage.Content.ReadAsStringAsync().Result);
                                            foreach (var out_ in phraseList)
                                            {
                                                string output = out_.phrase.ToString();
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "DuckDuckGo");
                                                    duckCounts++;
                                                    lblDuckDuckGoCounts.Invoke((MethodInvoker)(() => lblDuckDuckGoCounts.Text = duckCounts.ToString()));
                                                }
                                            }
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }
                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void Yahoo(string[] kw, int timeout, int retryCount)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            int yahooCounts = 0;

            if (File.Exists("prefixes.txt"))
            {
                prefixes = File.ReadAllLines("prefixes.txt").ToList();
            }
            if (File.Exists("suffixes.txt"))
            {
                suffixes = File.ReadAllLines("suffixes.txt").ToList();
            }

            foreach (string kwrd in kw)
            {
                List<string> kwrds = new List<string> { kwrd };
                foreach (string prefix in prefixes)
                {
                    kwrds.Add(prefix + " " + kwrd);
                }
                foreach (string suffix in suffixes)
                {
                    kwrds.Add(kwrd + " " + suffix);
                }
                kwrds.RemoveAll(s => s == "");
                if (running == true)
                {
                    foreach (string keyword in kwrds)
                    {
                        if (running == true)
                        {
                            semaphore.Wait(); // Wait for an available slot
                            lock (threadLock)
                            {
                                activeThreads++;
                                lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    for (int i = 0; i< retryCount; i++)
                                    {
                                        try
                                        {
                                            //https://search.yahoo.com/sugg/gossip/gossip-us-ura/?output=fxjson&command=
                                            HttpResponseMessage response = client.GetAsync("https://search.yahoo.com/sugg/ff?output=json&command=" + keyword).Result;
                                            dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                                            foreach (var result in jsonData.gossip.results)
                                            {
                                                string output = result.key.ToString();
                                                if (!dupe.Contains(output))
                                                {
                                                    dupe.Add(output);
                                                    SafeWrite(output, "Yahoo");
                                                    yahooCounts++;
                                                    lblYahooCounts.Invoke((MethodInvoker)(() => lblYahooCounts.Text = yahooCounts.ToString()));
                                                }
                                            }
                                        }
                                        catch (HttpRequestException httpEx)
                                        {
                                            Console.WriteLine($"Request Error for keyword '{keyword}': {httpEx.Message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error Processing keyword '{keyword}': {ex.Message}");
                                        }
                                    }
                                }
                                finally
                                {
                                    lock (threadLock)
                                    {
                                        activeThreads--;
                                        lblThreads.Invoke((MethodInvoker)(() => lblThreads.Text = activeThreads.ToString()));
                                    }
                                    semaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void AddToListView(string keyword, string source)
        {
            if (KeywordsListView.InvokeRequired)
            {
                KeywordsListView.Invoke(new Action<string, string>(AddToListView), keyword, source);
            }
            else
            {
                int id = KeywordsListView.Items.Count + 1;
                ListViewItem item = new ListViewItem(id.ToString());
                item.SubItems.Add(keyword);
                item.SubItems.Add(source);
                KeywordsListView.Items.Add(item);
                keywordCount++;
                lblResutls.Text = KeywordsListView.Items.Count.ToString();
                KeywordsListView.EnsureVisible(KeywordsListView.Items.Count - 1);
            }
        }

        private void SafeWrite(string output, string source)
        {
            AddToListView(output, source);
        }

        private Stopwatch stopwatch = new Stopwatch();
        private int keywordCount = 0;
        private int activeThreads = 0;
        private object threadLock = new object();
        private SemaphoreSlim semaphore;
        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (rtbKeywords == null || rtbKeywords.Lines.Length == 0)
            {
                MessageBox.Show("Please import Keywords", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (btnStart.Text == "Stop")
            {
                running = false;
                btnStart.Text = "Start";
                stopwatch.Stop();
            }
            else
            {
                btnStart.Text = "Stop";
                running = true;
                keywordCount = 0;
                stopwatch.Restart();
                string[] lines = rtbKeywords.Lines;
                int timeout = (int)timeoutNumUpDown.Value;
                int maxThreads = (int)nudThreads.Value;
                int retryCount = (int)nudRetry.Value;
                semaphore = new SemaphoreSlim(maxThreads);
                Task.Run(() => UpdateElapsedTime());
                if (ckbBing.Checked) { await Task.Run(() => BingSearch(lines, timeout, retryCount)); }
                if (ckbGoogle.Checked) { await Task.Run(() => GoogleSearch("https://suggestqueries.google.com/complete/search?client=chrome&q=", lines, timeout, retryCount)); }
                if (ckbYahoo.Checked) { await Task.Run(() => Yahoo(lines, timeout, retryCount)); }
                if (ckbEbay.Checked) { await Task.Run(() => Ebay(lines, timeout, retryCount)); }
                if (ckbYoutube.Checked) { await Task.Run(() => YouTubeSearch(lines, timeout, retryCount)); }
                if (ckbYandex.Checked) { await Task.Run(() => Yandex(lines, timeout, retryCount)); }
                if (ckbAmazon.Checked) { await Task.Run(() => Amazon(lines, timeout, retryCount)); }
                if (ckbDuck.Checked) { await Task.Run(() => DuckDuckGo(lines, timeout, retryCount)); }
                running = false;
                btnStart.Text = "Start";
                stopwatch.Stop();
            }
        }

        private void UpdateElapsedTime()
        {
            while (running)
            {
                lblTimeElapsed.Invoke((MethodInvoker)(() => lblTimeElapsed.Text = stopwatch.Elapsed.ToString(@"dd\.hh\:mm\:ss")));
                UpdateCPM();
                Thread.Sleep(1000);
            }
        }

        private void UpdateCPM()
        {
            if (stopwatch.Elapsed.TotalMinutes > 0)
            {
                double cpm = keywordCount / stopwatch.Elapsed.TotalMinutes;
                lblCPM.Invoke((MethodInvoker)(() => lblCPM.Text = $"{cpm:F2}"));
            }
        }

        private void btnPrefixes_Click(object sender, EventArgs e)
        {
            FrmModifiers frmModifiers = new FrmModifiers();
            if(File.Exists("prefixes.txt"))
            {
                frmModifiers.LoadPrefixesFromFile();
            }
            else
            {
                frmModifiers.LoadPrefixes(prefixes);
            }
            frmModifiers.ShowDialog();

        }

        private void btnSuffixes_Click(object sender, EventArgs e)
        {
            FrmModifiers frmModifiers = new FrmModifiers();
            if (File.Exists("suffixes.txt"))
            {
                frmModifiers.LoadSuffixesFromFile();
            }
            else
            {
                frmModifiers.LoadSuffixes(suffixes);
            }
            frmModifiers.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (KeywordsListView.Items.Count == null || KeywordsListView.Items.Count == 0)
            {
                MessageBox.Show($"No keywords found to save: {KeywordsListView.Items.Count}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sfd.Title = "Save Keywords";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    foreach (ListViewItem item in KeywordsListView.Items)
                    {
                        sw.WriteLine(item.SubItems[1].Text);
                    }
                }
            }
        }

        private void lblBingCounts_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblBingCounts, 184);
        }

        private void lblGoogleCounts_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblGoogleCounts, 184);
        }

        private void lblYahooCounts_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblYahooCounts, 184);
        }

        private void lblEbayCounts_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblEbayCounts, 184);
        }

        private void label26_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(label26, 184);
        }

        private void lblYandexCounts_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblYandexCounts, 184);
        }

        private void lblAmazonCounts_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblAmazonCounts, 184);
        }

        private void lblDuckDuckGoCounts_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelPosition(lblDuckDuckGoCounts, 184);
        }

        bool all = false;
        private void ckbAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!all)
            {
                ckbAmazon.Checked = true;
                ckbEbay.Checked = true;
                ckbBing.Checked = true;
                ckbDuck.Checked = true;
                ckbGoogle.Checked = true;
                ckbYahoo.Checked = true;
                ckbYandex.Checked = true;
                ckbYoutube.Checked = true;
                all = true;
            }
            else
            {
                ckbAmazon.Checked = false;
                ckbEbay.Checked = false;
                ckbBing.Checked = false;
                ckbDuck.Checked = false;
                ckbGoogle.Checked = false;
                ckbYahoo.Checked = false;
                ckbYandex.Checked = false;
                ckbYoutube.Checked = false;
                all = false;
            }
        }

        private void lblᴅᴀʀᴋᴄʜɪᴘᴇʀ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/DarkChipers");
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/DarkChipers");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/cheatAi");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/Epionx");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/+BO7K2Pq_3-JkZDY0");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/DarkChipers");
        }

        private void rtbKeywords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (Clipboard.ContainsText())
                {
                    rtbKeywords.SelectedText = Clipboard.GetText();
                }
                e.Handled = true;
            }
        }

        private void rtbKeywords_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void rtbKeywords_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                rtbKeywords.SelectedText = (string)e.Data.GetData(DataFormats.Text);
            }
        }

        private void btnCopyBTC_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("bc1qt5mjltcmk2uga435ah7kfypnfypw4v27p080e8");
            ShowCopiedMessage(lblBtcAddress);
        }

        private void btnCopyLTC_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("ltc1qhsrdyldzhj2z5dh4chsqfr29aexu378zqgd200");
            ShowCopiedMessage(lblLtcAddress);
        }

        private void btnCopyETH_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("0x6e9D949ff16EfFE917776A67B378b57b5AADB508");
            ShowCopiedMessage(lblEthAddress);
        }

        private void btnCopyTRX_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("TC6jmvM8xfNcn5q5QbAc1hTG3izcMifEXr");
            ShowCopiedMessage(lblTrxAddress);
        }

        private void ShowCopiedMessage(Label label)
        {
            label.Text = "Text Copied!";
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000;
            timer.Tick += (s, e) =>
            {
                label.Text = string.Empty;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/Epionx");
        }

        private void btnMinimise_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnShop_Click(object sender, EventArgs e)
        {
            Process.Start("https://darkchiper.sell.app/");
        }

        private void lblShopLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://darkchiper.sell.app/");
        }

        private void lblGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/2Brody");
        }

        private void btnGithub_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/2Brody");
        }
    }
}
