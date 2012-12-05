using HtmlAgilityPack;
using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TuentiDownloader
{
    public partial class Main : Form
    {
        private bool _cancel = false;
        public Main()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            savePath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Tuenti");
            numericUpDown1_ValueChanged(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (savePath.Enabled)
            {
                _cancel = false;
                savePath.Enabled = false;
                button2.Enabled = false;
                button1.Text = "Parar";

                Directory.CreateDirectory(savePath.Text);

                Tuenti.SavePath = savePath.Text;
                var downloader = new HtmlDownloader();
                downloader.ResourcePath = Path.Combine(savePath.Text, "Recursos");


                //Descargar fotos
                if (!_cancel && downloadPhotos.Checked)
                {
                    _downloadPhotos(downloader);
                }

                //Descargar perfil
                if (!_cancel && downloadProfile.Checked)
                {
                    _downloadProfile(downloader);
                }

                //Descargar mensajes
                if (!_cancel && downloadMessages.Checked)
                {
                    _downloadMessages(downloader);
                }

                if (!_cancel)
                {
                    MessageBox.Show("FIN!");
                }
            }
            else
            {
                _cancel = true;
                button1.Text = "Comenzar";
            }
            savePath.Enabled = true;
            button2.Enabled = true;
        }

        private void _downloadPhotos(HtmlDownloader downloader)
        {
            int photo = 0;
            bool more_comments, more_pages;
            do
            {
                int page = 0;

                //Recorrer comentarios
                do
                {
                    if (_cancel)
                        return;

                    //Calcular número de página
                    Regex regex = new Regex("(\\d+) de \\d+", RegexOptions.IgnoreCase);
                    var match = regex.Match(webBrowser.Document.Body.InnerText);
                    if (match.Success)
                    {
                        photo = int.Parse(match.Groups[1].Value) - 1;
                    }

                    //Parsear HTML
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.OptionDefaultStreamEncoding = Encoding.UTF8;
                    document.LoadHtml(webBrowser.Document.Body.Parent.OuterHtml);

                    //Corregir página
                    Tuenti.FixPhotoPage(document, photo, page);

                    //Guardar página
                    downloader.Download(document, Tuenti.GetPhotoPath(photo, page));

                    //Recorrer comentarios
                    more_comments = _moveNext("photo_wall");

                    page++;
                } while (more_comments);

                //Recorrer páginas
                more_pages = _moveNext("photo_pager");
                photo++;
            } while (more_pages);
        }

        private void _downloadMessages(HtmlDownloader downloader)
        {
            //Navegar a los mensajes
            webBrowser.Navigate("http://www.tuenti.com/#m=Message");
            webBrowser.WaitLoad(true);

            //Inyectar jQuery
            webBrowser.InjectJS(Properties.Resources.jQueryLoader);

            int page = 0;
            bool more_pages;
            do
            {

                //Calcular número de página
                Regex regex = new Regex("(\\d+) de \\d+", RegexOptions.IgnoreCase);
                var match = regex.Match(webBrowser.Document.Body.InnerText);
                if (match.Success)
                {
                    page = int.Parse(match.Groups[1].Value) - 1;
                }

                //Obtener ids de los mensajes
                List<string> messageIds = new List<string>();
                foreach (HtmlElement node in webBrowser.Document.All)
                {
                    string id = node.GetAttribute("threadid");
                    if (!string.IsNullOrEmpty(id) && !messageIds.Contains(id) && Regex.IsMatch(id,"^\\d+$"))
                        messageIds.Add(id);
                }

                //Parsear HTML
                var document = new HtmlAgilityPack.HtmlDocument();
                document.OptionDefaultStreamEncoding = Encoding.UTF8;
                document.LoadHtml(webBrowser.Document.Body.Parent.OuterHtml);

                //Corregir página
                Tuenti.FixMessagePage(document, page, -1);

                //Guardar página
                downloader.Download(document, Tuenti.GetMessagePath(page));

               //Recorrer mensajes
               int message = 1;
               foreach (string tid in messageIds)
                {
                    if (_cancel)
                        return;

                    //Abrir mensaje
                    webBrowser.Document.InvokeScript("eval", new object[] { "jQuery('div[threadid="+tid+"]').click()" });
                    webBrowser.WaitLoad();

                    //Cargar mensajes antiguos
                    bool foundMoreMessages;
                    do
                    {
                        foundMoreMessages = false;
                        foreach (HtmlElement element in webBrowser.GetElementsByClassName("viewMore"))
                        {
                            foreach (HtmlElement link in element.GetElementsByTagName("a"))
                            {
                                foundMoreMessages = true;
                                link.InvokeMember("click");

                                    webBrowser.WaitLoad();
                            }
                        }
                    } while (foundMoreMessages);

                    //Parsear HTML
                     document = new HtmlAgilityPack.HtmlDocument();
                   document.OptionDefaultStreamEncoding = Encoding.UTF8;
                    document.LoadHtml(webBrowser.Document.Body.Parent.OuterHtml);

                    //Corregir página
                    Tuenti.FixMessagePage(document, page, message);

                    //Guardar página
                    downloader.Download(document, Tuenti.GetMessagePath(page, message));

                    message++;

                   //Cerrar mensaje
                    webBrowser.Document.InvokeScript("eval", new object[] { "jQuery('.author[threadid=" + tid + "]:visible').click()" });
                    Application.DoEvents();
                    webBrowser.WaitLoad();
                }

               webBrowser.WaitLoad();//Necesario para que el DOM se actualice

                //Recorrer páginas
                more_pages = _moveNext("pager_overlay");
                page++;


                webBrowser.WaitLoad();//Necesario para que el DOM se actualice
            } while (more_pages);
        }

        private void _downloadProfile(HtmlDownloader downloader)
        {
            //Ir al perfil
            webBrowser.Navigate("http://www.tuenti.com/#m=Profile&func=index");
            webBrowser.WaitLoad(true);

            int page = 0;
            bool morePages;
            do
            {
                //Parsear HTML
                var document = new HtmlAgilityPack.HtmlDocument();
                document.OptionDefaultStreamEncoding = Encoding.UTF8;
                document.LoadHtml(webBrowser.Document.Body.Parent.OuterHtml);

                //Corregir página
                Tuenti.FixProfilePage(document, page);

                //Guardar página
                downloader.Download(document, Tuenti.GetProfilePath(page));

                //Recorrer páginas
                morePages = _moveNext("wall_body");
                page++;
            } while (morePages);
        }

        private bool _moveNext(string id)
        {
            Application.DoEvents();

            var elm = webBrowser.Document.GetElementById(id);
            if (elm != null)
            {
                foreach (HtmlElement link in elm.GetElementsByTagName("a"))
                {
                    if (link.GetAttribute("className") == "next" ||
                        link.GetAttribute("class") == "next")
                    {
                        link.InvokeMember("click");

                        //Esperar que se cargue
                        webBrowser.WaitLoad();

                        return true;
                    }
                }
            }
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                savePath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStripStatusLabel1.Text ="Cargando "+ e.Url.ToString();
        }

        private void webBrowser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Maximum = (int)e.MaximumProgress;
            toolStripProgressBar1.Value = (int)e.CurrentProgress;
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            toolStripStatusLabel1.Text = webBrowser.Url.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (groupBox1.Visible)
            {
                groupBox1.Visible = false;
                button3.Text = "Mostrar opciones";
            }
            else
            {

                groupBox1.Visible = true;
                button3.Text = "Ocultar opciones";
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            webBrowser.Timeout = (int)numericUpDown1.Value;
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/javiermarinros");
        }
    }
}
