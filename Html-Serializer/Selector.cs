

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }


        public static Selector MapToSelector(string query)
        {

            var queryParts = query.Split(' ');
            Selector rootSelector = null;
            Selector currentSelector = null;
            var AllTags = HtmlHelper.Instance.HtmlTags;
            foreach (var part in queryParts)
            {
                var newSelector = new Selector();
                if (rootSelector == null)
                {
                    rootSelector = newSelector;
                    currentSelector = rootSelector;
                }
                else
                {
                    currentSelector.Child = newSelector;
                    newSelector.Parent = currentSelector;
                    currentSelector = newSelector;
                }
                var partsTag = part.Split(new[] { '#', '.' }, StringSplitOptions.None);
                if (partsTag.Length > 0 && !string.IsNullOrEmpty(partsTag[0]) && AllTags.Contains(partsTag[0]))
                {
                    // אם החלק לא מתחיל ב-# או ב-. וגם תגית חוקית זה השם של תגית HTML
                    currentSelector.TagName = partsTag[0];
                }
                for (var i = 1; i < partsTag.Length; i++)
                {
                    if (part.Contains('#' + partsTag[i]))
                        currentSelector.Id = partsTag[i];
                    else if (part.Contains('.'))
                        currentSelector.Classes.Add(partsTag[i]);
                }

            }
            return rootSelector;

        }


    }
}

