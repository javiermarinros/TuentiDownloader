using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MostThingsWeb;

namespace TuentiDownloader
{
    internal class Tuenti
    {
        public static string SavePath { get; set; }

        public static void FixPhotoPage(HtmlDocument document, int photo, int page)
        {
            //Corregir enlace que permite ir a la foto siguiente
            HtmlNode photoElm = document.GetElementbyId("photo_action");
            if (photoElm != null)
            {
                photoElm.Attributes["href"].Value = Path.GetFileName(GetPhotoPath(photo + 1));
                photoElm.Attributes.Remove("onclick");
            }

            //Corregir paginadores
            _fixPager(document, "#photo_pager", photo, i => GetPhotoPath(i));

            _fixPager(document, "#photo_wall", page, i => GetPhotoPath(photo, i));

            //Ajustes comunes
            _fixCommon(document);
        }


        public static string GetPhotoPath(int photo, int page = 0)
        {
            string name = "Foto " + (photo + 1);
            //   if (page > 0)
            //     {
            name += ", pagina " + (page + 1);
            //     }
            return Path.Combine(SavePath, name + ".html");
        }

        public static void FixMessagePage(HtmlDocument document, int page, int message)
        {
            // var rows = document.DocumentNode.SelectNodes(MostThingsWeb.css2xpath.Transform("#mail_box_body tr:not( [noexpand] )"));
            HtmlNodeCollection rows = document.DocumentNode.SelectNodes(" //*[@id='mail_box_body']//tr[not(@noexpand)]");

            if (rows != null)
            {
                int currentMessage = 1;
                foreach (HtmlNode row in rows)
                {
                    string url = Path.GetFileName(GetMessagePath(page, message == currentMessage ? 0 : currentMessage));
                    row.Attributes.Add("onclick", "document.location='" + url + "'");
                    currentMessage++;
                }
            }

            //Corregir enlace del mensaje actual para que vaya a la pagina
            HtmlNode current = document.DocumentNode.SelectSingleNode(css2xpath.Transform(".expanded:not(.hide)"));
            if (current != null)
            {
                string url = Path.GetFileName(GetMessagePath(page));
                current.Attributes.Add("onclick", "document.location='" + url + "'");
            }

            //Corregir paginador
            _fixPager(document, "#pager_overlay", page, i => GetMessagePath(i));

            //Ajustes comunes
            _fixCommon(document);
        }

        public static string GetMessagePath(int page, int message = 0)
        {
            string name = "Mensajes pagina " + (page + 1);
            name += ", mensaje " + (message + 1);
            return Path.Combine(SavePath, name + ".html");
        }

        public static void FixProfilePage(HtmlDocument document, int page)
        {
            //Corregir enlaces a las últimas fotos
            HtmlNodeCollection latestPhotos =
                document.DocumentNode.SelectNodes(css2xpath.Transform("#latest_photos li a"));
            if (latestPhotos != null)
            {
                int currentPhoto = 0;
                foreach (HtmlNode photo in latestPhotos)
                {
                    photo.Attributes["href"].Value = Path.GetFileName(GetPhotoPath(currentPhoto));
                    currentPhoto++;
                }
            }

            //Corregir paginador
            _fixPager(document, "#wall #pager_overlay", page, GetProfilePath);

            //Ajustes comunes
            _fixCommon(document);
        }

        public static string GetProfilePath(int page)
        {
            string name = "Perfil pagina " + (page + 1);
            return Path.Combine(SavePath, name + ".html");
        }

        private static void _fixCommon(HtmlDocument document)
        {
            //Enlace a los mensajes
            HtmlNode messageLink = document.GetElementbyId("tab_message");
            if (messageLink != null)
            {
                messageLink.Attributes["href"].Value = Path.GetFileName(GetMessagePath(0));
                messageLink.Attributes.Remove("onclick");
            }

            //Enlace al perfil
            HtmlNode profileLink = document.GetElementbyId("tab_profile");
            if (profileLink != null)
            {
                profileLink.Attributes["href"].Value = Path.GetFileName(GetProfilePath(0));
                profileLink.Attributes.Remove("onclick");
            }

            //Eliminar publicidad y otros elementos innecesarios (chat)
            foreach (
                string ad in
                    new[]
                        {
                            "ltaAdItem", "overlay_ad_container", "trigger-exclusive_sponsorships", "sponsorships_list",
                            "chat_dock", "ie_deprecated_browser_banner", "plugin_detection_warning_layer"
                        })
            {
                HtmlNode adElm = document.GetElementbyId(ad);
                if (adElm != null)
                {
                    adElm.Remove();
                }
            }

            //Ajustar estilos
            HtmlNodeCollection pagers = document.DocumentNode.SelectNodes("//*[contains(@class,'pager')]");
            if (pagers != null)
            {
                foreach (HtmlNode pager in pagers)
                {
                    pager.Attributes.Add("style", "height:auto;");
                }
            }
        }

        private static void _fixPager(HtmlDocument document, string selector, int currentPos, Func<string, int> getPath,
                                      int total = -1)
        {
            HtmlNodeCollection firsts = document.DocumentNode.SelectNodes(css2xpath.Transform(selector + " a.first"));
            if (firsts != null)
            {
                foreach (HtmlNode first in firsts)
                {
                    first.Attributes["href"].Value = Path.GetFileName(getPath(0));
                    first.Attributes.Remove("onclick");
                }
            }

            HtmlNodeCollection lasts = document.DocumentNode.SelectNodes(css2xpath.Transform(selector + " a.last"));
            if (lasts != null)
            {
                //Calcular el total
                var regex = new Regex("\\d+ de (\\d+)", RegexOptions.IgnoreCase);
                Match match = regex.Match(document.DocumentNode.InnerText);
                if (match.Success || total > 0)
                {
                    if (total < 0)
                        total = int.Parse(match.Groups[1].Value) - 1;

                    foreach (HtmlNode last in lasts)
                    {
                        last.Attributes["href"].Value = Path.GetFileName(getPath(total));
                        last.Attributes.Remove("onclick");
                    }
                }
            }

            HtmlNodeCollection nexts = document.DocumentNode.SelectNodes(css2xpath.Transform(selector + " a.next"));
            if (nexts != null)
            {
                foreach (HtmlNode next in nexts)
                {
                    next.Attributes["href"].Value = Path.GetFileName(getPath(currentPos + 1));
                    next.Attributes.Remove("onclick");
                }
            }

            HtmlNodeCollection prevs = document.DocumentNode.SelectNodes(css2xpath.Transform(selector + " a.prev"));
            if (prevs != null)
            {
                foreach (HtmlNode prev in prevs)
                {
                    prev.Attributes["href"].Value = Path.GetFileName(getPath(currentPos - 1));
                    prev.Attributes.Remove("onclick");
                }
            }
        }

        private delegate TReturn Func<TReturn, TArg>(TArg arg);
    }
}