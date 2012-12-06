using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace TuentiDownloader
{
    internal class HtmlDownloader
    {
        public string ResourcePath { get; set; }

        public void Download(HtmlDocument document, string path)
        {
            Directory.CreateDirectory(ResourcePath);

            //Descargar todos los recursos
            _downloadImages(document);
            _downloadResources(document, "link", "href", true);
            _downloadResources(document, "script", "src");

            //Añadir cabeceras meta para indificar codificación UTF8
            HtmlNode meta = document.CreateElement("meta");
            meta.Attributes.Add("charset", "utf-8");
            document.DocumentNode.SelectSingleNode("//head").ChildNodes.Add(meta);

            /*HtmlNode meta2 = document.CreateElement("meta");
            meta2.Attributes.Add("http-equiv", "Content-Type");
            meta2.Attributes.Add("content", "Type=text/html; charset=utf-8");
            document.DocumentNode.SelectSingleNode("//head").ChildNodes.Add(meta2);*/

            File.WriteAllText(path, document.DocumentNode.WriteTo(), new UTF8Encoding(true));
        }

        private void _downloadResources(HtmlDocument document, string tagName, string hrefAttr, bool rewriteUrls = false)
        {
            // foreach (HtmlElement link in document.GetElementsByTagName(tagName))
            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//" + tagName))
            {
                try
                {
                    // string url = link.GetAttribute(hrefAttr);
                    string url = link.Attributes[hrefAttr].Value;

                    if (string.IsNullOrEmpty(url))
                        continue;

                    bool isNew;
                    string fileName = _downloadFile(url, out isNew);

                    if (isNew && rewriteUrls)
                    {
                        var regex = new Regex("url\\(\"?(.+?)\"?\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        string filePath = Path.Combine(ResourcePath, fileName);
                        string content = File.ReadAllText(filePath);
                        foreach (Match match in regex.Matches(content))
                        {
                            try
                            {
                                string resourceUrl = match.Groups[1].Value;
                                string filename = _downloadFile(resourceUrl, out isNew);
                                content = content.Replace(resourceUrl, filename);
                            }
                            catch
                            {
                            }
                        }
                        File.WriteAllText(filePath, content);
                    }

                    link.Attributes[hrefAttr].Value = "Recursos/" + fileName;
                }
                catch
                {
                }
            }
        }

        private string _downloadFile(string url, out bool isNew)
        {
            if (url.StartsWith("//"))
                url = "http:" + url;

            string fileName = _makeValidFileName(url);
            string filePath = Path.Combine(ResourcePath, fileName);

            if (!File.Exists(filePath) || new FileInfo(filePath).Length==0)
            {
                Application.DoEvents();

                using (var client = new WebClient())
                {
                    client.DownloadFile(url, filePath);
                    isNew = true;
                }

                Application.DoEvents();
            }
            else
            {
                isNew = false;
            }

            return fileName;
        }

        private void _downloadImages(HtmlDocument document)
        {
            foreach (HtmlNode imageElement in document.DocumentNode.SelectNodes("//img"))
            {
                try
                {
                    string url = imageElement.Attributes["src"].Value;
                    if (url.StartsWith("file://"))
                        continue;

                    bool isNew;
                    string fileName = _downloadFile(url, out isNew);

               //     if (isNew)
                        string filePath = Path.Combine(ResourcePath, fileName);
                        //Intentar ajustar la extensión
                    if (Path.GetExtension(fileName) == "" || Path.GetExtension(fileName).Length > 5)
                    {
                        try
                        {
                            string extension = ".jpg";
                            using (Image image = Image.FromFile(filePath))
                            {
                                if (ImageFormat.Jpeg.Equals(image.RawFormat))
                                {
                                    extension = ".jpg";
                                }
                                else if (ImageFormat.Png.Equals(image.RawFormat))
                                {
                                    extension = ".png";
                                }
                                else if (ImageFormat.Gif.Equals(image.RawFormat))
                                {
                                    extension = ".gif";
                                }
                            }

                            fileName += extension;
                            string newFilePath = Path.Combine(ResourcePath, fileName);

                            if (File.Exists(newFilePath))
                            {
                                File.Delete(filePath);
                            }
                            else
                            {
                                File.Move(filePath, newFilePath);
                            }
                            filePath = newFilePath;
                        }
                        catch
                        {
                        }
                    }

                    imageElement.Attributes["src"].Value = "Recursos/" + fileName;
                }
                catch
                {
                }
            }
        }

        private string _makeValidFileName(string name)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(name, "");
        }
    }
}