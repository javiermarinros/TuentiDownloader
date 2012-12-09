using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TuentiDownloader.Properties;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace TuentiDownloader
{
    public partial class Main : Form
    {
        private bool _cancel;

        public Main()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            savePath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Tuenti");
            numericUpDown1_ValueChanged(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bToggleSettings.Enabled)
            {
                _cancel = false;
                if (groupBox1.Visible)
                    bToggleSettings.PerformClick();
                bToggleSettings.Enabled = false;
                bStart.Text = "Parar";

                Directory.CreateDirectory(savePath.Text);

                Tuenti.SavePath = savePath.Text;
                var downloader = new HtmlDownloader {ResourcePath = Path.Combine(savePath.Text, "Recursos")};


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
                    System.Media.SystemSounds.Exclamation.Play();
                    MessageBox.Show("TuentiDownloader ha terminado de descargar contenidos", "Fin de la descarga",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else{
                _cancel = true;
            }
            bStart.Text = "Comenzar";
            bToggleSettings.Enabled = true;
        }

        private void _downloadPhotos(HtmlDownloader downloader)
        {
            int photo = 0;
            int processed = 0;
            bool morePages;
            do
            {
                int page = 0;

                //Recorrer comentarios
                bool moreComments;
                do
                {
                    if (_cancel)
                        return;

                    //Calcular número de página
                    var regex = new Regex("(\\d+) de \\d+", RegexOptions.IgnoreCase);
                    Match match = regex.Match(webBrowser.Document.Body.InnerText);
                    if (match.Success)
                    {
                        photo = int.Parse(match.Groups[1].Value) - 1;
                    }

                    //Descargar página
                    _process(doc => Tuenti.FixPhotoPage(doc, photo, page), Tuenti.GetPhotoPath(photo, page), downloader);

                    //Recorrer comentarios
                    moreComments = _moveNext("photo_wall");

                    page++;
                } while (moreComments);

                //Recorrer páginas
                morePages = _moveNext("photo_pager");
                photo++;

                processed++;
                if (processed % 750 == 0)
                {
                    //Recargar la página para evitar cuelgues y fugas de memoria
                    webBrowser.Refresh();
                    webBrowser.WaitLoad(true);
                    webBrowser.WaitLoad(true);
                    webBrowser.WaitLoad(true);
                    webBrowser.WaitLoad(true);
                }
            } while (morePages);
        }

        private void _downloadMessages(HtmlDownloader downloader)
        {
            //Navegar a los mensajes
            webBrowser.Navigate("http://www.tuenti.com/#m=Message");
            webBrowser.WaitLoad(true);

            //Inyectar jQuery
            webBrowser.InjectJS(Resources.jQueryLoader);

            int page = 0;
            bool morePages;
            do
            {
                //Calcular número de página
                var regex = new Regex("(\\d+) de \\d+", RegexOptions.IgnoreCase);
                Match match = regex.Match(webBrowser.Document.Body.InnerText);
                if (match.Success)
                {
                    page = int.Parse(match.Groups[1].Value) - 1;
                }

                //Obtener ids de los mensajes
                var messageIds = new List<string>();
                foreach (HtmlElement node in webBrowser.Document.All)
                {
                    string id = node.GetAttribute("threadid");
                    if (!string.IsNullOrEmpty(id) && !messageIds.Contains(id) && Regex.IsMatch(id, "^\\d+$"))
                        messageIds.Add(id);
                }

                //Parsear HTML
                var document = new HtmlDocument {OptionDefaultStreamEncoding = Encoding.UTF8};
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
                    webBrowser.Document.InvokeScript("eval",
                                                     new object[] {"jQuery('div[threadid=" + tid + "]').click()"});
                    webBrowser.WaitLoad();

                    //Cargar mensajes antiguos
                    bool foundMoreMessages;
                    int i = 0;
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
                        i++;
                    } while (foundMoreMessages && i<30);

                    //Descargar página
                    _process(doc => Tuenti.FixMessagePage(doc, page, message), Tuenti.GetMessagePath(page, message), downloader);

                    message++;

                    //Cerrar mensaje
                    webBrowser.Document.InvokeScript("eval",
                                                     new object[]
                                                         {"jQuery('.author[threadid=" + tid + "]:visible').click()"});
                    Application.DoEvents();
                    webBrowser.WaitLoad();
                }

                webBrowser.WaitLoad(); //Necesario para que el DOM se actualice

                //Recorrer páginas
                morePages = _moveNext("pager_overlay");
                page++;


                webBrowser.WaitLoad(); //Necesario para que el DOM se actualice
            } while (morePages);
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
                //Descargar página
                _process(document => Tuenti.FixProfilePage(document, page), Tuenti.GetProfilePath(page), downloader);

                //Recorrer páginas
                morePages = _moveNext("wall_body");
                page++;
            } while (morePages);
        }

        private void _process(Action<HtmlDocument> fixer, string savePath, HtmlDownloader downloader)
        {
            try
            {
                //Parsear HTML
                var document = new HtmlDocument { OptionDefaultStreamEncoding = Encoding.UTF8 };
                document.LoadHtml(webBrowser.Document.Body.Parent.OuterHtml);

                //Corregir página
                fixer(document);

                //Guardar página
                downloader.Download(document, savePath);

                //Liberar memoria
                document = null;
                GC.Collect();
            }
            catch
            {
                File.WriteAllText(savePath,"Se produjo un error al recuperar la URL "+webBrowser.Url);
            }
        }

        private bool _moveNext(string id)
        {
            Application.DoEvents();

            HtmlElement elm = webBrowser.Document.GetElementById(id);
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
            toolStripStatusLabel1.Text = "Cargando " + e.Url;
        }

        private void webBrowser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Maximum = (int) e.MaximumProgress;
            toolStripProgressBar1.Value = (int) e.CurrentProgress;
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
                bToggleSettings.Text = "Mostrar opciones";
            }
            else
            {
                groupBox1.Visible = true;
                bToggleSettings.Text = "Ocultar opciones";
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            webBrowser.Timeout = (int) numericUpDown1.Value;
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            Process.Start("https://twitter.com/javiermarinros");
        }
    }
}