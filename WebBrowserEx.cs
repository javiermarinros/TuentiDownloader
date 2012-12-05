using mshtml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TuentiDownloader
{
    class WebBrowserEx:WebBrowser
    {
        private bool _navigated;

        public int Timeout { get; set; }

        public WebBrowserEx()
        {
            Timeout = 3000;
            Navigated += delegate
                {
                    _navigated = true;
                };
        }

        public void InjectJS(string source)
        {
            HtmlElementCollection head = Document.GetElementsByTagName("head");
            if (head != null)
            {
                HtmlElement scriptEl = Document.CreateElement("script");
                IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                element.text = global::TuentiDownloader.Properties.Resources.jQueryLoader;
                ((HtmlElement)head[0]).AppendChild(scriptEl);
            }
        }

        public void WaitLoad(bool fullWait=false)
        {
            _navigated = false;
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < Timeout)
            {
                Application.DoEvents();

                if (!fullWait && _navigated && ReadyState == WebBrowserReadyState.Complete)
                {
                    break;
                }
            }
            Application.DoEvents();
        }

        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            return Document.GetElementsByTagName(tagName);
        }

        public HtmlElement GetElementById(string id)
        {
            return Document.GetElementById(id);
        }

        public HtmlElement[] GetElementsByClassName(string className, HtmlElement element=null)
        {
            Regex regex = new Regex("\\b" + className + "\\b",RegexOptions.IgnoreCase|RegexOptions.Compiled);

            List<HtmlElement> res = new List<HtmlElement>();
            foreach (HtmlElement subElm in element==null?Document.All:element.Children)
            {
                if (regex.IsMatch(subElm.GetAttribute("class")) || regex.IsMatch(subElm.GetAttribute("className")))
                    res.Add(subElm);
            }
            return res.ToArray();
        }


        public HtmlElement FindElementByDomObject(object o)
        {
            //Intentar obtener desde su implementación mshtml para optimizar la ejecución
            Type type = o.GetType();

            //Método 1 - Comprobar si el elemento actual es el BODY
            try
            {
                if (type.Name == "HTMLBodyClass")
                    return Document.Body;
            }
            catch
            {
            }

            //Método 2 - Obtener desde la posición del elemento
            try
            {
                int x = (int)type.GetProperty("offsetLeft").GetValue(o, null);
                int y = (int)type.GetProperty("offsetTop").GetValue(o, null);
                HtmlElement element = Document.GetElementFromPoint(new Point(x, y));
                if (element.DomElement == o)
                    return element;
            }
            catch
            {
            }

            //Método 3 - Acotar el número de resultados usando GetElementsByTagName
            try
            {
                string tagName = type.GetProperty("tagName").GetValue(o, null) as string;
                if (!string.IsNullOrEmpty(tagName))
                {
                    foreach (HtmlElement element in Document.GetElementsByTagName(tagName))
                    {
                        if (element.DomElement == o)
                            return element;
                    }
                }
            }
            catch
            {
            }

            //Obtener desde el documento
            foreach (HtmlElement element in Document.All)
            {
                if (element.DomElement == o)
                    return element;
            }
            return null;
        }
    }
}
