using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Recipes
{
    public partial class Form1 : Form
    {
        string PaternPopular = "target=\"_blank\">(.*?)</a>";
        string PaternPopularImage = "<img src='(.*?)alt=\"{New[i]}\"";
        string url = $"https://1000.menu/";
        Dictionary<string, Image> Popular = new Dictionary<string, Image>();
        Dictionary<string, string> PopularLink = new Dictionary<string, string>();
        Dictionary<string, string> Categories = new Dictionary<string, string>();
        int ImageNum = 0;
        List<PictureBox> AllImages = new List<PictureBox>();
        WebClient webClient = new WebClient();
        string result;

        public Form1()
        {
            InitializeComponent();
            result = webClient.DownloadString(url);
            this.Controls.Add(label1);
            this.Controls.Add(progressBar1);
            progressBar1.Location = new Point(350, 200);
            label1.Location = new Point(370, 200);
        }


        private void FullList()
        {
            int x = 13, y = 50;
            int ytmp = 0;
            tabControl1.TabPages[0].Name = "Main";
            tabControl1.TabPages[0].Text = "Главная";
            tabControl1.TabPages[0].Width = 542;
            tabControl1.TabPages[0].Height = 597;
            tabControl1.TabPages[0].AutoScroll = true;
            //tabControl1.TabPages[0].BackColor = Color.FromArgb(255, 218, 185);
            TextBox t = new TextBox();
            t.Multiline = true;
            t.Size = new Size(234, 23);
            t.Location = new Point(137, 11);
            t.Name = "rtb";
            tabControl1.TabPages[0].Controls.Add(t);
            Button b = new Button();
            b.Text = "Найти";
            b.Location = new Point(373, 11);
            b.Click += ButtonSearchClickAsync;
            tabControl1.TabPages[0].Controls.Add(b);
            Label label = new Label();
            label.Text = "Поиск категории";
            label.Location = new Point(30, 14);
            label.AutoSize = true;
            tabControl1.TabPages[0].Controls.Add(label);
            for (int i = 0; i < Popular.Count; i++)
            {
                if (i == Popular.Count / 2)
                {
                    x += 203 + 50;
                    ytmp = y;
                    y = 50;
                }
                var item = Popular.ElementAt(i);
                var itemKey = item.Key;
                var itemValue = item.Value;
                tabControl1.TabPages[0].Controls.Add(PopularPictureBox(x, ref y, itemValue));
                tabControl1.TabPages[0].Controls.Add(PopularLinkLabel(x, ref y, itemKey));
            }
            if (ytmp < y)
                tabControl1.TabPages[0].Controls.Add(SetEndPage(x, y));
            else
                tabControl1.TabPages[0].Controls.Add(SetEndPage(x, ytmp));
        }
        void SetListOnPage(Dictionary<string, Image> D, string Name, TabPage tabPage, int x, ref int y)
        {

            tabPage.Name = Name;
            tabPage.Text = Name;
            tabPage.Width = 542;
            tabPage.Height = 597;
            tabPage.AutoScroll = true;
            AddContent(D, x, ref y, tabPage);

        }

        void AddContent(Dictionary<string, Image> D, int x, ref int y, TabPage tabPage)
        {
            int ytmp = 0;
            int n = y;
            for (int i = 0; i < D.Count; i++)
            {
                if (i == D.Count / 2)
                {
                    x += 203 + 50;
                    ytmp = y;
                    y = n;
                }
                var item = D.ElementAt(i);
                var itemKey = item.Key;
                var itemValue = item.Value;
                tabPage.Controls.Add(PopularPictureBox(x, ref y, itemValue));
                tabPage.Controls.Add(PopularLinkLabel(x, ref y, itemKey));
            }
            if (ytmp > y)
                y = ytmp;
        }
        void SetCategories()
        {
            string Pattern = "<a href=\"/catalog(.*?)\">(.*?)</a>";

            Regex r = new Regex(Pattern);
            MatchCollection m = r.Matches(result);
            foreach (Match x in m)
            {
                if (!x.Groups[2].Value.Contains('<'))
                    Categories.Add(x.Groups[2].Value, "https://1000.menu/catalog" + x.Groups[1].Value);
            }
            SetCategoriesLabelAsync();
        }
        async Task SearchTabAsync(List<string> s)
        {
            int x = 8, y = 23;

            TabPage tabPage = new TabPage();
            tabPage.Name = "Result search";
            tabPage.Text = "Результаты поиска";
            tabPage.Width = 542;
            tabPage.Height = 597;
            tabPage.AutoScroll = true;
            tabPage.MouseClick += tabPage1_MouseClick;
            //tabPage.BackColor = Color.FromArgb(255, 218, 185);
            tabPage.Font = new Font("Times new Roman", 11);
            Label label = new Label();
            label.Text = "Результаты поиска:";
            label.AutoSize = true;
            label.Location = new Point(x, y);
            label.Font = new Font("Source Sans Pro", 12);
            y += 30;
            tabPage.Controls.Add(label);

            foreach (var item in s)
            {
                LinkLabel l = new LinkLabel();
                l.LinkBehavior = LinkBehavior.NeverUnderline;
                l.LinkColor = Color.Black;
                l.Font = new Font("Source Sans Pro", 12);
                l.Location = new Point(x, y);
                y += 30;
                l.Text = item;
                l.LinkClicked += LinkLabelCategoriesClickAsync;
                l.MaximumSize = new Size(210, 0);
                l.Visible = true;
                l.AutoSize = true;
                tabPage.Controls.Add(l);

            }
            tabControl1.Controls.Add(tabPage);
            tabControl1.SelectTab(tabPage);
        }
        async Task SetCategoriesLabelAsync()
        {
            int x = 8, y = 23;
            foreach (var d in Categories)
            {
                LinkLabel l = new LinkLabel();
                l.LinkBehavior = LinkBehavior.NeverUnderline;
                l.LinkColor = Color.Black;
                l.Font = new Font("Source Sans Pro", 12);
                l.Location = new Point(x, y);
                l.Text = d.Key;
                l.LinkClicked += LinkLabelCategoriesClickAsync;
                l.MaximumSize = new Size(210, 0);
                l.Visible = true;
                l.AutoSize = true;
                if (l.Size.Width > 210)
                    y = y + 35;
                else
                    y = y + 20;
                if (y >= this.Height + 100)
                    break;
                groupBox1.Controls.Add(l);
            }

            //8; 23

        }
        PictureBox SetEndPage(int x, int y)
        {
            PictureBox p2 = new PictureBox();
            p2.Location = new Point(x, y);
            p2.Size = new Size(326, 170);//142
            p2.MouseClick += tabPage1_MouseClick;
            return p2;
        }
        PictureBox PopularPictureBox(int x, ref int y, Image d)
        {
            PictureBox p = new PictureBox();
            Image imageCopy;
            using (var imageSource = d)
                imageCopy = new Bitmap(imageSource);
            p.BackgroundImage = imageCopy;

            p.Location = new Point(x, y);
            p.Size = new Size(203, 101);
            p.BackgroundImageLayout = ImageLayout.Stretch;
            y += p.Height + 10;
            p.MouseClick += tabPage1_MouseClick;
            AllImages.Add(p);

            return p;
        }
        LinkLabel PopularLinkLabel(int x, ref int y, string d)
        {
            LinkLabel l = new LinkLabel();
            l.Text = d;
            l.Location = new Point(x, y);
            l.AutoSize = true;
            l.MaximumSize = new Size(181, 0);
            l.LinkBehavior = LinkBehavior.NeverUnderline;
            l.ActiveLinkColor = Color.Blue;
            l.LinkColor = Color.Black;
            l.LinkClicked += linkLabel1_LinkClicked;
            int n = l.Text.Length / 28;
            y += n * 15;
            y += 35;
            return l;
        }
        private void PopularSet()
        {
            List<string> New = new List<string>();
            List<string> ImagesNew = new List<string>();
            List<string> Images = new List<string>();
            Regex r = new Regex(PaternPopular);
            //target="_blank"
            //Match match = Regex.Match(result, "target=\"_blank\">(.*?)</a>");
            MatchCollection m = r.Matches(result); foreach (Match x in m)
            {
                New.Add(x.Groups[1].Value);
                ImagesNew.Add($"<img src='(.*?)' alt=\"{x.Groups[1].Value}\"");
            }
            //\"Пирог с малиной и творогом\"
            string g = "<a class=\"h5\" href=\"(.*?)\" target=\"_blank\">";
            for (int i = 0; i < New.Count; i++)
            {
                Match match = Regex.Match(result, g + New[i]);
                if (match.Success)
                {
                    PopularLink.Add(New[i], @"https://1000.menu/" + match.Groups[1].Value);
                }
            }
            foreach (var i in ImagesNew)
            {
                Match match = Regex.Match(result, i);
                if (match.Success)
                {
                    Images.Add(match.Groups[1].Value);
                }
            }
            for (int j = 0; j < Images.Count; j++)
            {
                WebClient wc = new WebClient();
                Images[j] = "http:" + Images[j];
                wc.DownloadFile(Images[j], j.ToString() + ".jpg");
                Popular.Add(New[j], Image.FromFile(j.ToString() + ".jpg"));
            }
            ImageNum = ImagesNew.Count + 1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopularSet();
            FullList();
            SetCategories();
        }
        async Task CategoriesClickAsync(string lb)
        {
            bool c = false;
            for (int i = 0; i < tabControl1.Controls.Count; i++)
            {
                if (tabControl1.Controls[i].Name == lb)
                {
                    c = true;
                    break;
                }
            }
            if (!c)
            {
                TabPage tabPage = new TabPage();
                Name = lb;
                string Link;
                Link = Categories.Where(q => q.Key == Name).Select(q => q.Value).FirstOrDefault();
                string resultTmp = webClient.DownloadString(Link);
                tabPage.Name = lb;
                tabPage.Text = lb;
                tabPage.Width = 542;
                tabPage.Height = 597;
                tabPage.AutoScroll = true;
                tabPage.MouseClick += tabPage1_MouseClick;
                //tabPage.BackColor = Color.FromArgb(255, 218, 185);
                tabPage.Font = new Font("Times new Roman", 11);
                int num = 0;
                ImageNum++;
                //while (true)
                //{
                int x = 13, y = 50;
                Dictionary<string, Image> NameImage = new Dictionary<string, Image>();
                NameImage = await AddContent(NameImage, resultTmp);
                SetListOnPage(NameImage, lb, tabPage, x, ref y);
                Button b = new Button();
                b.Location = new Point(170, y - 5);
                y = y + 10;
                b.Text = "Загрузить еще";
                b.Click += ButtonClickAsync;
                b.Size = new Size(200, 23);
                b.Visible = true;
                tabPage.Controls.Add(b);
                tabPage.Controls.Add(SetEndPage(x, y));
                tabControl1.TabPages.Add(tabPage);
                tabControl1.SelectTab(tabPage);
            }
            else
            {
                MessageBox.Show("Такая вкладка уже открыта", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private async void LinkLabelCategoriesClickAsync(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel l = (LinkLabel)sender;
            await CategoriesClickAsync(l.Text);
        }
        async Task<Dictionary<string, Image>> AddContent(Dictionary<string, Image> NameImage, string resultTmp)
        {
            GetImageForPage("<img src='(.*?)' alt=\"(.*?)\"", Popular, resultTmp);
            GetImageForPage("<img src='(.*?)' alt=\"(.*?)\"", NameImage, resultTmp);
            //PopularLink,Popular
            GetImageForPage("data-original='(.*?)' alt=\"(.*?)\"", NameImage, resultTmp);
            GetImageForPage("data-original='(.*?)' alt=\"(.*?)\"", Popular, resultTmp);

            Regex r = new Regex("<a class=\"h5\" href=\"(.*?)\" target=\"_blank\">(.*?)</a>");
            MatchCollection m = r.Matches(resultTmp);
            foreach (Match x in m)
            {
                var b = PopularLink.Where(q => q.Key == x.Groups[2].Value).Select(x => x.Key).FirstOrDefault();
                if (b == null)
                    PopularLink.Add(x.Groups[2].Value, "https://1000.menu/" + x.Groups[1].Value);

            }
            tabControl1.Visible = true;
            return NameImage;
        }
        async void ButtonClickAsync(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            TabPage page = tabControl1.SelectedTab;
            int x = 13;
            int y = b.Location.Y;
            string Link = string.Empty;
            foreach (var c in Categories)
            {
                if (c.Key == page.Name)
                {
                    Link = c.Value;
                    break;
                }
            }
            int num;
            try
            {
                //num = Convert.ToInt32(Link[Link.Length - 1]);

                num = Convert.ToInt32(Link.Substring(Link.LastIndexOf('/') + 1, 1));
                num++;
                Link = Link.Remove(Link.LastIndexOf('/'), 2);
            }
            catch
            {
                num = 2;
            }
            //string Linktmp = Link;

            Categories[page.Name] = Link += $"/{num}";
            //Link += $"/{num}";
            string resultTmp = webClient.DownloadString(Link);
            if (!resultTmp.Contains("Указана неверная страница"))
            {
                page.Controls.Remove(page.Controls[page.Controls.Count - 1]);
                Dictionary<string, Image> NameImage = new Dictionary<string, Image>();
                NameImage = await AddContent(NameImage, resultTmp);
                y += 150;
                AddContent(NameImage, x, ref y, page);
                b.Location = new Point(170, y);
                page.Controls.Add(SetEndPage(x, y));
            }
            else
            {
                b.Visible = false;
                b.Enabled = false;
                Link = Link.Remove(Link.LastIndexOf('/'), 2);
            }

        }
        private void GetImageForPage(string Pattern, Dictionary<string, Image> D, string result)
        {
            Regex r = new Regex(Pattern);
            MatchCollection m = r.Matches(result);
            TabPage page = tabControl1.SelectedTab;
            tabControl1.Visible = false;
            label1.Visible = true;
            progressBar1.Maximum = m.Count;
            progressBar1.Visible = true;
            foreach (Match x in m)
            {
                try
                {
                    var b = D.Where(q => q.Key == x.Groups[2].Value).Select(x => x.Key).FirstOrDefault();
                    if (b == null)
                    {
                        webClient.DownloadFile(@"https:" + x.Groups[1].Value, ImageNum + ".jpg");
                        D.Add(x.Groups[2].Value, Image.FromFile(ImageNum.ToString() + ".jpg"));
                        ImageNum++;

                    }

                }
                catch
                {
                    break;
                    progressBar1.Value = m.Count;
                }
                progressBar1.Value++;
            }
            progressBar1.Value = 0;
            progressBar1.Visible = false;
            label1.Visible = false;

        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            LinkLabel l = (LinkLabel)sender;
            TMP(l);

        }
        void TMP(LinkLabel l)
        {
            int x = 92, y = 13;
            TabPage tabPage = new TabPage();
            string Link = string.Empty;
            Link = PopularLink.Where(q => q.Key == l.Text).Select(q => q.Value).FirstOrDefault();
            string resultTmp = webClient.DownloadString(Link);
            tabPage.Name = l.Text;
            tabPage.Text = l.Text;
            tabPage.Width = 542;
            tabPage.Height = 597;
            tabPage.AutoScroll = true;
            //tabPage.BackColor = Color.FromArgb(255, 218, 185);
            tabPage.Font = new Font("Times new Roman", 11);
            //542; 597
            tabPage.Controls.Add(SetTabImage(l.Text, "<img itemprop=\"image\" src='(.*?)' alt=\"" + l.Text + "\"", ref x, ref y, resultTmp));
            tabPage.Controls.Add(SetTabLabel1(l.Text, x, ref y));
            List<string> Ingredients = GetString("class='name'>(.*?)</a>", resultTmp);
            Ingredients.RemoveAt(Ingredients.Count - 1);
            tabPage.Controls.Add(SetTabLabel2(Ingredients, x, ref y, "Ингредиенты:"));
            x = 13;
            y = y + 20;
            tabPage.MouseClick += tabPage1_MouseClick;
            SetInstruction(x, y, resultTmp, tabPage);
            tabControl1.TabPages.Add(tabPage);
            tabControl1.SelectTab(tabPage);
        }
        void SetInstruction(int x, int y, string Link, TabPage t)
        {
            int k = 1;
            tabControl1.Visible = false;
            label1.Visible = true;
            while (true)
            {
                y = y + 20;
                string pattern = $"<h4> Шаг {k}:</h4><a href=\"(.*?)\" title=\"Шаг {k}.";
                Match match = Regex.Match(Link, pattern);
                if (match.Success)
                {
                    string photo = "http:" + match.Groups[1].Value;
                    //Label l1 = new Label();
                    //l1.Text = $"Шаг {k}.";
                    //l1.Location = new Point(x, y);
                    //t.Controls.Add(l1);
                    y = y + 20;
                    if (photo != string.Empty)
                    {
                        y = y + 8;
                        webClient.DownloadFile(photo, ImageNum.ToString() + ".jpg");
                        PictureBox p1 = new PictureBox();
                        p1.Location = new Point(x, y);
                        p1.Size = new Size(326, 142);
                        p1.MouseClick += tabPage1_MouseClick;
                        y = y + p1.Size.Height;
                        p1.BackgroundImageLayout = ImageLayout.Zoom;
                        Image imageCopy;
                        using (var imageSource = Image.FromFile(ImageNum.ToString() + ".jpg"))
                            imageCopy = new Bitmap(imageSource);
                        p1.BackgroundImage = imageCopy;
                        ImageNum++;
                        AllImages.Add(p1);
                        t.Controls.Add(p1);
                    }
                    pattern = $"title=\"Шаг {k}.(.*?)\" class=";
                    Match match1 = Regex.Match(Link, pattern);
                    Link = Link.Replace("\n", "").Replace("\r", "");
                    if (match1.Success)
                    {
                        Label l2 = new Label();
                        l2.Text = match1.Groups[1].Value;
                        l2.Location = new Point(x, y);
                        l2.MaximumSize = new Size(490, 0);
                        l2.AutoSize = true;
                        //int n = l2.Text.Length / 67;
                        //y = n * 10;
                        y = y + l2.PreferredSize.Height;
                        t.Controls.Add(l2);
                    }
                    k++;

                }
                else
                {
                    tabControl1.Visible = true;
                    label1.Visible = false;
                    break;
                }
                   

            }
            t.Controls.Add(SetEndPage(x, y));
        }
        List<string> GetString(string Pattern, string Link)
        {
            List<string> strings = new List<string>();
            Regex r = new Regex(Pattern);
            MatchCollection m = r.Matches(Link);
            foreach (Match x in m)
            {
                strings.Add(x.Groups[1].Value);
            }
            return strings;
        }
        Label SetTabLabel2(List<string> m, int x, ref int y, string c)
        {
            Label l = new Label();
            l.Text = c;
            foreach (var d in m)
            {
                l.Text += " " + d.ToLower() + ",";
            }
            l.Text += ".";
            l.AutoSize = false;
            l.Size = new Size(490, 39);
            l.Location = new Point(13, 224);
            y = y + l.Size.Height;
            return l;
        }
        Label SetTabLabel1(string text, int x, ref int y)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.MaximumSize = new Size(490, 0);

            l.Location = new Point(x, y);
            y = y + l.Size.Height;
            return l;
        }
        PictureBox SetTabImage(string text, string Pattern, ref int x, ref int y, string Link)
        {
            PictureBox picG = new PictureBox();
            picG.Location = new Point(x, y);
            //175, 142
            picG.Size = new Size(326, 142);
            picG.MouseClick += tabPage1_MouseClick;
            y = y + picG.Size.Height;
            Regex r = new Regex(Pattern);
            MatchCollection m = r.Matches(Link);
            picG.BackgroundImageLayout = ImageLayout.Zoom;
            foreach (Match x1 in m)
            {
                webClient.DownloadFile(@"https:" + x1.Groups[1].Value, ImageNum + ".jpg");
                Image imageCopy;
                using (var imageSource = Image.FromFile(ImageNum + ".jpg"))
                    imageCopy = new Bitmap(imageSource);
                picG.BackgroundImage = imageCopy;
                ImageNum++;
                AllImages.Add(picG);
                return picG;
            }
            return picG;
        }
        async void ButtonSearchClickAsync(object sender, EventArgs e)
        {
            TextBox selectedRtb = (TextBox)tabControl1.SelectedTab.Controls["rtb"];
            string x = selectedRtb.Text;
            bool d = false;
            string g = string.Empty;
            List<string> s = new List<string>();
            foreach (var c in Categories)
            {
                g = c.Key.ToLower();
                x = x.ToLower();
                if (g.Contains(x))
                {
                    s.Add(c.Key);
                    d = true;
                }
            }
            if (d)
            {
                selectedRtb.Text = string.Empty;
                SearchTabAsync(s);
            }
            else
            {
                MessageBox.Show("Не найдено", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        void DeletePic()
        {
            Popular.Clear();
            PopularLink.Clear();
            Categories.Clear();
            webClient.Dispose();
            tabControl1.Dispose();
            for (int i = 0; i < AllImages.Count; i++)
            {
                if (AllImages[i] != null)
                {
                    AllImages[i].BackgroundImage.Dispose();
                    AllImages[i].BackgroundImage = null;
                }


            }

            for (int i = 0; i < ImageNum - 1; i++)
            {
                try
                {
                    var image = Image.FromFile($"{i}.jpg");
                    image.Dispose(); // this removes all resources
                    File.Delete($"{i}.jpg");
                }
                catch (System.IO.FileNotFoundException ex)
                {

                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DeletePic();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DeletePic();
        }


        private void tabPage1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show(this.tabControl1, e.Location);
            }

        }

        private void закрытьВкладкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            else
                MessageBox.Show("Нельзя закрыть главную вкладку", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}