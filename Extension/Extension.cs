using System.Text.RegularExpressions;

namespace E_Commerce.Extension {
    public static class Extension {
        public static string ToVnd(this double price) {
            return price.ToString("#,###,##0") + "VND";
        }

        public static string ToTitleCase(string str) {
            string result = str;

            if (!string.IsNullOrEmpty(str)) {
                var words = str.Split(' ');

                for (int index = 0; index < words.Length; index++) {
                    var s = words[index];

                    if (s.Length > 0) {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }

            return result;
        }
    }
}