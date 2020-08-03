 
 /*   
            Get the latest mirror download address of Microsoft system
            headaccept：Content type accepted by header file
            contentype：Request media type
            urlref:Some websites will verify the Referer address
            head：Header file content
            mycookiecontainer：Cookie container, for insurance, every time the cookies header item is extracted and added to the container
            redirect_geturl：Jump address
 */
            Url = "https://www.microsoft.com/en-us/software-download/windows10ISO";       //Download from the official website
            var ResponseString1 = RequestGet(url1, headaccept, contentype, urlref, head1, mycookiecontainer, out redirect_geturl);
 
            var url2 = "https://www.microsoft.com/en-us/silentauth";   //log in
            RequestGet(url2, headaccept, contentype, url1, head1, mycookiecontainer, out redirect_geturl);  //Jump address
            RequestGet(redirect_geturl, headaccept, contentype, url1, head1, mycookiecontainer, out redirect_geturl); //Jump
            RequestGet(redirect_geturl, headaccept, contentype, url1, head1, mycookiecontainer, out redirect_geturl); //Jump
            var ResponseString5 = RequestGet(redirect_geturl, headaccept, contentype, url1, head1, mycookiecontainer, out redirect_geturl);
            string urlPost = "", sessionId = "";   //Get the post address and sessionId
            MatchCollection match5 = Regex.Matches(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ResponseString5)), @"\{(?:[^\{\}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!))\}", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match match in match5)
            {
                try
                {
                    JToken token = JObject.Parse(match.Value);
                    if (token.SelectToken("$.urlPost") != null)
                    {
                        urlPost = (string)token.SelectToken("$.urlPost");
                    }
                    if (token.SelectToken("$.sessionId") != null)
                    {
                        sessionId = (string)token.SelectToken("$.sessionId");
                    }
                }
                catch
                {
                }
            }
            var ResponseString6 = RequestGet("https://login.microsoftonline.com" + urlPost, headaccept, contentype, url1, head1, mycookiecontainer, out redirect_geturl);
            string action_url = "", error = "", error_description = "", state = "";   //Get POST submission URL and POST submission content
            HtmlAgilityPack.HtmlDocument htmldocument6 = new HtmlAgilityPack.HtmlDocument();
            htmldocument6.LoadHtml(ResponseString6);
            HtmlNodeCollection formNodes6 = htmldocument6.DocumentNode.SelectNodes("//form");
            foreach (HtmlNode formNode in formNodes6)
            {
                if (formNode.Name == "form")
                {
                    action_url = formNode.GetAttributeValue("action", "");
                    break;
                }

            }
            foreach (HtmlNode node in htmldocument6.DocumentNode.SelectNodes("//input"))
            {

                String name = node.GetAttributeValue("name", "");
                if (name == "error")
                {
                    error = node.GetAttributeValue("value", "");
                }
                if (name == "error_description")
                {
                    error_description = node.GetAttributeValue("value", "");
                }
                if (name == "state")
                {
                    state = node.GetAttributeValue("value", "");
                }

            }
          
            string postdata7 = "error=" + HttpUtility.UrlEncode(error) + "&error_description=" + HttpUtility.UrlEncode(error_description) + "&state=" + HttpUtility.UrlEncode(state);
            var ResponseString7 = RequestPost(action_url, "*/*", contentype, url6, head1, postdata7, mycookiecontainer, out redirect_posturl);
            //Get language list
            WebHeaderCollection head8 = new WebHeaderCollection()     //Head content structure
                           {
                               {"Request-Context", "appId=cid-v1:1b965f14-4848-4cb3-9553-535435b89811" },
                               {"Request-Id", "|zIDHB.8SyaY" },
                               {"Accept-Encoding:gzip, deflate"},
                               {"X-Requested-With", "XMLHttpRequest"},
                               {"Cache-Control","no-cache"}
                           };           
            contentype = "application/x-www-form-urlencoded";
            var url8 = "https://www.microsoft.com/en-us/api/controls/contentinclude/html?pageId=a8f8f489-4c7f-463a-9ca6-5cff94d8d041&host=www.microsoft.com&segments=software-download%2cwindows10ISO&query=&action=getskuinformationbyproductedition&sessionId=" + sessionId + "&productEditionId=" + productEditionId + "&sdVersion=2";
            var postdata8 = "controlAttributeMapping=";
            var ResponseString8 = RequestPost(url8, "*/*", contentype, url1, head8, postdata8, mycookiecontainer, out redirect_posturl);
            
            var jsons="";
            var retstr = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ResponseString8));
            HtmlAgilityPack.HtmlDocument htmldocument8 = new HtmlAgilityPack.HtmlDocument();
            htmldocument8.LoadHtml(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ResponseString8)));
            HtmlNode.ElementsFlags.Remove("option"); // need this before you do anything with HAP
            var options = htmldocument8.DocumentNode.SelectNodes("//select[@id='product-languages']/option");
            for (var i = 0; i < options.Count; i++)
            {
                var val = options[i].InnerHtml;
                var value = options[i].GetAttributeValue("value", "/");
                var jsonstring = Regex.Replace((string)Regex.Match(options[i].OuterHtml, @"\{(?:[^\{\}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!))\}").Groups[0].Value, @"\s", "");
                if (jsonstring != "")
                    jsons = jsons + "," + jsonstring;
            }
            try
            {
                jsons = "{\"value\":[" + jsons.TrimStart(',').Replace("=\"\"", " ") + "]}";
                JToken token8 = JObject.Parse(jsons);
                //languages = (string)token8.SelectToken("$.language");
                //lanuageID = (string)token8.SelectToken("$.id");
                string combox = "";
                comboBox4.Invoke(new MethodInvoker(delegate { combox=comboBox4.Text; }));
                lanuageID = token8.SelectToken("$.value[?(@.language == '" + combox + "')].id").ToString().ToLower();
            }
            catch
            {
            }



            //Get download link       
            WebHeaderCollection head9 = new WebHeaderCollection()
                           {
                               {"Request-Context", "appId=cid-v1:1b965f14-4848-4cb3-9553-535435b89811" },
                               {"Request-Id", "|zIDHB.8SyaY" },
                               {"Accept-Encoding:gzip, deflate"},
                               {"X-Requested-With", "XMLHttpRequest"},
                               {"Cache-Control","no-cache"}
                           };
            contentype = "application/x-www-form-urlencoded";
            var url9 = "https://www.microsoft.com/en-us/api/controls/contentinclude/html?pageId=cfa9e580-a81e-4a4b-a846-7b21bf4e2e5b&host=www.microsoft.com&segments=software-download%2cwindows10ISO&query=&action=GetProductDownloadLinksBySku&sessionId=" + sessionId + "&skuId=" + lanuageID + "&language=" + HttpUtility.HtmlEncode(comboxlan.text) + "&sdVersion=2";
            var postdata9 = "controlAttributeMapping=";
            var ResponseString9 = RequestPost(url9, "*/*", contentype, url1, head9, postdata9, mycookiecontainer, out redirect_posturl);
            HtmlAgilityPack.HtmlDocument htmldocument9 = new HtmlAgilityPack.HtmlDocument();
            htmldocument9.LoadHtml(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ResponseString9)));
            foreach (HtmlNode divNode in htmldocument9.DocumentNode.SelectNodes("//div[@class='row-fluid']"))
            {
                HtmlNode[] nodes = divNode.SelectNodes(".//a").ToArray();
                foreach (HtmlNode item in nodes)
                {
                    if (item.InnerText == "IsoX86 Download")
                    {
                        download32 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(item.GetAttributeValue("href", "")));
                        break;
                    }
                    if (item.InnerText == "IsoX64 Download")
                    {
                        download64 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(item.GetAttributeValue("href", "")));
                        break;
                    }
                }

            }
