using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TuentiDownloader
{
    class Tuenti
    {
        public static string SavePath{get;set;}

        public static void FixPhotoPage(HtmlAgilityPack.HtmlDocument document, int photo, int page)
        {
            //Corregir enlace que permite ir a la foto siguiente
            var photoElm = document.GetElementbyId("photo_action");
            if (photoElm != null)
            {
                photoElm.Attributes["href"].Value = Path.GetFileName(GetPhotoPath(photo + 1));
                photoElm.Attributes.Remove("onclick");
            }
           
            //Corregir paginadores
            _fixPager(document, "#photo_pager", photo, i => GetPhotoPath(i));

            _fixPager(document, "#photo_wall", page, i => GetPhotoPath(photo,i));

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

        public static void FixMessagePage(HtmlAgilityPack.HtmlDocument document, int page, int message)
        {
           // var rows = document.DocumentNode.SelectNodes(MostThingsWeb.css2xpath.Transform("#mail_box_body tr:not( [noexpand] )"));
            var rows = document.DocumentNode.SelectNodes(" //*[@id='mail_box_body']//tr[not(@noexpand)]");
           
            if (rows != null)
            {
                int currentMessage=1;
                foreach (HtmlNode row in rows)
                {
                    string url =Path.GetFileName(GetMessagePath(page, message==currentMessage?0:currentMessage));
                    row.Attributes.Add("onclick", "document.location='"+url+"'");
                    currentMessage++;
                }
            }

            //Corregir enlace del mensaje actual para que vaya a la pagina
            var current = document.DocumentNode.SelectSingleNode(MostThingsWeb.css2xpath.Transform(".expanded:not(.hide)"));
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

        public static void FixProfilePage(HtmlAgilityPack.HtmlDocument document, int page)
        {
            //Corregir enlaces a las últimas fotos
            var latestPhotos = document.DocumentNode.SelectNodes(MostThingsWeb.css2xpath.Transform("#latest_photos li a"));
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

        private static void _fixCommon(HtmlAgilityPack.HtmlDocument document)
        {

            //Enlace a los mensajes
            var messageLink = document.GetElementbyId("tab_message");
            if (messageLink != null)
            {
                messageLink.Attributes["href"].Value = GetMessagePath(0);
                messageLink.Attributes.Remove("onclick");
            }
            //Enlace al perfil
            var profileLink = document.GetElementbyId("tab_profile");
            if (profileLink != null)
            {
                profileLink.Attributes["href"].Value = GetProfilePath(0);
                profileLink.Attributes.Remove("onclick");
            }

            //Eliminar publicidad y otros elementos innecesarios (chat)
            foreach (string ad in new[] { "ltaAdItem", "overlay_ad_container", "trigger-exclusive_sponsorships", "sponsorships_list", "chat_dock" })
            {
                var adElm = document.GetElementbyId(ad);
                if (adElm != null)
                {
                    adElm.Remove();
                }
            }

            //Ajustar estilos
            var pagers = document.DocumentNode.SelectNodes("//*[contains(@class,'pager')]");
            if (pagers != null)
            {
                foreach (HtmlNode pager in pagers)
                {
                    pager.Attributes.Add("style", "height:auto;");
                }
            }
        }

        private delegate TReturn Func<TReturn, TArg>(TArg arg);

        private static void _fixPager(HtmlAgilityPack.HtmlDocument document, string selector, int currentPos, Func<string, int> getPath, int total=-1)
        {
            var firsts = document.DocumentNode.SelectNodes(MostThingsWeb.css2xpath.Transform(selector + " a.first"));
            if (firsts != null)
            {
                foreach (HtmlNode first in firsts)
                {
                    first.Attributes["href"].Value = Path.GetFileName(getPath(0));
                    first.Attributes.Remove("onclick");
                }
            }

            var lasts = document.DocumentNode.SelectNodes(MostThingsWeb.css2xpath.Transform(selector + " a.last"));
            if (lasts != null)
            {
                //Calcular el total
                Regex regex = new Regex("\\d+ de (\\d+)", RegexOptions.IgnoreCase);
                var match = regex.Match(document.DocumentNode.InnerText);
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

            var nexts = document.DocumentNode.SelectNodes(MostThingsWeb.css2xpath.Transform(selector + " a.next"));
            if (nexts != null)
            {
                foreach (HtmlNode next in nexts)
                {
                    next.Attributes["href"].Value = Path.GetFileName(getPath(currentPos + 1));
                    next.Attributes.Remove("onclick");
                }
            }

            var prevs = document.DocumentNode.SelectNodes(MostThingsWeb.css2xpath.Transform(selector + " a.prev"));
            if (prevs != null)
            {
                foreach (HtmlNode prev in prevs)
                {
                prev.Attributes["href"].Value = Path.GetFileName(getPath(currentPos - 1));
                prev.Attributes.Remove("onclick");
                }
            }
        }
    }
}
