using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string,string> Attributes { get; set; }=new Dictionary<string, string>();
        public List<string> Classes { get; set; }=new List<string>();
        public string InnerHtml { get; set; }

        public HtmlElement  Parent{ get; set; }
        public List<HtmlElement>  Children{ get; set; }=new List<HtmlElement> {};

        public IEnumerable<HtmlElement> Descendants()
        {
            HtmlElement current = null;
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while(queue.Any())
            {
                current = queue.Dequeue();
                foreach (HtmlElement element in current.Children)
                {
                    queue.Enqueue(element);
                }
                yield return current;
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public HashSet<HtmlElement> FindElement(Selector selector)
        {
            HashSet<HtmlElement>res= new HashSet<HtmlElement>();  
            FindElement(selector,this, res);
            return res;
        }

        public void FindElement(Selector selector, HtmlElement htmlElement, HashSet<HtmlElement> res)
        {

            if (selector == null || htmlElement == null)
                return;

            // ריצה על כל הצאצאים של האלמנט הנוכחי
            foreach (HtmlElement element in htmlElement.Descendants())
            {
                // בודקים אם האלמנט מתאים לסלקטור
                if (MatchesSelector(selector, element))
                {
                    if (selector.Child == null)
                        res.Add(element);

                    else
                        FindElement(selector.Child, element, res);

                }
            }
        }


        private bool MatchesSelector(Selector selector, HtmlElement htmlElement)
        {
            // בדוק אם האלמנט מתאים לסלקטור
            if (selector.TagName != null && htmlElement.Name != selector.TagName)
                return false;

            if (selector.Id != null && htmlElement.Id != selector.Id)
                return false;

            if (selector.Classes.Count > 0 && !htmlElement.Classes.Any(c => selector.Classes.Contains(c)))
                return false;

            return true;
        }

        public override string ToString()
        {
            string s = "\nName: " + Name + "\n Id: " + Id + "\n";
            foreach (string atr in Attributes.Keys)
            {
                s += "[" + atr + ": " + Attributes[atr] + "] ";
            }
            if (Classes.Count != 0)
            {
                s += "\nClass:";
                foreach (string class1 in Classes)
                {
                    s += " " + class1;
                }
            }
            s += "\nInnerHtml: " + InnerHtml;
            return s;
        }

    }
}
