

//using Html_Serializer;
//using System.Text.RegularExpressions;
//using System.Xml.Linq;

//async Task<string> Load(string url)
//{
//    HttpClient client = new HttpClient();
//    var response = await client.GetAsync(url);
//    var html = await response.Content.ReadAsStringAsync();
//    return html;
//}
//static string FirstWordInString(string s)
//{
//    return s.Trim().Split(' ')[0];
//}
//static void BuildTree(List<string> htmLines, HtmlElement rootElement)
//{
//    HtmlElement currentElement = rootElement;
//    string tagName, remainingContent;

//    //סוגי תגיות
//    var AllTags = HtmlHelper.Instance.HtmlTags;
//    var SelfClosingTags = HtmlHelper.Instance.HtmlVoidTags;

//    foreach (var line in htmLines.Skip(1))
//    {
//        if (line.StartsWith('/'))
//        {
//            if (FirstWordInString(line.Substring(1)) == "html")
//            {
//                rootElement = rootElement.Children[0];
//                return;
//            }
//            currentElement = currentElement.Parent;
//        }
//        else
//        {
//            tagName = FirstWordInString(line);
//            if (AllTags.Contains(tagName) || SelfClosingTags.Contains(tagName)) 
//            {

//                //עדכון העץ
//                HtmlElement newElement = new HtmlElement();
//                currentElement.Children.Add(newElement);
//                newElement.Parent = currentElement;
//                currentElement = newElement;

//                currentElement.Name = tagName;

//                remainingContent = line.Substring(tagName.Length).Trim();
//                var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(remainingContent);

//                foreach (Match match in attributes) 
//                {
//                    if (match.Groups[1].Value == "id")
//                        currentElement.Id = match.Groups[2].Value;

//                    else if (match.Groups[1].Value == "class")
//                        currentElement.Classes = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

//                    else 
//                        currentElement.Attributes.Add(match.Groups[1].Value, match.Groups[2].Value);
//                }

//                if (SelfClosingTags.Contains(tagName) || remainingContent.EndsWith('/'))
//                    currentElement = currentElement.Parent;

//            }
//            else 
//                currentElement.InnerHtml = line;
//            }
//        }
//    }
//}

//var html = await Load(" https://hebrewbooks.org/beis ");


//var cleanHtml = new Regex("\\s").Replace(html, " ");
//var tagMatches = Regex.Matches(cleanHtml, @"<\/?([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>|([^<]+)").Where(l => !String.IsNullOrWhiteSpace(l.Value));

//var htmLines = new List<string>();
//foreach (Match item in tagMatches)
//{
//    string tag = item.Value.Trim();
//    if (tag.StartsWith('<'))
//        tag = tag.Trim('<', '>');
//    htmLines.Add(tag);
//}

//HtmlElement rootElement = new HtmlElement();

//BuildTree(htmLines, rootElement);

//var res = rootElement.FindElement(Selector.MapToSelector("ul.nav.navbar-nav"));
//Selector selector = Selector.MapToSelector("div a.inactBG");

//HashSet<HtmlElement> elements = rootElement.FindElement(selector);
//foreach (HtmlElement element in elements)
//{
//    Console.WriteLine(element);
//    Console.WriteLine("----------------------------");
//}


//Console.ReadLine();









using Html_Serializer;
using System.Text.RegularExpressions;


    static async Task<string> LoadHtmlAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }

    //שליפת שם התגית
    static string ExtractFirstWord(string line) => line.Trim().Split(' ')[0];
    
    //מעבר על כל התגיות
    static void ParseHtmlLines(List<string> htmLines, HtmlElement rootElement)
    {
        HtmlElement currentElement = rootElement;

        var allTags = HtmlHelper.Instance.HtmlTags;
        var selfClosingTags = HtmlHelper.Instance.HtmlVoidTags;

        foreach (var line in htmLines.Skip(1))
        {
            ProcessLine(line, ref currentElement, allTags, selfClosingTags);
        }
    }

    //פענוח כל תגית
    static void ProcessLine(string line, ref HtmlElement currentElement,string[] allTags, string[] selfClosingTags)
    {
        if (line.StartsWith('/'))
        {
            HandleClosingTag(line, ref currentElement);
        }
        else
        {
            HandleOpeningTag(line, ref currentElement, allTags, selfClosingTags);
        }
    }

    //תגית סוגרת
    static void HandleClosingTag(string line, ref HtmlElement currentElement)
    {
        if (ExtractFirstWord(line.Substring(1)) == "html")
        {
            currentElement = currentElement.Children[0];
        }
        else
        {
            currentElement = currentElement.Parent;
        }
    }
    
    //תגית פותחת
    static void HandleOpeningTag(string line, ref HtmlElement currentElement, string[] allTags, string[] selfClosingTags)
    {
        var tagName = ExtractFirstWord(line);
        if (allTags.Contains(tagName) || selfClosingTags.Contains(tagName))
        {
            HtmlElement newElement = new HtmlElement
            {
                Name = tagName,
                Parent = currentElement
            };
            currentElement.Children.Add(newElement);
            currentElement = newElement;

            ParseAttributes(line.Substring(tagName.Length).Trim(), currentElement);

            if (selfClosingTags.Contains(tagName) || line.EndsWith('/'))
                currentElement = currentElement.Parent;
        }
        else
        {
            currentElement.InnerHtml = line;
        }
    }

    //פענוח מאפייני התגית
    static void ParseAttributes(string remainingContent, HtmlElement currentElement)
    {
        var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(remainingContent);
        foreach (Match match in attributes)
        {
            var name = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            if (name == "id")
                currentElement.Id = value;
            else if (name == "class")
                currentElement.Classes = value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            else
                currentElement.Attributes.Add(name, value);
        }
    }

    //החזרת רשימה של תגיות
    static List<string> CleanHtml(string html)
    {
        var cleanHtml = new Regex("\\s").Replace(html, " ");
        return Regex.Matches(cleanHtml, @"<\/?([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>|([^<]+)")
                    .Where(l => !string.IsNullOrWhiteSpace(l.Value))
                    .Select(item => item.Value.Trim().Trim('<', '>'))
                    .ToList();
    }

    
string url = "https://hebrewbooks.org/beis";
var html = await LoadHtmlAsync(url);
var htmLines = CleanHtml(html);

HtmlElement rootElement = new HtmlElement();
ParseHtmlLines(htmLines, rootElement);

var selector = Selector.MapToSelector("div a.inactBG");
var elements = rootElement.FindElement(selector);

foreach (var element in elements)
{
    Console.WriteLine(element);
    Console.WriteLine("----------------------------");
}
    

    


